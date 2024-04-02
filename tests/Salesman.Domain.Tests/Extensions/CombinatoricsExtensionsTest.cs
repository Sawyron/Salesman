using Salesman.Domain.Extensions;

namespace Salesman.Domain.Tests.Extensions;
public class CombinatoricsExtensionsTest
{
    [Fact]
    public void Combinations_ShouldWork()
    {
        var testString = "ABCD";
        var combinations = testString.Combinations(2).
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
}
