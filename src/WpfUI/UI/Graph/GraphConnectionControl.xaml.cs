using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI.Graph;
/// <summary>
/// Interaction logic for GraphConnectionControl.xaml
/// </summary>
public partial class GraphConnectionControl : UserControl
{
    public GraphConnectionControl()
    {
        InitializeComponent();
    }

    public ObservableCollection<ConnectionModel> Connections
    {
        get { return (ObservableCollection<ConnectionModel>)GetValue(ConnectionsProperty); }
        set { SetValue(ConnectionsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Connections.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConnectionsProperty =
        DependencyProperty.Register(
            "Connections",
            typeof(ObservableCollection<ConnectionModel>),
            typeof(GraphConnectionControl),
            new PropertyMetadata(new ObservableCollection<ConnectionModel>()));
}
