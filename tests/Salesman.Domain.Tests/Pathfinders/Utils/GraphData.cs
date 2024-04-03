using Salesman.Domain.Graph;

namespace Salesman.Domain.Tests.Pathfinders.Utils;
internal class GraphData
{
    public static TheoryData<Graph<int, int>, PathResult<int, int>> GetPathfinders()
    {
        var data = new TheoryData<Graph<int, int>, PathResult<int, int>>();
        var (graph, result) = Create4NodeGraph();
        data.Add(graph, result);
        (graph, result) = CreateEmptyGraph();
        data.Add(graph, result);
        (graph, result) = CreateOneNodeGraph();
        data.Add(graph, result);
        (graph, result) = CreateTwoNodeGraph();
        data.Add(graph, result);
        return data;
    }

    internal static (Graph<int, int>, PathResult<int, int>) CreateEmptyGraph()
    {
        var graph = new Graph<int, int>([], new Dictionary<int, IDictionary<int, int>>());
        return (graph, new PathResult<int, int>([], 0));
    }

    internal static (Graph<int, int>, PathResult<int, int>) CreateOneNodeGraph()
    {
        var graph = new Graph<int, int>([1], new Dictionary<int, IDictionary<int, int>>());
        return (graph, new PathResult<int, int>([], 0));
    }

    internal static (Graph<int, int>, PathResult<int, int>) CreateTwoNodeGraph()
    {
        int pathLength = 3;
        var graph = new Graph<int, int>([1, 2], new Dictionary<int, IDictionary<int, int>>
        {
            [1] = new Dictionary<int, int> { { 2, pathLength } }
        });
        return (graph, new PathResult<int, int>([1, 2, 1], pathLength * 2));
    }
    internal static (Graph<int, int>, PathResult<int, int>) Create4NodeGraph()
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

    internal static Graph<int, int> Create5NodeGraph1()
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
