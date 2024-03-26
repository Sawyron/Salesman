using System.Numerics;
using WpfUI.Domain;

namespace WpfUI.Pathfinders;

public class DummySalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public PathResult<N, V> FindPath(Graph<N, V> graph)
    {
        var nodes = graph.Nodes.ToList();
        if (nodes.Count > 0)
        {
            nodes.Add(nodes[0]);
        }
        return new PathResult<N, V>(nodes, graph.CalculatePathLength(nodes));
    }
}
