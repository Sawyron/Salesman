using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WpfUI.Data;
using WpfUI.Domain;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;

namespace WpfUI.UI.Main;

public class MainViewModel : ObservableObject
{
    private readonly GraphHolder _graphHolder;
    private readonly ISalesmanPathfinder<int, int> _pathfinder;

    public MainViewModel(GraphHolder graphHolder, ISalesmanPathfinder<int, int> pathfinder)
    {
        _graphHolder = graphHolder;
        _pathfinder = pathfinder;
        OnAreaClickCommand = new RelayCommand<Point>(CreateNode);
        RemoveNodeCommand = new RelayCommand<Node>(RemoveNode);
        OpenEdgeSettingsWindowCommand = new RelayCommand(() =>
        {
            var window = new EdgeSettingsWindow();
            window.Show();
        });
        FindPathCommand = new AsyncRelayCommand(async () =>
        {
            var graph = _graphHolder.CrateGraph();
            var pathIds = await Task.Run(() => _pathfinder.FindPath(graph));
            var idToNode = _graphHolder.Nodes.ToDictionary(n => n.Id, n => n);
            var nodes = pathIds.Select(id => idToNode[id]);
            using var enumerator = nodes.GetEnumerator();
            enumerator.MoveNext();
            var previous = enumerator.Current;
            Connections.Clear();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                Connections.Add(CreateConnectionBetweenNodes(previous, current));
                previous = current;
            }
        });
        ExitCommand = new RelayCommand(() => Environment.Exit(0));
    }

    public ObservableCollection<Node> Nodes => _graphHolder.Nodes;

    public ObservableCollection<Connection> Connections { get; } = [];

    public ObservableCollection<Edge> Edges => _graphHolder.Edges;


    private int _nodeRadius = 50;
    public int NodeRadius
    {
        get => _nodeRadius;
        set => SetProperty(ref _nodeRadius, value);
    }

    public IRelayCommand<Point> OnAreaClickCommand { get; }
    public IRelayCommand<Node> RemoveNodeCommand { get; }
    public IRelayCommand OpenEdgeSettingsWindowCommand { get; }
    public IAsyncRelayCommand FindPathCommand { get; }
    public IRelayCommand ExitCommand { get; }

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
