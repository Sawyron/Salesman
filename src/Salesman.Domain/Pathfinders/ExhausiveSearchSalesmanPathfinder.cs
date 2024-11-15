﻿using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;

public sealed class ExhaustiveSearchSalesmanPathfinder<TNode, TValue> : IReportingSalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    public Task<PathResult<TNode, TValue>> FindPathAsync(
        Graph<TNode, TValue> graph,
        CancellationToken cancellationToken = default) => FindPathWithReportAsync(graph, null, cancellationToken);

    public async Task<PathResult<TNode, TValue>> FindPathWithReportAsync(Graph<TNode, TValue> graph, IProgress<TValue>? progress = null, CancellationToken cancellationToken = default)
    {
        var nodes = graph.Nodes;
        if (nodes.Count <= 1)
        {
            return new PathResult<TNode, TValue>([], TValue.Zero);
        }
        var (path, length) = await Task.Run(() =>
        {
            var bestSolution = (
                Path: nodes
                    .Append(nodes[0])
                    .ToList(),
                Length: graph.CalculatePathLength(nodes.Append(nodes[0])));
            var variants = nodes.Skip(1)
                        .Permutations()
                        .Select(c => c
                            .Prepend(nodes[0])
                            .Append(nodes[0])
                            .ToList())
                        .Select(p => (Path: p, Lenght: graph.CalculatePathLength(p)));
            foreach (var variant in variants)
            {
                if (variant.Lenght < bestSolution.Length)
                {
                    bestSolution = variant;
                    progress?.Report(variant.Lenght);
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            progress?.Report(bestSolution.Length);
            return bestSolution;
        }, cancellationToken);
        return new PathResult<TNode, TValue>(path, length);
    }
}
