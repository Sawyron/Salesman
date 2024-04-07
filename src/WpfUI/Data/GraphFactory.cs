using Salesman.Domain.Graph;
using WpfUI.UI.Graph;

namespace WpfUI.Data;
public class GraphFactory : IGraphFactory
{
    public Graph<int, int> CreateGraph(IEnumerable<Node> nodes, IEnumerable<Edge> edges)
    {
        var graphNodes = nodes.Select(n => n.Id);
        var adjacency = edges.GroupBy(e => e.FromId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.ToId, x => x.Value) as IDictionary<int, int>);
        return new Graph<int, int>(graphNodes, adjacency);
    }
}
