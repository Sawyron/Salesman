using System.Numerics;
using WpfUI.Domain;
using WpfUI.Extensions;

namespace WpfUI.Pathfinders;
public class ExhaustiveSearchSalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public PathResult<N, V> FindPath(Graph<N, V> graph)
    {
        var nodes = graph.Nodes;
        if (nodes.Count == 0)
        {
            return new PathResult<N, V>([], V.Zero);
        }
        var paths = nodes.Skip(1).Permutations()
            .Select(c => c
                .Prepend(nodes[0])
                .Append(nodes[0])
                .ToList())
            .Select(p => (Path: p, Lenght: graph.CalculatePathLength(p)));
        (List<N> path, V length) = paths.MinBy(tuple => tuple.Lenght);
        return new PathResult<N, V>(path, length);
    }
}
