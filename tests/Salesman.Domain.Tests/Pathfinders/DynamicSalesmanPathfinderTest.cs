using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;
using System.Reflection;

namespace Salesman.Domain.Tests.Pathfinders;
public class DynamicSalesmanPathfinderTest
{
    private readonly DynamicSalesmanPathfinder<int, int> _pathfinder = new();

    [Theory]
    [MemberData(nameof(GetPathfinders))]
    public async Task FindPath_ShouldReturnShortestPath(Graph<int, int> graph, PathResult<int, int> expectedResult)
    {
        // Assert
        var actualResult = await _pathfinder.FindPathAsync(graph);
        Assert.Equal(expectedResult.Length, actualResult.Length);
        Assert.Equal(expectedResult.Path, actualResult.Path);
    }

    public static IEnumerable<object[]> GetPathfinders()
    {
        //var (graph, result) = Create4NodeGraph();
        //yield return new object[] { graph, result };
        //(graph, result) = CreateEmptyGraph();
        //yield return new object[] { graph, result };
        //(graph, result) = CreateOneNodeGraph();
        //yield return new object[] { graph, result };
        var (graph, result) = CreateTwoNodeGraph();
        yield return new object[] { graph, result };
    }

    private static (Graph<int, int>, PathResult<int, int>) CreateEmptyGraph()
    {
        var graph = new Graph<int, int>([], new Dictionary<int, IDictionary<int, int>>());
        return (graph, new PathResult<int, int>([], 0));
    }

    private static (Graph<int, int>, PathResult<int, int>) CreateOneNodeGraph()
    {
        var graph = new Graph<int, int>([1], new Dictionary<int, IDictionary<int, int>>());
        return (graph, new PathResult<int, int>([], 0));
    }

    private static (Graph<int, int>, PathResult<int, int>) CreateTwoNodeGraph()
    {
        var graph = new Graph<int, int>([1, 2], new Dictionary<int, IDictionary<int, int>>
        {
            [1] = new Dictionary<int, int> { { 2, 3 } }
        });
        return (graph, new PathResult<int, int>([2], 3));
    }
    private static (Graph<int, int>, PathResult<int, int>) Create4NodeGraph()
    {
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
        Graph<int, int> graph = new Graph<int, int>(nodes, adjacency, true);
        var result = new PathResult<int, int>([0, 2, 3, 1, 0], 10);
        return (graph, result);
    }

    private static Graph<int, int> Create5NodeGraph1()
    {
        var nodes = Enumerable.Range(0, 5).ToList();
        var adjacency = new Dictionary<int, IDictionary<int, int>>
        {
            [0] = new Dictionary<int, int>
            {
                {1, 3},
                {2, 5},
                {3, 4},
                {4, 1}
            },
            [1] = new Dictionary<int, int>
            {
                {0, 5},
                {2, 5},
                {3, 1},
                {4, 2}
            },
            [2] = new Dictionary<int, int>
            {
                {0, 6},
                {1, 2},
                {3, 3},
                {4, 5}
            },
            [3] = new Dictionary<int, int>
            {
                {0, 3},
                {1, 4},
                {2, 5},
                {4, 8}
            },
            [4] = new Dictionary<int, int>
            {
                {0, 2},
                {1, 3},
                {2, 5},
                {3, 9}
            }
        };
        return new Graph<int, int>(nodes, adjacency);
    }
}
