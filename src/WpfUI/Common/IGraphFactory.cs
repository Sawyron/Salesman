using Salesman.Domain.Graph;

namespace WpfUI.Common;

public interface IGraphFactory
{
    public Graph<int, int> CreateGraph(IEnumerable<Node> nodes, IEnumerable<Edge> edges);
}
