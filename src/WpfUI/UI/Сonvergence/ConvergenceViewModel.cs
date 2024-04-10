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
		ReportingPathfinders = new(pathfinderRepository.GetAll()
			.Where(p => p.Method is IRepotingSalesmanPathfinder<int, int>));
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

	public ObservableCollection<Pathfinder> ReportingPathfinders { get; }

	public Pathfinder SelectedPathfinder { get; }

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
		long lastTime = 0;
        var progress = new Progress<int>(value =>
        {
			//if (sw.ElapsedMilliseconds - lastTime < period)
			//{
			//	return;
			//}
			lastTime = sw.ElapsedMilliseconds;
			values.Add(value);
			times.Add(lastTime / 1000.0);
            _messenger.Send(report);
        });
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Task.Run(async () =>
		{
			sw.Start();
			await _exhaustive.FindPathWithReportAsync(graph, progress, cts.Token);
			sw.Stop();
		}, cancellationToken);
		try
		{
			await Task.Delay(TestTimeInSeconds * 1000, cancellationToken);
		}
		catch (OperationCanceledException)
		{
		}
		sw.Stop();
		cts.Cancel();
        _messenger.Send(report);
    }
}
