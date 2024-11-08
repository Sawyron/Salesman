using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;

public sealed class BranchAndBoundSalesmanPathfinder<TNode, TValue> : IReportingSalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>, IMinMaxValue<TValue>
{
    public Task<PathResult<TNode, TValue>> FindPathAsync(
        Graph<TNode, TValue> graph,
        CancellationToken cancellationToken = default) => FindPathWithReportAsync(graph, null, cancellationToken);

    public async Task<PathResult<TNode, TValue>> FindPathWithReportAsync(
        Graph<TNode, TValue> graph,
        IProgress<TValue>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return new PathResult<TNode, TValue>([], TValue.Zero);
        }
        var notBranchedNodes = new PriorityQueue<SolutionNode, TValue>();
        SolutionNode node = new(graph.Nodes[0], TValue.Zero, [graph.Nodes[0]]);
        while (node.Visited.Count != graph.Nodes.Count)
        {
            progress?.Report(
                (await FindBestNode(
                    notBranchedNodes.UnorderedItems
                        .Select(i => i.Element)
                        .Append(node),
                    graph)).Bound);
            if (cancellationToken.IsCancellationRequested)
            {
                node = await FindBestNode(
                    notBranchedNodes.UnorderedItems
                        .Select(i => i.Element)
                        .Append(node),
                    graph);
                break;
            }
            var branches = await node.Branch(graph);
            notBranchedNodes.EnqueueRange(branches.Select(b => (b, b.Bound)));
            if (!notBranchedNodes.TryDequeue(out var nextNode, out _))
            {
                break;
            }
            node = nextNode;
            progress?.Report(
                (await FindBestNode(
                    notBranchedNodes.UnorderedItems
                        .Select(i => i.Element)
                        .Append(node),
                    graph)).Bound);
        }
        progress?.Report(node.Bound);
        return new PathResult<TNode, TValue>([.. node.Visited, graph.Nodes[0]], node.Bound);
    }

    private static async Task<SolutionNode> FindBestNode(IEnumerable<SolutionNode> nodes, Graph<TNode, TValue> graph)
    {
        var tasks = nodes.Select(n => Task.Run(() => n.GetGreedyNode(graph)));
        var solutions = await Task.WhenAll(tasks);
        return solutions.MinBy(s => s.Bound) ??
            new SolutionNode(graph.Nodes[0], TValue.Zero, [graph.Nodes[0]]).GetGreedyNode(graph);
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

        public async Task<SolutionNode[]> Branch(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
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

        public SolutionNode GetGreedyNode(Graph<TNode, TValue> graph)
        {
            var path = Visited.ToList();
            TNode currentNode = path[^1];
            while (path.Count != graph.Nodes.Count)
            {
                var (node, lenght) = graph[currentNode].Where(pair => !path.Contains(pair.Key))
                    .MinBy(pair => pair.Value);
                currentNode = node;
                path.Add(node);
            }
            path.Add(graph.Nodes[0]);
            return new SolutionNode(Node, graph.CalculatePathLength(path), path);
        }

        private TValue GetLowerBound(Graph<TNode, TValue> graph, TNode nextNode)
        {
            var visitedValues = GetVisitedEdges().Append((From: Visited[Visited.Count - 1], To: nextNode))
                .Select(e => graph[e.From, e.To]);
            var notVisitedRows = new List<TValue>();
            foreach (var node in graph.Nodes.Except(Visited))
            {
                var rowValue = graph[node]
                    .Where(pair =>
                        !Visited.Contains(pair.Key) &&
                        !nextNode.Equals(pair.Key) &&
                        !node.Equals(pair.Key))
                    .Select(pair => pair.Value)
                    .DefaultIfEmpty(graph[node, graph.Nodes[0]])
                    .Min()!;
                notVisitedRows.Add(rowValue);
            }
            return notVisitedRows.Concat(visitedValues)
                    .Aggregate(
                        TValue.Zero,
                        (acc, val) => TValue.MaxValue - acc < val ? TValue.MaxValue : acc + val);
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
