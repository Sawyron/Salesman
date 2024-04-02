using System.Numerics;

namespace Salesman.Domain.Graph;
public record PathResult<N, V>(IReadOnlyList<N> Path, V Length)
    where N : notnull
    where V : INumber<V>;
