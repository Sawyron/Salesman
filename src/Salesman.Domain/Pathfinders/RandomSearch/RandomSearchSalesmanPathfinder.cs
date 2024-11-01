using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.RandomSearch;

public class RandomSearchSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private readonly Func<RandomSearchParameters> _parametersFactory;

    public RandomSearchSalesmanPathfinder(Func<RandomSearchParameters> parametersFactory)
    {
        _parametersFactory = parametersFactory;
    }

    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        RandomSearchParameters parameters = _parametersFactory();
        var random = new Random();
        TNode first = graph.Nodes[0];
        TNode[] otherNodes = graph.Nodes.Skip(1).ToArray();
        random.Shuffle(otherNodes);
        TNode[] firstPath = [first, .. otherNodes, first];
        var best = new PathResult<TNode, TValue>(firstPath, graph.CalculatePathLength(firstPath));
        for (int i = 0; i < parameters.Iterations; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            random.Shuffle(otherNodes);
            TNode[] currentPath = [first, .. otherNodes, first];
            TValue currentLength = graph.CalculatePathLength(currentPath);
            if (currentLength < best.Length)
            {
                best = new PathResult<TNode, TValue>(currentPath, currentLength);
            }
        }
        return Task.FromResult(best);
    }
}
