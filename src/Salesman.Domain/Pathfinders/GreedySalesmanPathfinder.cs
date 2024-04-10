using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;

public sealed class GreedySalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        var nodes = graph.Nodes.ToList();
        if (nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        var path = new List<TNode>(nodes.Count) { nodes[0] };
        var currentNode = nodes[0];
        var pathLength = TValue.Zero;
        while (path.Count != nodes.Count)
        {
            var (node, lenght) = graph[currentNode].Where(pair => !path.Contains(pair.Key))
                .MinBy(pair => pair.Value);
            pathLength += lenght;
            currentNode = node;
            path.Add(node);
        }
        pathLength += graph[currentNode, nodes[0]];
        path.Add(nodes[0]);
        return Task.FromResult(new PathResult<TNode, TValue>(path, pathLength));
    }
}
