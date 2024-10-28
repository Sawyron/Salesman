using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUI.Common;

namespace WpfUI.UI.Graph;
/// <summary>
/// Interaction logic for GraphNodesControl.xaml
/// </summary>
public partial class GraphNodesControl : UserControl
{
    public GraphNodesControl()
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
        DependencyProperty.Register("CanvasRelativeSize", typeof(int), typeof(GraphNodesControl), new PropertyMetadata(100));


    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(double), typeof(GraphNodesControl), new PropertyMetadata(5.0));


    public ObservableCollection<Node> Nodes
    {
        get { return (ObservableCollection<Node>)GetValue(NodeProperty); }
        set { SetValue(NodeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Nodes.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NodeProperty =
        DependencyProperty.Register(
            "Nodes",
            typeof(ObservableCollection<Node>),
            typeof(GraphNodesControl),
            new PropertyMetadata(new ObservableCollection<Node>(), (d, e) =>
            {
                if (d is not GraphNodesControl control)
                {
                    return;
                }
                if (e.NewValue is not ObservableCollection<Node> nodes)
                {
                    return;
                }
                control.NodeUIModels = new ObservableCollection<NodeUI>(
                    nodes.Select(control.MapNodeToUI));
                nodes.CollectionChanged += control.OnNodesCollectionChanged;
                if (e.OldValue is ObservableCollection<Node> oldNodes)
                {
                    oldNodes.CollectionChanged -= control.OnNodesCollectionChanged;
                }
            }));

    public ObservableCollection<NodeUI> NodeUIModels
    {
        get { return (ObservableCollection<NodeUI>)GetValue(NodeUIModelsProperty); }
        set { SetValue(NodeUIModelsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NodeUIModels.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NodeUIModelsProperty =
        DependencyProperty.Register(
            "NodeUIModels",
            typeof(ObservableCollection<NodeUI>),
            typeof(GraphNodesControl),
            new PropertyMetadata(new ObservableCollection<NodeUI>()));

    public ICommand? OnClickCommand
    {
        get { return (ICommand)GetValue(OnClickCommandProperty); }
        set { SetValue(OnClickCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OnClickCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OnClickCommandProperty =
        DependencyProperty.Register("OnClickCommand", typeof(ICommand), typeof(GraphNodesControl), new PropertyMetadata(null));


    public ICommand? OnNodeClickCommand
    {
        get { return (ICommand?)GetValue(OnNodeClickCommandProperty); }
        set { SetValue(OnNodeClickCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OnNodeClickCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OnNodeClickCommandProperty =
        DependencyProperty.Register("OnNodeClickCommand", typeof(ICommand), typeof(GraphNodesControl), new PropertyMetadata(null));

    private void OnCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is not Canvas || OnClickCommand is null)
        {
            return;
        }
        Point position = e.GetPosition(canvasControl);
        position.X = position.X / canvasControl.ActualWidth * CanvasRelativeSize;
        position.Y = position.Y / canvasControl.ActualHeight * CanvasRelativeSize;
        if (OnClickCommand is not null && OnClickCommand.CanExecute(position))
        {
            OnClickCommand.Execute(position);
        }
    }

    private void OnNodeControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is NodeControl nodeControl)
        {
            var node = Nodes.FirstOrDefault(n => n.Name == nodeControl.Text);
            if (node is not null
                && OnNodeClickCommand is not null
                && OnNodeClickCommand.CanExecute(node))
            {
                OnNodeClickCommand.Execute(node);
            }
        }
    }

    private void OnNodesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is IList<Node> nodes)
        {
            NodeUIModels = new ObservableCollection<NodeUI>(nodes.Select(MapNodeToUI));
        }
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        NodeUIModels = new ObservableCollection<NodeUI>(Nodes.Select(MapNodeToUI));
    }

    private NodeUI MapNodeToUI(Node node) => new()
    {
        Name = node.Name,
        X = node.X / CanvasRelativeSize * canvasControl.ActualWidth,
        Y = node.Y / CanvasRelativeSize * canvasControl.ActualHeight,
        Radius = Radius / CanvasRelativeSize * (canvasControl.ActualWidth + canvasControl.ActualHeight) / 2
    };
}
