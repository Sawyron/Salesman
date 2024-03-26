using System.Numerics;
using WpfUI.Domain;
using WpfUI.Extensions;

namespace WpfUI.Pathfinders;
public class ExhaustiveSearchSalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public IEnumerable<N> FindPath(Graph<N, V> graph)
    {
        var nodes = graph.Nodes;
        if (nodes.Count == 0)
        {
            return [];
        }
        var paths = nodes.Skip(1).Permutations(nodes.Count - 1)
            .Select(c => c
                .Prepend(nodes[0])
                .Append(nodes[0])
                .ToList())
            .Select(p => (Path: p, Lenght: CalculatePathLength(p, graph)));
        return paths
            .MinBy(tuple => tuple.Lenght).Path;
    }

    private static V CalculatePathLength(IEnumerable<N> path, Graph<N, V> graph)
    {
        V length = V.Zero;
        using var enumerator = path.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return length;
        }
        N previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            N current = enumerator.Current;
            length += graph[previous][current];
            previous = current;
        }
        return length;
    }
}
