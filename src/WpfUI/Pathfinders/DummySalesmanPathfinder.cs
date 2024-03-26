using System.Numerics;
using WpfUI.Domain;

namespace WpfUI.Pathfinders;

public class DummySalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public PathResult<N, V> FindPath(Graph<N, V> graph)
    {
        var resultNodes = new List<N>();
        var nodes = graph.Nodes;
        foreach (var node in nodes)
        {
            resultNodes.Add(node);
        }
        if (nodes.Count > 0)
        {
            resultNodes.Add(nodes[0]);
        }
        return new PathResult<N, V>(nodes, V.Zero);
    }
}
