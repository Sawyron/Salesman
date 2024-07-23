using Salesman.Domain.Extensions;

namespace Salesman.Domain.Tests.Extensions;

public class CombinatoricsExtensionsTest
{
    [Fact]
    public void WhenSorceIsNonEmpty_ThenShouldReturnCorrectResult()
    {
        var testString = "ABCD";
        List<string> combinations = testString.Combinations(2).
            Select(c => new string(c.ToArray()))
            .ToList();
        Assert.Equal(6, combinations.Count);
        Assert.Equal("AB", combinations[0]);
        Assert.Equal("AC", combinations[1]);
        Assert.Equal("AD", combinations[2]);
        Assert.Equal("BC", combinations[3]);
        Assert.Equal("BD", combinations[4]);
        Assert.Equal("CD", combinations[5]);
    }

    [Fact]
    public void WhenCombinationsSourceIsEmpty_ThenShouldEmptyResultReturn()
    {
        int[] array = [];
        IEnumerable<IEnumerable<int>> result = array.Combinations(2);
        Assert.Empty(result);
    }

    [Fact]
    public void WhenCombinationsSizeIsZero_ThenShouldReturnEmptyResult()
    {
        int[] array = [];
        var result = array.Combinations(0);
        Assert.Empty(result);
    }

    [Fact]
    public void WhenPermutationsSourceIsEmpty_ThenShouldReturnEmptyResult()
    {
        int[] array = [];
        var result = array.Permutations().ToList();
        Assert.Empty(result);
    }
}
