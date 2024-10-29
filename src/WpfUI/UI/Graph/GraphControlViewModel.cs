using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Graph;
using System.Collections.ObjectModel;
using System.Windows;
using WpfUI.Common;
using WpfUI.UI.InfoPanel;

namespace WpfUI.UI.Graph;

public class GraphControlViewModel : ObservableObject,
    IRecipient<GraphUIState.ChangedMessage>,
    IRecipient<GraphPathMessage>
{
    private readonly GraphHolder _graphHolder;

    public GraphControlViewModel(GraphHolder graphHolder, IMessenger messenger, Store<UIParameters> uiParametersStore)
    {
        _graphHolder = graphHolder;
        _radius = uiParametersStore.Value.Radius;
        _relativeSize = uiParametersStore.Value.GraphRelativeSize;
        messenger.RegisterAll(this);
        OnAreaClickCommand = new RelayCommand<Point>(CreateNode, _ => IsEditEnabled);
        RemoveNodeCommand = new RelayCommand<Node>(RemoveNode, _ => IsEditEnabled);
    }

    public ObservableCollection<Node> Nodes => _graphHolder.Nodes;

    private ObservableCollection<Connection> _connections = [];
    public ObservableCollection<Connection> Connections
    {
        get => _connections;
        set => SetProperty(ref _connections, value);
    }

    private double _relativeSize;
    public double RelativeSize
    {
        get => _relativeSize;
        set => SetProperty(ref _relativeSize, value);
    }

    private double _radius;
    public double Radius
    {
        get => _radius;
        set => SetProperty(ref _radius, value);
    }

    private bool _isEditEnabled = true;

    public bool IsEditEnabled
    {
        get => _isEditEnabled;
        set => SetProperty(ref _isEditEnabled, value);
    }

    public IRelayCommand<Point> OnAreaClickCommand { get; }

    public IRelayCommand<Node> RemoveNodeCommand { get; }

    public void Receive(GraphUIState.ChangedMessage message)
    {
        IsEditEnabled = !message.Value.IsInProgress;
    }

    public void Receive(GraphPathMessage message)
    {
        Connections = [];
        var localConnections = new List<Connection>();
        foreach (var connection in CreateConnectionsFromResult(message.Value))
        {
            localConnections.Add(connection);
        }
        Connections = new ObservableCollection<Connection>(localConnections);
    }

    private void RemoveNode(Node? node)
    {
        if (node is not null)
        {
            _graphHolder.RemoveNode(node);
            Connections = [];
        }
    }

    private void CreateNode(Point point)
    {
        Connections = [];
        _graphHolder.AddNode(point.X - Radius / 2, point.Y - Radius / 2);
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

    private Connection CreateConnectionBetweenNodes(Node first, Node second) =>
        new()
        {
            StartX = first.X + Radius / 2,
            StartY = first.Y + Radius / 2,
            EndX = second.X + Radius / 2,
            EndY = second.Y + Radius / 2,
            FromNodeId = first.Id,
            ToNodeId = second.Id
        };
}
