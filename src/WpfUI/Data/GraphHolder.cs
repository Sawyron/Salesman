using System.Collections.ObjectModel;
using WpfUI.UI.Graph;

namespace WpfUI.Data;
public class GraphHolder
{
    public ObservableCollection<Node> Nodes { get; } = [];
    public ObservableCollection<Edge> Edges { get; } = [];
}
