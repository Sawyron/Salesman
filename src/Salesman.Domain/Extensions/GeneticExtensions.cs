namespace Salesman.Domain.Extensions;

public static class GeneticExtensions
{
    public static IList<T> Cross<T>(this IList<T> first, IList<T> second, int crossPoint)
    {
        var result = new List<T>(first.Count);
        result.AddRange(first.Take(crossPoint + 1));
        HashSet<T> addedElements = [.. result];
        List<T> leftFirstElements = first.Skip(crossPoint + 1).ToList();
        for (int i = crossPoint + 1; i < second.Count; i++)
        {
            T chosenElement = addedElements.Contains(second[i]) ?
                leftFirstElements[0]
                : second[i];
            result.Add(chosenElement);
            addedElements.Add(chosenElement);
            leftFirstElements.Remove(chosenElement);
        }
        return result;
    }
}
