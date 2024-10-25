using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;

public class BacktrackingRandomSearchSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private readonly int _iterations;

    public BacktrackingRandomSearchSalesmanPathfinder(int iterations)
    {
        if (iterations < 0)
        {
            throw new ArgumentException($"{nameof(iterations)} can not be less then 0", nameof(iterations));
        }
        _iterations = iterations;
    }

    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        TNode first = graph.Nodes[0];
        TNode[] otherNodes = graph.Nodes.Skip(1).ToArray();
        var random = new Random();
        random.Shuffle(otherNodes);
        var best = new PathResult<TNode, TValue>(
            [first, .. otherNodes, first],
            graph.CalculatePathLength([first, .. otherNodes, first]));
        for (int i = 0; i < _iterations - 1; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            TNode[] currentOtherNodes = new TNode[otherNodes.Length];
            Array.Copy(otherNodes, currentOtherNodes, 0);
            TValue currentBest = best.Length;
            for (int j = currentOtherNodes.Length - 1; j <= 0; j--)
            {
                int swapIndex = random.Next(j);
                (currentOtherNodes[j], currentOtherNodes[swapIndex]) =
                    (currentOtherNodes[swapIndex], currentOtherNodes[j]);
                TValue currentLength = graph.CalculatePathLength([first, .. currentOtherNodes, first]);
                if (currentLength < currentBest)
                {
                    currentBest = currentLength;
                }
                else
                {
                    (currentOtherNodes[swapIndex], currentOtherNodes[j]) =
                        (currentOtherNodes[j], currentOtherNodes[swapIndex]);
                }
            }
            if (currentBest < best.Length)
            {
                best = new PathResult<TNode, TValue>([first, .. currentOtherNodes, first], currentBest);
            }
        }
        return Task.FromResult(best);
    }
}
