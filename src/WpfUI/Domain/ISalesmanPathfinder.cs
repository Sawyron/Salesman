﻿using System.Numerics;

namespace WpfUI.Domain;

public interface ISalesmanPathfinder<N, V>
    where N : notnull
    where V : INumber<V>
{
    public Task<PathResult<N, V>> FindPathAsync(Graph<N, V> graph, CancellationToken cancellationToken = default);
}
