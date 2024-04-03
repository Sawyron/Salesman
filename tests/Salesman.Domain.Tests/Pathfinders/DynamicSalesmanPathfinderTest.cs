using FluentAssertions;
using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;
using Salesman.Domain.Tests.Pathfinders.Utils;

namespace Salesman.Domain.Tests.Pathfinders;
public class DynamicSalesmanPathfinderTest
{
    private readonly DynamicSalesmanPathfinder<int, int> _pathfinder = new();

    [Theory]
    [MemberData(nameof(GraphData.GetPathfinders), MemberType = typeof(GraphData))]
    public async Task FindPath_ShouldReturnShortestPath(Graph<int, int> graph, PathResult<int, int> expectedResult)
    {
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        // Assert
        actualResult.Length.Should().Be(expectedResult.Length);
        actualResult.Path.Should().Equal(actualResult.Path);
    }
}
