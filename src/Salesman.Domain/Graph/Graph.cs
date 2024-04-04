namespace Salesman.Domain.Graph;

public class Graph<N, V> where N : notnull
{
    private readonly List<N> _nodes;
    private readonly Dictionary<N, IDictionary<N, V>> _adjacency;

    public Graph(IEnumerable<N> nodes, IDictionary<N, IDictionary<N, V>> adjacency, bool directed = false)
    {
        _nodes = nodes.ToList();
        if (_nodes.Count != _nodes.Distinct().Count())
        {
            throw new ArgumentException("All nodes must be unique");
        }
        Dictionary<N, IDictionary<N, V>> connections = nodes
            .ToDictionary(n => n, n => new Dictionary<N, V>() as IDictionary<N, V>);
        foreach ((N key, var _) in connections)
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
            foreach (var (node, edges) in connections)
            {
                foreach ((N adjacentNode, V value) in edges)
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

    public IReadOnlyList<N> Nodes => _nodes.AsReadOnly();

    public IReadOnlyDictionary<N, V> this[N node] =>
        _adjacency.TryGetValue(node, out IDictionary<N, V>? edges) ?
            edges.AsReadOnly() : throw new ArgumentException($"Graph does not contain node '{node}'");

    public V this[N node, N ajancentNode] =>
        _adjacency.TryGetValue(node, out var edjes) ?
            edjes.TryGetValue(ajancentNode, out var value) ?
                value : throw new ArgumentException($"Graph does not contain node '{node}'")
            : throw new ArgumentException($"Graph does not contain value '{node}' and {ajancentNode}");
}
