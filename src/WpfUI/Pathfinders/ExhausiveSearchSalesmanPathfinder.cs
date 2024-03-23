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
        var c = nodes.Permutations(nodes.Count)
            .ToList();
        var paths = nodes.Skip(1).Permutations(nodes.Count - 1)
            .Select(c => c
                .Prepend(nodes[0])
                .Append(nodes[0])
                .ToList())
            .ToList()
            .Select(p => (Path: p, Lenght: CalculatePathLength(p, graph)))
            .ToList();
        return paths
            .MinBy(tuple => tuple.Lenght).Path;
    }

    private static V CalculatePathLength(List<N> path, Graph<N, V> graph)
    {
        V length = V.Zero;
        foreach (var (node, i) in path[..^1].Select((n, i) => (n, i)))
        {
            length += graph[node][path[i + 1]];
        }
        return length;
    }
}
