using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfUI.Data;
using WpfUI.UI.Graph;

namespace WpfUI.UI.EdgeSettings;
public class EdgeSettingsViewModel(GraphHolder graphHolder) : ObservableObject
{
    private readonly GraphHolder _graphHolder = graphHolder;

    public ObservableCollection<Edge> Edges => _graphHolder.Edges;
}
