﻿using Salesman.Domain.Graph;
using System.Numerics;
using System.Xml.Linq;

namespace Salesman.Domain.Pathfinders;
public sealed class BranchAndBoundSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    public async Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return new PathResult<TNode, TValue>([], TValue.Zero);
        }
        var notBranchedNodes = new List<SolutionNode>();
        SolutionNode node = new(graph.Nodes[0], TValue.Zero, [graph.Nodes[0]]);
        while (node.Visited.Count != graph.Nodes.Count)
        {
            notBranchedNodes.AddRange(await node.Branch(graph, cancellationToken));
            notBranchedNodes.Remove(node);
            var nextNode = notBranchedNodes.MinBy(n => n.Bound);
            if (nextNode is null)
            {
                break;
            }
            node = nextNode;
        }
        return new PathResult<TNode, TValue>([.. node.Visited, graph.Nodes[0]], node.Bound);
    }

    private sealed class SolutionNode
    {
        public SolutionNode(TNode node, TValue bound, IEnumerable<TNode> visited)
        {
            Node = node;
            Bound = bound;
            Visited = visited.ToList();
        }

        public TNode Node { get; }

        public TValue Bound { get; }

        public IReadOnlyList<TNode> Visited { get; }

        public async Task<IEnumerable<SolutionNode>> Branch(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
        {
            var tasks = graph[Node].Where(pair => !Visited.Contains(pair.Key))
                .Select(pair => Task.Run(
                    () => new SolutionNode(
                        pair.Key,
                        GetLowerBound(graph, pair.Key),
                        Visited.Append(pair.Key)),
                    cancellationToken));
            return await Task.WhenAll(tasks);
        }

        private TValue GetLowerBound(Graph<TNode, TValue> graph, TNode nextNode)
        {
            var visitedValues = GetVisitedEdges().Append((From: Visited[Visited.Count - 1], To: nextNode))
                .Select(e => graph[e.From, e.To]);
            var notVisitedRows = new List<TValue>();
            foreach (var node in graph.Nodes.Except(Visited))
            {
                var rowValue = graph[node]
                    .Where(pair => !Visited.Contains(pair.Key))
                    .Where(pair => !nextNode.Equals(pair.Key))
                    .Select(pair => pair.Value)
                    .DefaultIfEmpty(graph[node, graph.Nodes[0]])
                    .Min()!;
                notVisitedRows.Add(rowValue);
            }
            return notVisitedRows.Concat(visitedValues)
                    .Aggregate(TValue.Zero, (acc, val) => acc + val);
        }

        private IEnumerable<(TNode From, TNode To)> GetVisitedEdges()
        {
            using var enumerator = Visited.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                yield break;
            }
            var node = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var nextNode = enumerator.Current;
                yield return (node, nextNode);
                node = nextNode;
            }
        }
    }
}
