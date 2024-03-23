using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    public int Radius
    {
        get { return (int)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(int), typeof(GraphNodesControl), new PropertyMetadata(50));


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
            new PropertyMetadata(new ObservableCollection<Node>()));

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
        if (e.Source is not Canvas)
        {
            return;
        }
        if (OnClickCommand is null)
        {
            return;
        }
        var position = e.GetPosition(canvasControl);
        if (OnClickCommand is not null && OnClickCommand.CanExecute(position))
        {
            OnClickCommand.Execute(position);
        }
    }
    private void NodeControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
}
