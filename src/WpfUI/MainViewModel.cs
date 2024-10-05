using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Graph;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfUI.Common;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;
using WpfUI.UI.InfoPanel;
using WpfUI.UI.Сonvergence;

namespace WpfUI;

public class MainViewModel : ObservableObject
{
    private readonly GraphHolder _graphHolder;
    private readonly IMessenger _messenger;

    public MainViewModel(
        GraphHolder graphHolder,
        PathfinderRepository pathfinderRepository,
        IMessenger messenger)
    {
        _graphHolder = graphHolder;
        Pathfinders = new ObservableCollection<Pathfinder>(pathfinderRepository.GetAll());
        _selectedPathfinder = Pathfinders[0];
        _messenger = messenger;
        FindPathCommand = new AsyncRelayCommand(FindPath);
        FindPathCommand.CanExecuteChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsNotRunning));
        };
        OnAreaClickCommand = new RelayCommand<Point>(CreateNode, _ => !FindPathCommand.IsRunning);
        RemoveNodeCommand = new RelayCommand<Node>(RemoveNode, _ => !FindPathCommand.IsRunning);
        OpenEdgeSettingsWindowCommand = new RelayCommand(() =>
        {
            var window = new EdgeSettingsWindow();
            window.ShowDialog();
        });
        OpenConvergenceWindowCommand = new RelayCommand(() =>
        {
            var window = new СonvergenceWindow();
            window.ShowDialog();
        });
        ExitCommand = new RelayCommand(() => Environment.Exit(0));
        CancelCommand = FindPathCommand.CreateCancelCommand();
        ClearCommand = new RelayCommand(() =>
        {
            Nodes.Clear();
            Edges.Clear();
            Connections.Clear();
            PathResult = new([], 0);
        });
        _messenger.Register<MainViewModel, GraphUIState.RequestMessage>(this, (r, m) =>
        {
            m.Reply(new GraphUIState(FindPathCommand.IsRunning));
        });
        _messenger.Register<MainViewModel, GraphPathMessage>(this, (r, m) => r.ApplyPathResult(m.Value));
    }

    public ObservableCollection<Node> Nodes => _graphHolder.Nodes;

    public ObservableCollection<Connection> Connections { get; } = [];

    public ObservableCollection<Edge> Edges => _graphHolder.Edges;

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

    private int _nodeRadius = 50;
    public int NodeRadius
    {
        get => _nodeRadius;
        set => SetProperty(ref _nodeRadius, value);
    }

    private Pathfinder _selectedPathfinder;
    public Pathfinder SelectedPathfinder
    {
        get => _selectedPathfinder;
        set => SetProperty(ref _selectedPathfinder, value);
    }

    public bool IsNotRunning => !FindPathCommand.IsRunning;

    public IRelayCommand<Point> OnAreaClickCommand { get; }

    public IRelayCommand<Node> RemoveNodeCommand { get; }

    public IRelayCommand OpenEdgeSettingsWindowCommand { get; }
    public IRelayCommand OpenConvergenceWindowCommand { get; }
    public IRelayCommand ClearCommand { get; }

    public IAsyncRelayCommand FindPathCommand { get; }

    public IRelayCommand ExitCommand { get; }

    public ICommand CancelCommand { get; }

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
        ApplyPathResult(pathResult);
        Time = sw.ElapsedMilliseconds / 1000.0;
        PathResult = pathResult;
        _messenger.Send(new GraphUIState.ChangedMessage(new GraphUIState(false)));
    }

    private void ApplyPathResult(PathResult<int, int> pathResult)
    {
        Connections.Clear();
        foreach (var connection in CreateConnectionsFromResult(pathResult))
        {
            Connections.Add(connection);
        }
        PathResult = pathResult;
    }

    private IEnumerable<Connection> CreateConnectionsFromResult(PathResult<int, int> pathResult)
    {
        var idToNode = _graphHolder.Nodes.ToDictionary(n => n.Id, n => n);
        var nodes = pathResult.Path.Select(id => idToNode[id]);
        using var enumerator = nodes.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return CreateConnectionBetweenNodes(previous, current);
                previous = current;
            }
        }
    }

    private void CreateNode(Point point)
    {
        Connections.Clear();
        int id = Nodes.Select(n => n.Id)
                        .DefaultIfEmpty()
                        .Max() + 1;
        var node = new Node
        {
            Id = id,
            X = point.X - NodeRadius / 2,
            Y = point.Y - NodeRadius / 2,
            Name = $"{id}"
        };
        foreach (var existingNode in Nodes)
        {
            Edges.Add(new Edge
            {
                FromId = existingNode.Id,
                ToId = node.Id,
                Value = Convert.ToInt32(
                    Math.Sqrt(
                        Math.Pow(existingNode.X - node.X, 2)
                        + Math.Pow(existingNode.Y - node.Y, 2)))
            });
        }
        Nodes.Add(node);
    }

    private void RemoveNode(Node? node)
    {
        if (node is not null)
        {
            Connections.Clear();
            Nodes.Remove(node);
            var edgesToRemove = Edges.Where(e => e.FromId == node.Id || e.ToId == node.Id)
                .ToList();
            foreach (var edge in edgesToRemove)
            {
                Edges.Remove(edge);
            }
        }
    }

    private Connection CreateConnectionBetweenNodes(Node first, Node second) =>
        new()
        {
            StartX = first.X + NodeRadius / 2,
            StartY = first.Y + NodeRadius / 2,
            EndX = second.X + NodeRadius / 2,
            EndY = second.Y + NodeRadius / 2,
            FromNodeId = first.Id,
            ToNodeId = second.Id
        };
}
