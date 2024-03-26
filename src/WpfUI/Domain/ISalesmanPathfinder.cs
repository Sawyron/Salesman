using System.Numerics;

namespace WpfUI.Domain;

public interface ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public PathResult<N, V> FindPath(Graph<N, V> graph);
}
