using Salesman.Domain.Extensions;

namespace Salesman.Domain.Tests.Extensions;

public class GeneticExtensionsTest
{
    [Fact]
    public void TestCross()
    {
        List<int> first = [0, 3, 1, 4, 2];
        List<int> second = [0, 2, 4, 3, 1];
        List<int> expected = [0, 1, 4, 1, 2];
        int crossingPoint = 1;

        IList<int> actual = first.Cross(second, crossingPoint);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestCross2()
    {
        List<int> first = [0, 3, 1, 4, 2];
        List<int> second = [0, 2, 4, 3, 1];
        List<int> expected = [0, 2, 1, 4, 3];
        int crossingPoint = 1;

        IList<int> actual = second.Cross(first, crossingPoint);
        Assert.Equal(expected, actual);
    }
}
