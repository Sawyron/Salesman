using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfUI.Common;

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

    public int CanvasRelativeSize
    {
        get { return (int)GetValue(CanvasRelativeSizeProperty); }
        set { SetValue(CanvasRelativeSizeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CanvasRelativeSize.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CanvasRelativeSizeProperty =
        DependencyProperty.Register("CanvasRelativeSize", typeof(int), typeof(GraphConnectionControl), new PropertyMetadata(100));


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
            new PropertyMetadata(new ObservableCollection<Connection>(), (d, e) =>
            {
                if (d is not GraphConnectionControl control || e.NewValue is not ObservableCollection<Connection> connections)
                {
                    return;
                }
                control.ActualConnections = new ObservableCollection<Connection>(connections.Select(control.MapToActualConnection));
                connections.CollectionChanged += control.OnConnectionsCollectionChanged;
                if (e.OldValue is ObservableCollection<Connection> oldConnections)
                {
                    oldConnections.CollectionChanged -= control.OnConnectionsCollectionChanged;
                }
            }));


    public ObservableCollection<Connection> ActualConnections
    {
        get { return (ObservableCollection<Connection>)GetValue(ActualConnectionsProperty); }
        private set { SetValue(ActualConnectionsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ActualConnections.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ActualConnectionsProperty =
        DependencyProperty.Register(
            "ActualConnections",
            typeof(ObservableCollection<Connection>),
            typeof(GraphConnectionControl),
            new PropertyMetadata(new ObservableCollection<Connection>()));

    private void OnConnectionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is IList<Connection> connections)
        {
            ActualConnections = new ObservableCollection<Connection>(connections.Select(MapToActualConnection));
        }
    }

    private Connection MapToActualConnection(Connection connection) => new()
    {
        FromNodeId = connection.FromNodeId,
        ToNodeId = connection.ToNodeId,
        StartX = connection.StartX / CanvasRelativeSize * canvasControl.ActualWidth,
        EndX = connection.EndX / CanvasRelativeSize * canvasControl.ActualWidth,
        StartY = connection.StartY / CanvasRelativeSize * canvasControl.ActualHeight,
        EndY = connection.EndY / CanvasRelativeSize * canvasControl.ActualHeight,
    };

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ActualConnections = new ObservableCollection<Connection>(Connections.Select(MapToActualConnection));
    }
}
