using System.Numerics;

namespace Salesman.Domain.Graph;

public interface IRepotingSalesmanPathfinder<TNode, TValue> : ISalesmanPathfinder<TNode, TValue>
    where TNode : notnull
    where TValue : INumber<TValue>
{
    Task<PathResult<TNode, TValue>> FindPathWithReportAsync(
        Graph<TNode, TValue> graph,
        IProgress<TValue>? progress = null,
        CancellationToken cancellationToken = default);
}
