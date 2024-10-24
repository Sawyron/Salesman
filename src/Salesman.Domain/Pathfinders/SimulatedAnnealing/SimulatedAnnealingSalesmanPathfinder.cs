using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.SimulatedAnnealing;

public class SimulatedAnnealingSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private readonly Func<SimulatedAnnealingParameters> parametersFactory;
    private const int IterationsThreshold = 10_000_000;

    private readonly Random _random = new();

    public SimulatedAnnealingSalesmanPathfinder(Func<SimulatedAnnealingParameters> parametersFactory)
    {
        this.parametersFactory = parametersFactory;
    }

    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        SimulatedAnnealingParameters parameters = parametersFactory();
        double temperature = parameters.InitialTemperature;
        TNode first = graph.Nodes[0];
        TNode[] state = graph.Nodes.Skip(1).ToArray();
        _random.Shuffle(state);
        var path = new PathResult<TNode, TValue>(
            [first, .. state, first],
            graph.CalculatePathLength([first, .. state, first]));
        for (int i = 0; temperature > parameters.MinTemperature; i++)
        {
            TNode[] candidate = GenerateCandidateState(state);
            TValue currentEnergy = graph.CalculatePathLength([first, .. candidate, first]);
            if (currentEnergy <= path.Length)
            {
                path = new PathResult<TNode, TValue>([first, .. candidate, first], currentEnergy);
            }
            else
            {
                double de = (double.CreateChecked(currentEnergy) - double.CreateChecked(path.Length)) / temperature;
                double transitionProbability = Math.Exp(-de);
                double value = _random.NextDouble();
                if (value <= transitionProbability)
                {
                    path = new PathResult<TNode, TValue>([first, .. candidate, first], currentEnergy);
                }
            }
            temperature = parameters.InitialTemperature * 0.1 / i;
        }
        return Task.FromResult(path);
    }

    private TNode[] GenerateCandidateState(TNode[] state)
    {
        TNode[] candidate = new TNode[state.Length];
        Array.Copy(state, candidate, candidate.Length);
        int start = _random.Next(state.Length);
        int end = _random.Next(state.Length);
        if (start > end)
        {
            (start, end) = (end, start);
        }
        Array.Reverse(candidate, start, end - start + 1);
        return candidate;
    }
}
