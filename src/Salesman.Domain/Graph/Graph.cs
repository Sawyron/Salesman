namespace Salesman.Domain.Graph;

public class Graph<TNode, TValue> where TNode : notnull
{
    private readonly List<TNode> _nodes;
    private readonly Dictionary<TNode, IDictionary<TNode, TValue>> _adjacency;

    public Graph(IEnumerable<TNode> nodes, IDictionary<TNode, IDictionary<TNode, TValue>> adjacency, bool directed = false)
    {
        _nodes = nodes.ToList();
        if (_nodes.Count != _nodes.Distinct().Count())
        {
            throw new ArgumentException("All nodes must be unique");
        }
        var connections = nodes.ToDictionary(
            node => node,
            _ => new Dictionary<TNode, TValue>() as IDictionary<TNode, TValue>);
        foreach ((TNode key, var _) in connections)
        {
            if (!_nodes.Contains(key))
            {
                throw new ArgumentException($"Provided '{nameof(nodes)}' does not contain '{key}'");
            }
        }
        foreach (var (key, edges) in adjacency)
        {
            connections[key] = edges.ToDictionary();
        }
        if (!directed)
        {
            foreach ((TNode node, IDictionary<TNode, TValue> edges) in connections)
            {
                foreach ((TNode adjacentNode, TValue value) in edges)
                {
                    if (!connections[adjacentNode].ContainsKey(node))
                    {
                        connections[adjacentNode][node] = value;
                    }
                }
            }
        }
        _adjacency = connections;
    }

    public IReadOnlyList<TNode> Nodes => _nodes;

    public IReadOnlyDictionary<TNode, TValue> this[TNode node] =>
        _adjacency.TryGetValue(node, out IDictionary<TNode, TValue>? edges) ?
            edges.AsReadOnly() : throw new ArgumentException($"Graph does not contain node '{node}'");

    public TValue this[TNode node, TNode ajancentNode]
    {
        get
        {
            if (_adjacency.TryGetValue(node, out var edges) && edges.TryGetValue(ajancentNode, out var value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"There is no connection between '{node}' and '{ajancentNode}'");
            }
        }
    }
}
