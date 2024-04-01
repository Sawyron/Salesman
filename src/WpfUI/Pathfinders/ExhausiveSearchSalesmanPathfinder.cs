using System.Numerics;
using WpfUI.Domain;
using WpfUI.Extensions;

namespace WpfUI.Pathfinders;
public class ExhaustiveSearchSalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public async Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default)
    {
        var nodes = graph.Nodes;
        if (nodes.Count == 0)
        {
            return new PathResult<N, V>([], V.Zero);
        }
        var (path, length) = await Task.Run(() => nodes.Skip(1)
            .Permutations()
            .Select(c => c
                .Prepend(nodes[0])
                .Append(nodes[0])
                .ToList())
            .AsParallel()
            .WithCancellation(cancellationToken)
            .Select(p => (Path: p, Lenght: graph.CalculatePathLength(p)))
            .MinBy(tuple => tuple.Lenght), cancellationToken);
        return new PathResult<N, V>(path, length);
    }
}
