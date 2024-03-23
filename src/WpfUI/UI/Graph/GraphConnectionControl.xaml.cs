using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

    public Brush LineColor
    {
        get { return (Brush)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LineColorProperty =
        DependencyProperty.Register(
            "LineColor",
            typeof(Brush),
            typeof(GraphConnectionControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    public ObservableCollection<Connection> Connections
    {
        get { return (ObservableCollection<Connection>)GetValue(ConnectionsProperty); }
        set { SetValue(ConnectionsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Connections.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConnectionsProperty =
        DependencyProperty.Register(
            "Connections",
            typeof(ObservableCollection<Connection>),
            typeof(GraphConnectionControl),
            new PropertyMetadata(new ObservableCollection<Connection>()));
}
