using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfUI.UI.Graph;

/// <summary>
/// Interaction logic for NodeControl.xaml
/// </summary>
public partial class NodeControl : UserControl
{
    public NodeControl()
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
        DependencyProperty.Register("Radius", typeof(int), typeof(NodeControl), new PropertyMetadata(50));

    public Brush Fill
    {
        get { return (Brush)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(NodeControl), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

    public Brush Stroke
    {
        get { return (Brush)GetValue(StrokeProperty); }
        set { SetValue(StrokeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register("Stroke", typeof(Brush), typeof(NodeControl), new PropertyMetadata(new SolidColorBrush(Colors.Green)));


    public Brush TextColor
    {
        get { return (Brush)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextColorProperty =
        DependencyProperty.Register("TextColor", typeof(Brush), typeof(NodeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(NodeControl), new PropertyMetadata(string.Empty));



    public ICommand? LeftMouseDownCommand
    {
        get { return (ICommand)GetValue(MyPropertyProperty); }
        set { SetValue(MyPropertyProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LeftMouseDownCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.Register("LeftMouseDownCommand", typeof(ICommand), typeof(NodeControl), new PropertyMetadata(null));

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (LeftMouseDownCommand is null)
        {
            return;
        }
        if (LeftMouseDownCommand.CanExecute(null))
        {
            LeftMouseDownCommand.Execute(null);
        }
    }
}
