using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders;
public sealed class DynamicSalesmanPathfinder<N, V> : ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public async Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default)
    {
        var vertexes = graph.Nodes.Skip(1)
            .ToList();
        if (vertexes.Count == 0)
        {
            return new PathResult<N, V>([], V.Zero);
        }
        N firstNode = graph.Nodes[0];
        if (vertexes.Count == 1)
        {
            var lengthToSecondNode = graph[firstNode][vertexes[0]];
            return new PathResult<N, V>(
                [firstNode, vertexes[0], firstNode],
                lengthToSecondNode + lengthToSecondNode);
        }
        var nodes = graph.Nodes.ToArray();
        var nodeIndices = nodes.Select((_, i) => i).ToArray();
        var dp = vertexes.ToDictionary(v => v, v => new Dictionary<HashSet<N>, (N, V)>(HashSet<N>.CreateSetComparer()));
        foreach (N node in vertexes)
        {
            foreach (N otherNode in vertexes)
            {
                if (!node.Equals(otherNode))
                {
                    dp[node][new HashSet<N>([otherNode])] = (otherNode, graph[node, otherNode] + graph[otherNode, firstNode]);
                }
            }
        }
        var context = new SolutionContext(graph, dp);
        foreach (int setSize in Enumerable.Range(2, vertexes.Count - 1))
        {
            var tasks = vertexes
                .Select(v => (v, s: vertexes.Except([v])))
                .Select(tuple => (tuple.v, c: tuple.s.Combinations(setSize).Select(c => c.ToHashSet())))
                .SelectMany(tuple => tuple.c.Select(c => (tuple.v, s: c)))
                .Select(tuple => Task.Run(() => (Node: tuple.v, Set: tuple.s, Value: context.FindSubpath(tuple.v, tuple.s))));
            var results = await Task.WhenAll(tasks);
            foreach (var (node, set, value) in results)
            {
                dp[node][set] = value;
            }
        }
        var vertexSet = vertexes.ToHashSet();
        var final = context.FindSubpath(firstNode, vertexSet);
        dp[firstNode] = new Dictionary<HashSet<N>, (N, V)>(HashSet<N>.CreateSetComparer())
        {
            [vertexSet] = final
        };
        return new PathResult<N, V>(context.ResolvePath(), final.Length);
    }
    private sealed class SolutionContext
    {
        private readonly Graph<N, V> _graph;
        private readonly Dictionary<N, Dictionary<HashSet<N>, (N Node, V Length)>> _cache;
        public SolutionContext(Graph<N, V> graph, Dictionary<N, Dictionary<HashSet<N>, (N, V)>> cache)
        {
            _graph = graph;
            _cache = cache;
        }

        public (N Node, V Length) FindSubpath(N node, ISet<N> nodeSubset) =>
            nodeSubset.Select(n => (SubSet: nodeSubset.Except([n]).ToHashSet(), Excluded: n))
                .Select(step => (
                    Node: step.Excluded,
                    Lenght: _cache[step.Excluded][step.SubSet].Length + _graph[node, step.Excluded]))
                .MinBy(step => step.Lenght);

        public List<N> ResolvePath()
        {
            var firstNode = _graph.Nodes[0];
            var path = new List<N> { firstNode };
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
