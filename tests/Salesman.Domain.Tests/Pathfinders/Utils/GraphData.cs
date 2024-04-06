using Salesman.Domain.Graph;
using Salesman.Domain.Pathfinders;

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
        (graph, result) = Create6NodeGraph1();
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
        var graph = new Graph<int, int>(nodes, adjacency, true);
        var result = new PathResult<int, int>([0, 2, 3, 1, 0], 10);
        return (graph, result);
    }

    internal static (Graph<int, int>, PathResult<int, int>) Create5NodeGraph1()
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
        var graph = new Graph<int, int>(nodes, adjacency);
        return (graph, new PathResult<int, int>([0, 4, 2, 1, 3, 0], 12));
    }

    internal static (Graph<int, int>, PathResult<int, int>) Create5NodeGraph2()
    {
        var nodes = Enumerable.Range(0, 5).ToList();
        var adjacency = new Dictionary<int, IDictionary<int, int>>
        {
            [0] = new Dictionary<int, int>
            {
                {1, 2},
                {2, 4},
                {3, 9},
                {4, 8}
            },
            [1] = new Dictionary<int, int>
            {
                {0, 5},
                {2, 4},
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
                {0, 5},
                {1, 2},
                {2, 3},
                {4, 8}
            },
            [4] = new Dictionary<int, int>
            {
                {0, 2},
                {1, 4},
                {2, 5},
                {3, 3}
            }
        };
        var graph = new Graph<int, int>(nodes, adjacency);
        return (graph, new PathResult<int, int>([0, 1, 3, 2, 4, 0], 13));
    }

    internal static (Graph<int, int>, PathResult<int, int>) Create6NodeGraph1()
    {
        var nodes = Enumerable.Range(0, 6).ToList();
        var adjacency = new Dictionary<int, IDictionary<int, int>>
        {
            [0] = new Dictionary<int, int>
            {
                {1, 53},
                {2, 112},
                {3, 237},
                {4, 421},
                {5, 414},
            },
            [1] = new Dictionary<int, int>
            {
                {0, 53},
                {2, 93},
                {3, 197},
                {4, 383},
                {5, 363},
            },
            [2] = new Dictionary<int, int>
            {
                {0, 112},
                {1, 93},
                {3, 137},
                {4, 314},
                {5, 339}
            },
            [3] = new Dictionary<int, int>
            {
                {0, 237},
                {1, 197},
                {2, 137},
                {4, 187},
                {5, 213},
            },
            [4] = new Dictionary<int, int>
            {
                {0, 421},
                {1, 383},
                {2, 314},
                {3, 187},
                {5, 199},
            },
            [5] = new Dictionary<int, int>
            {
                {0, 414},
                {1, 363},
                {2, 339},
                {3, 213},
                {4, 199},
            }
        };
        var graph = new Graph<int, int>(nodes, adjacency);
        return (graph, new PathResult<int, int>([0, 1, 5, 4, 3, 2, 0], 1051));
    }
}
