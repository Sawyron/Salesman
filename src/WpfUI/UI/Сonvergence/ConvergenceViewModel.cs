using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Graph;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Input;
using WpfUI.Common;
using WpfUI.UI.Graph;

namespace WpfUI.UI.Сonvergence;

public class ConvergenceViewModel : ObservableValidator
{
    private readonly GraphHolder _graphHolder;
    private readonly IMessenger _messenger;

    public ConvergenceViewModel(
        PathfinderRepository pathfinderRepository,
        GraphHolder graphHolder,
        IMessenger messenger)
    {
        _graphHolder = graphHolder;
        ReportingPathfinders = new(pathfinderRepository.GetReportingPathfinders());
        SelectedPathfinder = ReportingPathfinders[0];
        _messenger = messenger;
        RunTestCommand = new AsyncRelayCommand(RunTest);
        RunTestCommand.CanExecuteChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsNotRunning));
        };
        CancelTestCommand = RunTestCommand.CreateCancelCommand();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(TestTimeInSeconds))
        {
            RunTestCommand.NotifyCanExecuteChanged();
        }
    }

    private int _testTimeInSeconds = 10;

    [Range(1, int.MaxValue)]
    public int TestTimeInSeconds
    {
        get => _testTimeInSeconds;
        set => SetProperty(ref _testTimeInSeconds, value, true);
    }

    private int _bestValue = 0;
    public int BestValue
    {
        get => _bestValue;
        set => SetProperty(ref _bestValue, value);
    }

    private int _progressPercent = 0;
    public int ProgressPercent
    {
        get => _progressPercent;
        set => SetProperty(ref _progressPercent, value);
    }

    public bool IsNotRunning => !RunTestCommand.IsRunning;

    public ObservableCollection<ReportingPathfinder> ReportingPathfinders { get; }

    public ReportingPathfinder SelectedPathfinder { get; set; }

    public IAsyncRelayCommand RunTestCommand { get; }
    public ICommand CancelTestCommand { get; }

    private async Task RunTest(CancellationToken cancellationToken)
    {
        ValidateProperty(TestTimeInSeconds, nameof(TestTimeInSeconds));
        var errors = GetErrors(nameof(TestTimeInSeconds));
        if (errors.Any())
        {
            return;
        }
        ProgressPercent = 0;
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
        var resultTask = Task.Run(async () =>
        {
            sw.Start();
            PathResult<int, int> result;
            try
            {
                result = await SelectedPathfinder.Metgod.FindPathWithReportAsync(graph, progress, cts.Token);
            }
            catch (OperationCanceledException)
            {
                result = new([], 0);
            }
            ctsForDelay.Cancel();
            sw.Stop();
            return result;
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
                double currentTime = sw.ElapsedMilliseconds / 1000.0;
                times.Add(currentTime);
                ProgressPercent = (int)(currentTime / TestTimeInSeconds * 100);
                await Task.Delay(period);
            }
        }, cts.Token);
        try
        {
            await Task.Delay(TestTimeInSeconds * 1000, ctsForDelay.Token);
        }
        catch (OperationCanceledException) { }
        values.Add(lastValue);
        times.Add(sw.ElapsedMilliseconds / 1000.0);
        _messenger.Send(report);
        ProgressPercent = 100;
        sw.Stop();
        cts.Cancel();
        if (values.Count > 0)
        {
            BestValue = values[^1];
        }
        var result = await resultTask;
        _messenger.Send(new GraphPathMessage(result));
    }
}
