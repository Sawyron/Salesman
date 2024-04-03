using Salesman.Domain.Graph;

namespace Salesman.Domain.Tests.Pathfinders.Utils;
internal class GreedyGraphData
{
    public static TheoryData<Graph<int, int>, PathResult<int, int>> GetPathfinders()
    {
        var data = new TheoryData<Graph<int, int>, PathResult<int, int>>();
        var (graph, result) = GraphData.CreateEmptyGraph();
        data.Add(graph, result);
        (graph, result) = GraphData.CreateOneNodeGraph();
        data.Add(graph, result);
        (graph, result) = GraphData.CreateTwoNodeGraph();
        data.Add(graph, result);
        (graph, result) = Create4NodeGraph();
        data.Add(graph, result);
        return data;
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
}
