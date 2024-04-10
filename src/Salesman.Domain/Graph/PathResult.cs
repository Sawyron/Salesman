using System.Numerics;

namespace Salesman.Domain.Graph;

public record PathResult<TNode, TValue>(IReadOnlyList<TNode> Path, TValue Length)
    where TNode : notnull
    where TValue : INumber<TValue>;
