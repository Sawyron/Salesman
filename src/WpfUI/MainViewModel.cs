using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Graph;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Input;
using WpfUI.Common;
using WpfUI.UI;
using WpfUI.UI.Graph;
using WpfUI.UI.InfoPanel;

namespace WpfUI;

public class MainViewModel : ObservableValidator
{
    private readonly GraphHolder _graphHolder;
    private readonly Store<UIParameters> _uiStore;
    private readonly IMessenger _messenger;

    public MainViewModel(
        GraphHolder graphHolder,
        PathfinderRepository pathfinderRepository,
        IMessenger messenger,
        Store<UIParameters> uiStore)
    {
        _graphHolder = graphHolder;
        _messenger = messenger;
        Pathfinders = new ObservableCollection<Pathfinder>(pathfinderRepository.GetAll());
        _selectedPathfinder = Pathfinders[0];
        FindPathCommand = new AsyncRelayCommand(FindPath);
        FindPathCommand.CanExecuteChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsNotRunning));
        };
        CancelCommand = FindPathCommand.CreateCancelCommand();
        ClearCommand = new RelayCommand(Clear);
        GenerateNodesCommand = new RelayCommand(GenerateNodes);
        _messenger.Register<MainViewModel, GraphUIState.RequestMessage>(this, (r, m) =>
        {
            m.Reply(new GraphUIState(FindPathCommand.IsRunning));
        });
        _uiStore = uiStore;
    }

    public ObservableCollection<Pathfinder> Pathfinders { get; }

    private double _time;
    public double Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    private PathResult<int, int> _pathResult = new([], 0);
    public PathResult<int, int> PathResult
    {
        get => _pathResult;
        set => SetProperty(ref _pathResult, value);
    }

    private Pathfinder _selectedPathfinder;
    public Pathfinder SelectedPathfinder
    {
        get => _selectedPathfinder;
        set => SetProperty(ref _selectedPathfinder, value);
    }

    private int _nodesToGenerate = 10;

    [Range(0, 500)]
    public int NodesToGenerate
    {
        get => _nodesToGenerate;
        set => SetProperty(ref _nodesToGenerate, value, true);
    }

    public bool IsNotRunning => !FindPathCommand.IsRunning;

    public IRelayCommand ClearCommand { get; }

    public IAsyncRelayCommand FindPathCommand { get; }

    public ICommand CancelCommand { get; }
    public IRelayCommand GenerateNodesCommand { get; }

    private async Task FindPath(CancellationToken cancellationToken)
    {
        PathResult = new([], 0);
        Time = 0.0;
        _messenger.Send(new GraphUIState.ChangedMessage(new GraphUIState(true)));
        var graph = _graphHolder.CrateGraph();
        var sw = new Stopwatch();
        sw.Start();
        PathResult<int, int> pathResult;
        try
        {
            pathResult = await Task.Run(async () =>
                await SelectedPathfinder.Method.FindPathAsync(graph, cancellationToken), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            pathResult = new([], 0);
        }
        sw.Stop();
        Time = sw.ElapsedMilliseconds / 1000.0;
        PathResult = pathResult;
        _messenger.Send(new GraphPathMessage(pathResult));
        _messenger.Send(new GraphUIState.ChangedMessage(new GraphUIState(false)));
    }

    private void Clear()
    {
        PathResult = new([], 0);
        _graphHolder.Clear();
        _messenger.Send(new GraphPathMessage(PathResult));
    }

    private void GenerateNodes()
    {
        Clear();
        var random = new Random();
        UIParameters parameters = _uiStore.Value;
        double distanceLimit = parameters.GraphRelativeSize - parameters.Radius;
        var coordinates = Enumerable.Range(0, _nodesToGenerate)
            .Select(_ => (random.NextDouble() * distanceLimit, random.NextDouble() * distanceLimit));
        _graphHolder.AddNodeRange(coordinates);
    }
}
