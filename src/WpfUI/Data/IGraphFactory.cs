using Salesman.Domain.Graph;
using WpfUI.UI.Graph;

namespace WpfUI.Data;

public interface IGraphFactory
{
    public Graph<int, int> CreateGraph(IEnumerable<Node> nodes, IEnumerable<Edge> edges);
}
