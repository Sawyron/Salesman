using Salesman.Domain.Graph;
using System.Collections.ObjectModel;

namespace WpfUI.Common;

public class GraphHolder(IGraphFactory graphFactory)
{
    private readonly IGraphFactory _graphFactory = graphFactory;

    public ObservableCollection<Node> Nodes { get; } = [];
    public ObservableCollection<Edge> Edges { get; } = [];

    public Graph<int, int> CrateGraph() => _graphFactory.CreateGraph(Nodes, Edges);

    public void Clear()
    {
        Nodes.Clear();
        Edges.Clear();
    }

    public void AddNode(double x, double y)
    {
        int id = Nodes.Select(n => n.Id)
                        .DefaultIfEmpty()
                        .Max() + 1;
        var node = new Node
        {
            Id = id,
            X = x,
            Y = y,
            Name = $"{id}"
        };
        foreach (var existingNode in Nodes)
        {
            Edges.Add(new Edge
            {
                FromId = existingNode.Id,
                ToId = node.Id,
                Value = Convert.ToInt32(
                    Math.Sqrt(
                        Math.Pow(existingNode.X - node.X, 2)
                        + Math.Pow(existingNode.Y - node.Y, 2)))
            });
        }
        Nodes.Add(node);
    }

    public void RemoveNode(Node node)
    {
        Nodes.Remove(node);
        var edgesToRemove = Edges.Where(e => e.FromId == node.Id || e.ToId == node.Id)
            .ToList();
        foreach (var edge in edgesToRemove)
        {
            Edges.Remove(edge);
        }
    }
}
