using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace WpfUI.UI.Graph;
public class GraphViewModel : ObservableObject
{
    public GraphViewModel()
    {
        AddNodeCommand = new RelayCommand<(double X, double Y)>(AddNode);
        RemoveNodeCommand = new RelayCommand<Node>(RemoveNode);
    }

    private ObservableCollection<Node> _nodes = [];
    public ObservableCollection<Node> Nodes
    {
        get => _nodes;
        set => SetProperty(ref _nodes, value);
    }

    private ObservableCollection<Connection> _connections = [];
    public ObservableCollection<Connection> Connections
    {
        get => _connections;
        set => SetProperty(ref _connections, value);
    }


    public IRelayCommand<(double, double)> AddNodeCommand;
    public IRelayCommand<Node> RemoveNodeCommand;

    private void AddNode((double X, double Y) tuple)
    {
        int id = Nodes.Select(n => n.Id)
            .DefaultIfEmpty()
            .Max() + 1;
        Nodes.Add(new Node { Id = id, X = tuple.X, Y = tuple.Y, Name = $"Node {id}" });
        if (Nodes.Count > 1)
        {
            var second = Nodes[^1];
            var first = Nodes[^2];
            _connections.Add(CreateConnectionBetweenNodes(second, first));
        }
    }

    private void RemoveNode(Node? model)
    {
        if (model is null)
        {
            return;
        }
        Nodes.Remove(model);
    }

    private static Connection CreateConnectionBetweenNodes(Node first, Node second) =>
        new()
        {
            StartX = first.X,
            StartY = first.Y,
            EndX = second.X,
            EndY = second.X
        };
}
