using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WpfUI.Data;

namespace WpfUI.UI.Сonvergence;

public class ConvergenceViewModel : ObservableObject
{
	private readonly BranchAndBoundSalesmanPathfinder<int, int> _pathfinder;
	private readonly ExhaustiveSearchSalesmanPathfinder<int, int> _exhaustive;
	private readonly GraphHolder _graphHolder;
	private readonly IMessenger _messenger;

	public ConvergenceViewModel(
		BranchAndBoundSalesmanPathfinder<int, int> pathfinder,
		ExhaustiveSearchSalesmanPathfinder<int, int> exhaustive,
		PathfinderRepository pathfinderRepository,
		GraphHolder graphHolder,
		IMessenger messenger)
	{
		_pathfinder = pathfinder;
		_graphHolder = graphHolder;
		_exhaustive = exhaustive;
		ReportingPathfinders = new(pathfinderRepository.GetReportingPathfinders());
		SelectedPathfinder = ReportingPathfinders[0];
		_messenger = messenger;
		RunTestCommand = new AsyncRelayCommand(RunTest);
		CancelTestCommand = RunTestCommand.CreateCancelCommand();
	}

	private int _testTimeInSeconds = 10;
	public int TestTimeInSeconds
	{
		get => _testTimeInSeconds;
		set => SetProperty(ref _testTimeInSeconds, value);
	}

	private int _bestValue = 0;
	public int BestValue
    {
        get => _bestValue;
		set => SetProperty(ref _bestValue, value);
    }

    public ObservableCollection<ReportingPathfinder> ReportingPathfinders { get; }

	public ReportingPathfinder SelectedPathfinder { get; set; }

	public IAsyncRelayCommand RunTestCommand { get; }
	public ICommand CancelTestCommand { get; }

	private async Task RunTest(CancellationToken cancellationToken)
	{
		const int period = 500;
		var graph = _graphHolder.CrateGraph();
		int expectedPeriods = TestTimeInSeconds * 1000 / period;
		var times = new List<double>(expectedPeriods);
		var values = new List<int>(expectedPeriods);
		var report = new ConvergenceResult.ConvergenceResultChangedMessage(
            new ConvergenceResult(times, values));
		var sw = new Stopwatch();
		int lastValue = -1;
        var progress = new Progress<int>(value =>
        {
			lastValue = value;
        });
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var ctsForDelay = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Task.Run(async () =>
		{
			sw.Start();
			await SelectedPathfinder.Metgod.FindPathWithReportAsync(graph, progress, cts.Token);
			ctsForDelay.Cancel();
			sw.Stop();
		}, cancellationToken);
		_ = Task.Run(async () =>
		{
			while (!cts.Token.IsCancellationRequested)
			{
				if (lastValue < 0)
				{
					continue;
				}
                values.Add(lastValue);
                times.Add(sw.ElapsedMilliseconds / 1000.0);
                _messenger.Send(report);
                await Task.Delay(period);
            }
		}, cts.Token);
		try
		{
			await Task.Delay(TestTimeInSeconds * 1000, ctsForDelay.Token);
		}
		catch (OperationCanceledException)
		{
        }
        values.Add(lastValue);
        times.Add(sw.ElapsedMilliseconds / 1000.0);
        _messenger.Send(report);
        sw.Stop();
		cts.Cancel();
        _messenger.Send(report);
		if (values.Count > 0)
		{
			BestValue = values[^1];
		}
    }
}
