using Salesman.Domain.Graph;
using System.Collections.ObjectModel;

namespace WpfUI.Common;

public class GraphHolder(IGraphFactory graphFactory)
{
    private readonly IGraphFactory _graphFactory = graphFactory;

    public ObservableCollection<Node> Nodes { get; } = [];
    public ObservableCollection<Edge> Edges { get; } = [];

    public Graph<int, int> CrateGraph() => _graphFactory.CreateGraph(Nodes, Edges);
}
