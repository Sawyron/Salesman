using System.Numerics;

namespace WpfUI.Domain;
public record PathResult<N, V>(IReadOnlyList<N> Path, V Length)
    where N : notnull
    where V : INumber<V>;
