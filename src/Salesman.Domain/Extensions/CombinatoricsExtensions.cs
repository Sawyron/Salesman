namespace Salesman.Domain.Extensions;
public static class CombinatoricsExtensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source)
    {
        var values = source.ToArray();
        return values.Combinations(values.Length);
    }

    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int size) =>
        source.ToArray().Combinations(size);

    public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source)
    {
        var values = source.ToArray();
        return values.Permutations(values.Length);
    }

    public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source, int size) =>
        source.ToArray().Permutations(size);

    private static IEnumerable<IEnumerable<T>> Combinations<T>(this T[] values, int size)
    {
        int n = values.Length;
        if (size > n)
        {
            yield break;
        }
        int[] indices = Enumerable.Range(0, size).ToArray();
        yield return indices.Select(i => values[i]);
        bool completed = false;
        while (true)
        {
            int i;
            for (i = size - 1; i >= 0; i--)
            {
                if (indices[i] != i + n - size)
                {
                    break;
                }
                if (i == 0)
                {
                    completed = true;
                }
            }
            if (completed)
            {
                yield break;
            }
            indices[i] += 1;
            for (int j = i + 1; j < size; j++)
            {
                indices[j] = indices[j - 1] + 1;
            }
            yield return indices.Select(i => values[i]);
        }
    }

    private static IEnumerable<IEnumerable<T>> Permutations<T>(this T[] values, int size)
    {
        int n = values.Length;
        var indices = new int[n];
        if (size > n)
        {
            yield break;
        }
        yield return values.Take(size);
        int i = 0;
        while (i < n)
        {
            if (indices[i] < i)
            {
                if (i % 2 == 0)
                {
                    (values[0], values[i]) = (values[i], values[0]);
                }
                else
                {
                    (values[indices[i]], values[i]) = (values[i], values[indices[i]]);
                }
                yield return values.Take(size);
                indices[i]++;
                i = 0;
            }
            else
            {
                indices[i] = 0;
                i++;
            }
        }
    }
}
