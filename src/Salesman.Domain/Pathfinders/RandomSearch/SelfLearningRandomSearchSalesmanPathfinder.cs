using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.RandomSearch;

public class SelfLearningRandomSearchSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private const double Alpha = 0.05;

    private readonly Func<RandomSearchParameters> _parametersFactory;

    public SelfLearningRandomSearchSalesmanPathfinder(Func<RandomSearchParameters> parametersFactory)
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
        var context = new SolutionContext(parameters, graph, new Random());
        return Task.FromResult(context.Solve(cancellationToken));
    }

    private sealed class SolutionContext
    {
        private readonly RandomSearchParameters _parameters;
        private readonly Graph<TNode, TValue> _graph;
        private readonly Random _random;
        private readonly List<Action<TNode[]>> _operations;
        private readonly List<double> _probabilities;

        public SolutionContext(
            RandomSearchParameters parameters,
            Graph<TNode, TValue> graph,
            Random random)
        {
            _parameters = parameters;
            _graph = graph;
            _random = random;
            _operations = [Shuffle, Reverse, Swap, Insert];
            _probabilities = _operations.Select(_ => 1.0 / _operations.Count).ToList();
        }

        public PathResult<TNode, TValue> Solve(CancellationToken token)
        {
            TNode first = _graph.Nodes[0];
            TNode[] otherNodes = _graph.Nodes.Skip(1).ToArray();
            _random.Shuffle(otherNodes);
            var path = new PathResult<TNode, TValue>(
                [first, .. otherNodes, first],
                _graph.CalculatePathLength([first, .. otherNodes, first]));
            for (int i = 0; i < _parameters.Iterations; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                int operationIndex = _probabilities.Choose(_random.NextDouble());
                Action<TNode[]> operation = _operations[operationIndex];
                operation(otherNodes);
                TValue currentLength = _graph.CalculatePathLength([first, .. otherNodes, first]);
                if (currentLength < path.Length)
                {
                    path = new PathResult<TNode, TValue>([first, .. otherNodes, first], currentLength);
                    IncreaseOperationProbability(operationIndex);
                }
                else
                {
                    DecreaseOperationIndex(operationIndex);
                }
            }
            return path;
        }

        private void IncreaseOperationProbability(int operationIndex)
        {
            _probabilities[operationIndex] += Alpha;
            double decrease = Alpha / (_probabilities.Count - 1);
            for (int j = 0; j < _probabilities.Count; j++)
            {
                if (j != operationIndex)
                {
                    _probabilities[j] -= decrease;
                }
            }
        }

        private void DecreaseOperationIndex(int operationIndex)
        {
            _probabilities[operationIndex] -= Alpha;
            double increase = Alpha / (_probabilities.Count - 1);
            for (int j = 0; j < _probabilities.Count; j++)
            {
                _probabilities[j] += increase;
            }
        }

        private void Shuffle(TNode[] nodes)
        {
            _random.Shuffle(nodes);
        }

        private void Reverse(TNode[] nodes)
        {
            int start = _random.Next(nodes.Length);
            int end = _random.Next(nodes.Length);
            if (start > end)
            {
                (start, end) = (end, start);
            }
            Array.Reverse(nodes, start, end - start + 1);
        }

        private void Swap(TNode[] nodes)
        {
            int i = _random.Next(nodes.Length);
            int j = _random.Next(nodes.Length);
            (nodes[i], nodes[j]) = (nodes[j], nodes[i]);
        }

        private void Insert(TNode[] nodes)
        {
            int elementIndex = _random.Next(nodes.Length);
            int afterIndex = _random.Next(nodes.Length);
            if (elementIndex == afterIndex)
            {
                return;
            }
            TNode element = nodes[elementIndex];
            TNode after = nodes[afterIndex];
            if (elementIndex > afterIndex)
            {
                for (int i = elementIndex; i > afterIndex; i--)
                {
                    nodes[i] = nodes[i - 1];
                }
                nodes[afterIndex + 1] = element;
            }
            else
            {
                for (int i = elementIndex; i < afterIndex - 1; i++)
                {
                    nodes[i] = nodes[i + 1];
                }
                nodes[elementIndex] = after;
            }
        }
    }
}
