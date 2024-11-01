using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.Ant;

public sealed class Ant<TNode, TValue>
    where TNode : IEquatable<TNode>
    where TValue : INumber<TValue>
{
    private readonly Graph<TNode, TValue> _graph;
    private readonly Graph<TNode, double> _pheromoneGraph;
    private readonly AntParameters _parameters;
    private readonly Random _random;
    private readonly List<TNode> _path = [];

    public Ant(
        TNode initialPosition,
        Graph<TNode, TValue> graph,
        Graph<TNode, double> pheromoneGraph,
        AntParameters parameters,
        Random random)
    {
        _graph = graph;
        _pheromoneGraph = pheromoneGraph;
        _parameters = parameters;
        _random = random;
        _path.Add(initialPosition);
    }

    public IReadOnlyList<TNode> Path => _path.AsReadOnly();
    public bool Succeeded { get; private set; } = false;

    public void Walk()
    {
        while (_graph[_path[^1]].Count > 0 && _path.Count < _graph.Nodes.Count)
        {
            MakeStep();
        }
        if (_path.Count == _graph.Nodes.Count)
        {
            _path.Add(_path[0]);
            Succeeded = true;
        }
    }

    private void MakeStep()
    {
        TNode currentNode = _path[^1];
        IReadOnlyDictionary<TNode, TValue> availableTransitions = _graph[currentNode]
            .Where(pair => !_path.Contains(pair.Key))
            .ToDictionary();
        List<TNode> nodes = availableTransitions.Keys.ToList();
        List<double> wishes = nodes.Select(node =>
        {
            double t = _pheromoneGraph[currentNode, node];
            double n = 1 / double.CreateChecked(availableTransitions[node]);
            return Math.Pow(t, _parameters.Alpha) * Math.Pow(n, _parameters.Beta);
        }).ToList();
        double totalWish = wishes.Sum();
        List<double> transitionProbabilities = wishes.Select(w => w / totalWish).ToList();
        int chosenNodeIndex = transitionProbabilities.Choose(_random.NextDouble());
        TNode chosenNode = nodes[chosenNodeIndex];
        _path.Add(chosenNode);
    }
}
