using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;

public class DummySalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            var nodes = graph.Nodes.ToList();
            if (nodes.Count > 0)
            {
                nodes.Add(nodes[0]);
            }
            return new PathResult<N, V>(nodes, graph.CalculatePathLength(nodes));
        }, cancellationToken);
}
