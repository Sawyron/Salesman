namespace WpfUI.Extensions;
public static class CombinatoricsExtensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int size)
    {
        var values = source.ToArray();
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
            int index = size;
            foreach (int i in Enumerable.Range(0, size).Reverse())
            {
                if (indices[i] != i + n - size)
                {
                    index = i;
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
            indices[index] += 1;
            foreach (int j in Enumerable.Range(index + 1, size))
            {
                indices[j] = indices[j - 1] + 1;
            }
            yield return indices.Select(i => values[i]);
        }
    }

    public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source, int size)
    {
        var values = source.ToArray();
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
