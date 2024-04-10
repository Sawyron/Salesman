using FluentAssertions;
using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;
using Salesman.Domain.Tests.Pathfinders.Utils;

namespace Salesman.Domain.Tests.Pathfinders;

public class GreedySalesmanPathfinderTest
{
    private readonly GreedySalesmanPathfinder<int, int> _pathfinder = new();

    [Theory]
    [MemberData(nameof(GreedyGraphData.GetPathfinders), MemberType = typeof(GreedyGraphData))]
    public async Task FindPath_ShouldWork(Graph<int, int> graph, PathResult<int, int> expectedResult)
    {
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        // Assert
        actualResult.Length.Should().Be(expectedResult.Length);
        actualResult.Path.Should().Equal(expectedResult.Path);
    }
}
