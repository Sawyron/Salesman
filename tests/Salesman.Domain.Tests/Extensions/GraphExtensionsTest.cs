using FluentAssertions;
using Salesman.Domain.Extensions;
using Salesman.Domain.Graph;

namespace Salesman.Domain.Tests.Extensions;

public class GraphExtensionsTest
{
    [Fact]
    public void ToAdjacencyMatrix_Should_Work()
    {
        // Arrange
        var nodes = Enumerable.Range(0, 4);
        var adjacency = new Dictionary<int, IDictionary<int, int>>
        {
            [0] = new Dictionary<int, int>
            {
                {1, 5},
                {2, 1},
                {3, 4}
            },
            [1] = new Dictionary<int, int>
            {
                {0, 2},
                {2, 3},
                {3, 5}
            },
            [2] = new Dictionary<int, int>
            {
                {0, 1},
                {1, 4},
                {3, 2}
            },
            [3] = new Dictionary<int, int>
            {
                {0, 3},
                {1, 5},
                {2, 4},
            }
        };
        var graph = new Graph<int, int>(nodes, adjacency, true);
        int inf = int.MaxValue;
        var expectedMatrix = new int[4, 4]
        {
            { inf, 5, 1, 4 },
            { 2, inf, 3, 5 },
            { 1, 4, inf, 2 },
            { 3, 5, 4, inf }
        };
        // Act
        var actualMatrix = graph.ToAdjacencyMatrix();
        // Assert
        actualMatrix.Should().BeEquivalentTo(expectedMatrix);
    }
}
