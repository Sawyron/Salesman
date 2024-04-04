using Salesman.Domain.Graph;
using System.Numerics;

namespace Salesman.Domain.Extensions;

public static class GraphExtensions
{
    public static V CalculatePathLength<N, V>(this Graph<N, V> graph, IEnumerable<N> path)
        where N : notnull
        where V : INumber<V>
    {
        V length = V.Zero;
        using var enumerator = path.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return length;
        }
        N previousNode = enumerator.Current;
        while (enumerator.MoveNext())
        {
            N currentNode = enumerator.Current;
            length += graph[previousNode][currentNode];
            previousNode = currentNode;
        }
        return length;
    }

    public static V[,] ToAdjacencyMatrix<N, V>(this Graph<N, V> graph)
        where N : notnull
        where V : INumber<V>, IMinMaxValue<V>
    {
        var nodes = graph.Nodes;
        V[,] matrix = new V[nodes.Count, nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            var edges = graph[nodes[i]];
            for (int j = 0; j < nodes.Count; j++)
            {
                matrix[i, j] = edges.TryGetValue(nodes[j], out V? value) ?
                    value : V.MaxValue;
            }
        }
        return matrix;
    }
}
