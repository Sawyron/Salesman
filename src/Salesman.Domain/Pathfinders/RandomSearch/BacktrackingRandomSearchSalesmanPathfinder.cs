﻿using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.RandomSearch;

public class BacktrackingRandomSearchSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private readonly Func<RandomSearchParameters> _parametersFactory;

    public BacktrackingRandomSearchSalesmanPathfinder(Func<RandomSearchParameters> parametersFactory)
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
        TNode first = graph.Nodes[0];
        TNode[] otherNodes = graph.Nodes.Skip(1).ToArray();
        var random = new Random();
        random.Shuffle(otherNodes);
        var best = new PathResult<TNode, TValue>(
            [first, .. otherNodes, first],
            graph.CalculatePathLength([first, .. otherNodes, first]));
        for (int i = 0; i < parameters.Iterations - 1; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            TNode[] currentOtherNodes = new TNode[otherNodes.Length];
            Array.Copy(otherNodes, currentOtherNodes, otherNodes.Length);
            TValue currentBest = best.Length;
            for (int j = currentOtherNodes.Length - 1; j > 0; j--)
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
                Array.Copy(currentOtherNodes, otherNodes, currentOtherNodes.Length);
            }
        }
        return Task.FromResult(best);
    }
}
