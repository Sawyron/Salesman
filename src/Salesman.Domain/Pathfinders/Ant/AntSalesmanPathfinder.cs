using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.Ant;

public class AntSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : IEquatable<TNode>
    where TValue : INumber<TValue>
{
    private const int IterationsWithoutImprovementsThreshold = 10000;

    private readonly Func<AntParameters> _parametersFactory;

    public AntSalesmanPathfinder(Func<AntParameters> parametersFactory)
    {
        _parametersFactory = parametersFactory;
    }

    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        AntParameters parameters = _parametersFactory();
        Graph<TNode, double> pheromoneGraph = CreatePheromoneGraph(graph, parameters.InitialPheromone);
        var random = new Random();
        int iteration = 0;
        var path = new PathResult<TNode, TValue>(
            [],
            graph.Nodes.SelectMany(
                n => graph[n].Select(pair => pair.Value))
            .Aggregate(TValue.Zero, (acc, v) => acc + v));
        while (iteration++ <= IterationsWithoutImprovementsThreshold)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            List<Ant<TNode, TValue>> ants = graph.Nodes
                .Select(n => new Ant<TNode, TValue>(n, graph, pheromoneGraph, parameters, random))
                .ToList();
            foreach (Ant<TNode, TValue> ant in ants)
            {
                ant.Walk();
            }
            var localPheromone = CreatePheromoneGraph(graph);
            foreach (var ant in ants)
            {
                if (ant.Succeeded)
                {
                    TValue currentLength = graph.CalculatePathLength(ant.Path);
                    if (currentLength < path.Length)
                    {
                        path = new PathResult<TNode, TValue>([.. ant.Path], currentLength);
                        iteration = 0;
                    }
                    for (int i = 0; i < ant.Path.Count - 1; i++)
                    {
                        localPheromone[ant.Path[i], ant.Path[i + 1]] += parameters.Q / double.CreateChecked(currentLength);
                    }
                }
                UpdatePheromoneGlobal(pheromoneGraph, localPheromone, parameters);
            }

        }
        return Task.FromResult(path);
    }

    private static void UpdatePheromoneGlobal(
        Graph<TNode, double> globalPheromoneGraph,
        Graph<TNode, double> localPheromoneGraph,
        AntParameters parameters)
    {
        double evaporationCoefficient = 1 - parameters.P;
        double totalUpdate = localPheromoneGraph.Nodes.
            SelectMany(n => localPheromoneGraph[n].Select(pair => pair.Value))
            .Aggregate(0.0, (accumulator, current) => accumulator + current);
        foreach (TNode node in localPheromoneGraph.Nodes)
        {
            foreach ((TNode otherNode, double pheromone) in localPheromoneGraph[node])
            {
                globalPheromoneGraph[node, otherNode] =
                    evaporationCoefficient * globalPheromoneGraph[node, otherNode] + totalUpdate;
            }
        }
    }

    private static Graph<TNode, double> CreatePheromoneGraph(Graph<TNode, TValue> graph, double initialPheromone = 0)
    {
        var edges = graph.Nodes
            .ToDictionary(n => n, n => graph[n].ToDictionary(pair => pair.Key, _ => initialPheromone) as IDictionary<TNode, double>);
        return new Graph<TNode, double>(graph.Nodes, edges);
    }
}
