using FluentAssertions;
using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;
using Salesman.Domain.Tests.Pathfinders.Utils;

namespace Salesman.Domain.Tests.Pathfinders;
public class BranchAndBoundSalesmanPathfinderTest
{
    private readonly BranchAndBoundSalesmanPathfinder<int, int> _pathfinder = new();

    [Fact]
    public async Task Test()
    {
        // Arrange
        List<int> nodes = [1, 2, 3, 4, 5];
        var ajacency = new Dictionary<int, IDictionary<int, int>>
        {
            [1] = new Dictionary<int, int>
            {
                { 1, int.MaxValue },
                { 2, 20 },
                { 3, 18 },
                { 4, 12 },
                { 5, 8 },
            },
            [2] = new Dictionary<int, int>
            {
                { 1, 5 },
                { 2, int.MaxValue },
                { 3, 14 },
                { 4, 7 },
                { 5, 11 },
            },
            [3] = new Dictionary<int, int>
            {
                { 1, 12 },
                { 2, 18 },
                { 3, int.MaxValue },
                { 4, 6 },
                { 5, 11 },
            },
            [4] = new Dictionary<int, int>
            {
                { 1, 11 },
                { 2, 17 },
                { 3, 11 },
                { 4, int.MaxValue },
                { 5, 12 },
            },
            [5] = new Dictionary<int, int>
            {
                { 1, 5 },
                { 2, 5 },
                { 3, 5 },
                { 4, 5 },
                { 5, int.MaxValue },
            },
        };
        var graph = new Graph<int, int>(nodes, ajacency);
        var expectedLength = 41;
        var expectedPath = new List<int> { 1, 5, 3, 4, 2, 1 };
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        // Assert
        actualResult.Length.Should().Be(expectedLength);
        actualResult.Path.Should().Equal(expectedPath);
    }

    [Fact]
    public async Task Test4Nodes()
    {
        var (graph, result) = GraphData.Create4NodeGraph();
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        actualResult.Length.Should().Be(result.Length);
        actualResult.Path.Should().Equal(result.Path);
    }

    [Fact]
    public async Task Test5Nodes()
    {
        var (graph, result) = GraphData.Create5NodeGraph1();
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        actualResult.Length.Should().Be(result.Length);
        actualResult.Path.Should().Equal(result.Path);
    }

    [Fact]
    public async Task Test6Nodes()
    {
        var (graph, result) = GraphData.Create6NodeGraph1();
        // Act
        var actualResult = await _pathfinder.FindPathAsync(graph);
        actualResult.Length.Should().Be(result.Length);
        actualResult.Path.Should().Equal(result.Path);
    }
}
