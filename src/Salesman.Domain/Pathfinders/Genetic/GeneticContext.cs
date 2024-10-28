using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.Genetic;

public sealed class GeneticContext<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private record Individual(List<TNode> Path, TValue Length);

    private readonly Graph<TNode, TValue> _graph;
    private readonly GeneticParameters _parameters;
    private readonly Random _random;

    public GeneticContext(Graph<TNode, TValue> graph, GeneticParameters parameters, Random random)
    {
        _graph = graph;
        _parameters = parameters;
        _random = random;
    }

    public PathResult<TNode, TValue> FindPath(CancellationToken token = default)
    {
        int populationSize = _graph.Nodes.Count;
        TNode firstNode = _graph.Nodes[0];
        TNode[] nodes = _graph.Nodes.Skip(1).ToArray();
        var population = new List<Individual>(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            _random.Shuffle(nodes);
            List<TNode> individualPath = [firstNode, .. nodes, firstNode];
            population.Add(new(individualPath, _graph.CalculatePathLength(individualPath)));
        }
        Individual best = population.DefaultIfEmpty(new([], TValue.Zero)).MinBy(p => p.Length)!;
        int iterations = 0;
        while (iterations++ <= _parameters.Iterations)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }
            var descendants = Breed(population);
            population.AddRange(descendants);
            population = population.OrderBy(i => i.Length)
                .Take(populationSize)
                .ToList();
            Individual currentBest = population.DefaultIfEmpty(new([], TValue.Zero)).MinBy(p => p.Length)!;
            if (currentBest.Length < best.Length)
            {
                best = currentBest;
                iterations = 0;
            }
        }
        return new PathResult<TNode, TValue>(best.Path, best.Length);
    }

    private IEnumerable<Individual> Breed(List<Individual> population)
    {
        int firstIndex = _random.Next(population.Count);
        int secondIndex = _random.Next(population.Count);
        if (firstIndex == secondIndex)
        {
            secondIndex = population.Count - firstIndex - 1;
        }
        Individual first = population[firstIndex];
        Individual second = population[secondIndex];
        int crossoverPoint = _random.Next(1, first.Path.Count - 1);
        yield return Cross(first, second, crossoverPoint);
        yield return Cross(second, first, crossoverPoint);
    }

    private Individual Cross(Individual father, Individual mother, int crossoverPoint)
    {
        List<TNode> fatherNodes = father.Path
            .Take(crossoverPoint + 1)
            .ToList();
        var nodes = new List<TNode>(father.Path.Count);
        nodes.AddRange(fatherNodes);
        var leftFatherNodes = father.Path.Skip(crossoverPoint + 1).ToList();
        for (int i = crossoverPoint + 1; i < mother.Path.Count - 1; i++)
        {
            TNode chosenNode = nodes.Contains(mother.Path[i]) ?
                leftFatherNodes[0]
                : mother.Path[i];
            nodes.Add(chosenNode);
            leftFatherNodes.Remove(chosenNode);
        }
        nodes.Add(father.Path[^1]);
        double mutationCheck = _random.NextDouble();
        if (mutationCheck <= _parameters.MutationProbability)
        {
            int firstGeneIndex = _random.Next(1, nodes.Count - 1);
            int secondGeneIndex = _random.Next(1, nodes.Count - 1);
            (nodes[firstGeneIndex], nodes[secondGeneIndex]) =
                (nodes[secondGeneIndex], nodes[firstGeneIndex]);
        }
        return new Individual(nodes, _graph.CalculatePathLength(nodes));
    }
}
