namespace Salesman.Domain.Extensions;

public static class ProbabilityExtensions
{
    public static int Choose(this IList<double> probabilities, double choice)
    {
        double lowerBound = 0;
        for (int i = 0; i < probabilities.Count; i++)
        {
            double probability = probabilities[i];
            if (choice >= lowerBound && choice <= lowerBound + probability)
            {
                return i;
            }
            lowerBound += probability;
        }
        return -1;
    }
}
