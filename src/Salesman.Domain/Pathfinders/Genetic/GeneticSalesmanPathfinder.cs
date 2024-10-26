using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Pathfinders.Genetic;

public class GeneticSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    private readonly Func<GeneticParameters> _parametersFactory;

    public GeneticSalesmanPathfinder(Func<GeneticParameters> parametersFactory)
    {
        _parametersFactory = parametersFactory;
    }

    public Task<PathResult<TNode, TValue>> FindPathAsync(Graph<TNode, TValue> graph, CancellationToken cancellationToken = default)
    {
        if (graph.Nodes.Count <= 1)
        {
            return Task.FromResult(new PathResult<TNode, TValue>([], TValue.Zero));
        }
        GeneticParameters parameters = _parametersFactory();
        var random = new Random();
        var context = new GeneticContext<TNode, TValue>(graph, parameters, random);
        return Task.FromResult(context.FindPath(cancellationToken));
    }
}
