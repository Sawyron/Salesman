using WpfUI.UI.Graph;

namespace WpfUI.Domain;
public interface IGraphFactory
{
    public Graph<int, int> CreateGraph(IEnumerable<Node> nodes, IEnumerable<Edge> edges);
}
