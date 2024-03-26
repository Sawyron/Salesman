using System.Numerics;

namespace WpfUI.Domain;

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
}
