using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WpfUI.UI.Graph;

namespace WpfUI.UI.Main;

public class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        OnAreaClick = new RelayCommand<Point>(CreateNode);
        RemoveNodeCommand = new RelayCommand<NodeModel>(RemoveNode);
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

    private int _nodeRadius = 50;

    public int NodeRadius
    {
        get => _nodeRadius;
        set => SetProperty(ref _nodeRadius, value);
    }

    public IRelayCommand<Point> OnAreaClick { get; }
    public IRelayCommand<NodeModel> RemoveNodeCommand { get; }

    private void CreateNode(Point point)
    {
        int id = Nodes.Select(n => n.Id)
                        .DefaultIfEmpty()
                        .Max() + 1;
        Nodes.Add(new NodeModel
        {
            Id = id,
            X = point.X - NodeRadius / 2,
            Y = point.Y - NodeRadius / 2,
            Radius = NodeRadius,
            Name = $"Node {id}"
        });
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
