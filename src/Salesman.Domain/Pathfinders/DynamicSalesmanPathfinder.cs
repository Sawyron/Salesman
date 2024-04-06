using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;
public sealed class DynamicSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    public async Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        var vertexes = graph.Nodes.Skip(1)
            .ToList();
        if (vertexes.Count == 0)
        {
            return new PathResult<TNode, TValue>([], TValue.Zero);
        }
        TNode firstNode = graph.Nodes[0];
        if (vertexes.Count == 1)
        {
            var lengthToSecondNode = graph[firstNode][vertexes[0]];
            return new PathResult<TNode, TValue>(
                [firstNode, vertexes[0], firstNode],
                lengthToSecondNode + lengthToSecondNode);
        }
        var nodes = graph.Nodes.ToArray();
        var nodeIndices = nodes.Select((_, i) => i).ToArray();
        var dp = vertexes.ToDictionary(v => v, v => new Dictionary<HashSet<TNode>, (TNode, TValue)>(HashSet<TNode>.CreateSetComparer()));
        foreach (TNode node in vertexes)
        {
            foreach (TNode otherNode in vertexes)
            {
                if (!node.Equals(otherNode))
                {
                    dp[node][new HashSet<TNode>([otherNode])] = (otherNode, graph[node, otherNode] + graph[otherNode, firstNode]);
                }
            }
        }
        var context = new SolutionContext(graph, dp);
        foreach (int setSize in Enumerable.Range(2, vertexes.Count - 1))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tasks = vertexes
                .Select(v => (v, s: vertexes.Except([v])))
                .Select(tuple => (tuple.v, c: tuple.s.Combinations(setSize).Select(c => c.ToHashSet())))
                .SelectMany(tuple => tuple.c.Select(c => (tuple.v, s: c)))
                .Select(tuple => Task.Run(
                    () => (Node: tuple.v, Set: tuple.s, Value: context.FindSubpath(tuple.v, tuple.s)),
                    cancellationToken));
            var results = await Task.WhenAll(tasks);
            foreach (var (node, set, value) in results)
            {
                dp[node][set] = value;
            }
        }
        var vertexSet = vertexes.ToHashSet();
        var final = context.FindSubpath(firstNode, vertexSet);
        dp[firstNode] = new Dictionary<HashSet<TNode>, (TNode, TValue)>(HashSet<TNode>.CreateSetComparer())
        {
            [vertexSet] = final
        };
        return new PathResult<TNode, TValue>(context.ResolvePath(), final.Length);
    }
    private sealed class SolutionContext
    {
        private readonly Graph<TNode, TValue> _graph;
        private readonly Dictionary<TNode, Dictionary<HashSet<TNode>, (TNode Node, TValue Length)>> _cache;
        public SolutionContext(Graph<TNode, TValue> graph, Dictionary<TNode, Dictionary<HashSet<TNode>, (TNode, TValue)>> cache)
        {
            _graph = graph;
            _cache = cache;
        }

        public (TNode Node, TValue Length) FindSubpath(TNode node, ISet<TNode> nodeSubset) =>
            nodeSubset.Select(n => (SubSet: nodeSubset.Except([n]).ToHashSet(), Excluded: n))
                .Select(step => (
                    Node: step.Excluded,
                    Lenght: _cache[step.Excluded][step.SubSet].Length + _graph[node, step.Excluded]))
                .MinBy(step => step.Lenght);

        public List<TNode> ResolvePath()
        {
            var firstNode = _graph.Nodes[0];
            var path = new List<TNode> { firstNode };
            var remainderSet = _graph.Nodes.Skip(1).ToHashSet();
            var optimal = firstNode;
            var n = remainderSet.Count;
            int count = 0;
            while (remainderSet.Count != 0)
            {
                (optimal, _) = _cache[optimal][remainderSet];
                remainderSet.Remove(optimal);
                path.Add(optimal);
                count++;
                if (count > n)
                {
                    throw new InvalidOperationException("Too many iteration for resolving path");
                }
            }
            path.Add(firstNode);
            return path;
        }
    }
}
