using System.Numerics;
using WpfUI.Domain;

namespace WpfUI.Pathfinders;

public class DummySalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public IEnumerable<N> FindPath(Graph<N, V> graph)
    {
        var nodes = graph.Nodes;
        foreach (var node in nodes)
        {
            yield return node;
        }
        if (nodes.Count > 0)
        {
            yield return nodes[0];
        }
    }
}
