using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WpfUI.Data;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;

namespace WpfUI.UI.Main;

public class MainViewModel : ObservableObject
{
    private readonly GraphHolder _graphHolder;

    public MainViewModel(GraphHolder graphHolder)
    {
        _graphHolder = graphHolder;
        OnAreaClick = new RelayCommand<Point>(CreateNode);
        RemoveNodeCommand = new RelayCommand<NodeModel>(RemoveNode);
        OpenEdgeSettingsWindowCommand = new RelayCommand(() =>
        {
            var window = new EdgeSettingsWindow();
            window.Show();
        });
        ExitCommand = new RelayCommand(() => Environment.Exit(0));
        Nodes.CollectionChanged += OnNodesChanged;
        Edges.CollectionChanged += OnEdgesChanged;
    }
    private ObservableCollection<NodeModel> _nodes = [];
    public ObservableCollection<NodeModel> Nodes
    {
        get => _nodes;
        set => SetProperty(ref _nodes, value);
    }

    private ObservableCollection<ConnectionModel> _connections = [];
    public ObservableCollection<ConnectionModel> Connections
    {
        get => _connections;
        set => SetProperty(ref _connections, value);
    }

    private ObservableCollection<EdgeModel> _edges = [];
    public ObservableCollection<EdgeModel> Edges
    {
        get => _edges;
        set => SetProperty(ref _edges, value);
    }


    private int _nodeRadius = 50;
    public int NodeRadius
    {
        get => _nodeRadius;
        set => SetProperty(ref _nodeRadius, value);
    }

    public IRelayCommand<Point> OnAreaClick { get; }
    public IRelayCommand<NodeModel> RemoveNodeCommand { get; }
    public IRelayCommand OpenEdgeSettingsWindowCommand { get; }
    public IRelayCommand ExitCommand { get; }

    private void CreateNode(Point point)
    {
        int id = Nodes.Select(n => n.Id)
                        .DefaultIfEmpty()
                        .Max() + 1;
        var node = new NodeModel
        {
            Id = id,
            X = point.X - NodeRadius / 2,
            Y = point.Y - NodeRadius / 2,
            Radius = NodeRadius,
            Name = $"{id}"
        };
        foreach (var existingNode in Nodes)
        {
            Edges.Add(new EdgeModel
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
        if (Nodes.Count > 1)
        {
            var second = Nodes[^1];
            var first = Nodes[^2];
            _connections.Add(CreateConnectionBetweenNodes(second, first));
        }
    }

    private void RemoveNode(NodeModel? node)
    {
        if (node is not null)
        {
            Nodes.Remove(node);
            var edgesToRemove = Edges.Where(e => e.FromId == node.Id || e.ToId == node.Id)
                .ToList();
            foreach (var edge in edgesToRemove)
            {
                Edges.Remove(edge);
            }
        }
    }
    private void OnEdgesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        _graphHolder.Edges.Clear();
        foreach (var edge in Edges)
        {
            _graphHolder.Edges.Add(new Edge
            {
                FromId = edge.FromId,
                ToId = edge.ToId,
                Value = edge.Value
            });
        }
    }

    private void OnNodesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        _graphHolder.Nodes.Clear();
        foreach (var node in Nodes)
        {
            _graphHolder.Nodes.Add(new Node
            {
                Id = node.Id,
                Name = node.Name
            });
        }
    }
    private ConnectionModel CreateConnectionBetweenNodes(NodeModel first, NodeModel second) =>
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
