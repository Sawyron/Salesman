using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using WpfUI.Data;
using WpfUI.Domain;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;

namespace WpfUI.UI.Main;

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
        _messenger.Register<MainViewModel, GraphUIState.RequestMessage>(this, (r, m) =>
        {
            m.Reply(new GraphUIState(IsInProgress));
        });
        OnAreaClickCommand = new RelayCommand<Point>(CreateNode, _ => !IsInProgress);
        RemoveNodeCommand = new RelayCommand<Node>(RemoveNode, _ => !IsInProgress);
        OpenEdgeSettingsWindowCommand = new RelayCommand(() =>
        {
            var window = new EdgeSettingsWindow();
            window.ShowDialog();
        });
        FindPathCommand = new AsyncRelayCommand(FindPath, () => !IsInProgress);
        ExitCommand = new RelayCommand(() => Environment.Exit(0));
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

    private bool _isInProgress;
    public bool IsInProgress
    {
        get => _isInProgress;
        set => SetProperty(ref _isInProgress, value);
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

    public IRelayCommand<Point> OnAreaClickCommand { get; }
    public IRelayCommand<Node> RemoveNodeCommand { get; }
    public IRelayCommand OpenEdgeSettingsWindowCommand { get; }
    public IAsyncRelayCommand FindPathCommand { get; }
    public IRelayCommand ExitCommand { get; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(IsInProgress))
        {
            OpenEdgeSettingsWindowCommand.NotifyCanExecuteChanged();
            OnAreaClickCommand.NotifyCanExecuteChanged();
            RemoveNodeCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task FindPath()
    {
        IsInProgress = true;
        PathResult = new([], 0);
        Time = 0.0;
        _messenger.Send(new GraphUIState.ChangedMessage(new GraphUIState(true)));
        var graph = _graphHolder.CrateGraph();
        var sw = new Stopwatch();
        sw.Start();
        var pathResult = await Task.Run(() => SelectedPathfinder.Method.FindPath(graph));
        var (pathIds, length) = pathResult;
        sw.Stop();
        var idToNode = _graphHolder.Nodes.ToDictionary(n => n.Id, n => n);
        var nodes = pathIds.Select(id => idToNode[id]);
        using var enumerator = nodes.GetEnumerator();
        Connections.Clear();
        if (enumerator.MoveNext())
        {
            var previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                Connections.Add(CreateConnectionBetweenNodes(previous, current));
                previous = current;
            }
        }
        Time = sw.ElapsedMilliseconds / 1000.0;
        IsInProgress = false;
        PathResult = pathResult;
        _messenger.Send(new GraphUIState.ChangedMessage(new GraphUIState(false)));
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
