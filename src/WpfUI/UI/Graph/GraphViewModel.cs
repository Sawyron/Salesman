using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace WpfUI.UI.Graph;
public class GraphViewModel : ObservableObject
{
    public GraphViewModel()
    {
        AddNodeCommand = new RelayCommand<(double X, double Y)>(AddNode);
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


    public IRelayCommand<(double, double)> AddNodeCommand;
    public IRelayCommand<NodeModel> RemoveNodeCommand;

    private void AddNode((double X, double Y) tuple)
    {
        int id = Nodes.Select(n => n.Id)
            .DefaultIfEmpty()
            .Max() + 1;
        Nodes.Add(new NodeModel { Id = id, X = tuple.X, Y = tuple.Y, Name = $"Node {id}" });
        if (Nodes.Count > 1)
        {
            var second = Nodes[^1];
            var first = Nodes[^2];
            _connections.Add(CreateConnectionBetweenNodes(second, first));
        }
    }

    private void RemoveNode(NodeModel? model)
    {
        if (model is null)
        {
            return;
        }
        Nodes.Remove(model);
    }

    private static ConnectionModel CreateConnectionBetweenNodes(NodeModel first, NodeModel second) =>
        new()
        {
            StartX = first.X,
            StartY = first.Y,
            EndX = second.X,
            EndY = second.X
        };
}
