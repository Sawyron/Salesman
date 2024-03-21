using System.Collections.ObjectModel;

namespace WpfUI.Data;
public class GraphHolder
{
    public ObservableCollection<Node> Nodes { get; } = [];
    public ObservableCollection<Edge> Edges { get; } = [];
}
