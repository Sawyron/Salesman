using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;
public class GreedySalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default)
    {
        var nodes = graph.Nodes.ToList();
        if (nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<N, V>([], V.Zero));
        }
        var path = new List<N>(nodes.Count) { nodes[0] };
        var currentNode = nodes[0];
        var lenght = V.Zero;
        while (path.Count != nodes.Count)
        {
            var (Node, Lenght) = nodes.Where(n => !path.Contains(n))
                .Select(n => (Node: n, Lenght: graph[currentNode][n]))
                .MinBy(tuple => tuple.Lenght);
            lenght += Lenght;
            currentNode = Node;
            path.Add(currentNode);
        }
        lenght += graph[currentNode][nodes[0]];
        path.Add(nodes[0]);
        return Task.FromResult(new PathResult<N, V>(path, lenght));
    }
}
