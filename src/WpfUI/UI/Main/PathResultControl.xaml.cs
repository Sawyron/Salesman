using System.Windows;
using System.Windows.Controls;
using WpfUI.Domain;

namespace WpfUI.UI.Main;
/// <summary>
/// Interaction logic for PathResultControl.xaml
/// </summary>
public partial class PathResultControl : UserControl
{
    public PathResultControl()
    {
        InitializeComponent();
    }

    public PathResult<int, int>? PathResult
    {
        get { return (PathResult<int, int>)GetValue(PathResultProperty); }
        set { SetValue(PathResultProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PathResult.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PathResultProperty =
        DependencyProperty.Register(
            "PathResult",
            typeof(PathResult<int, int>),
            typeof(PathResultControl),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is not PathResultControl control)
                {
                    return;
                }
                if (e.NewValue is not PathResult<int, int> result)
                {
                    return;
                }
                (IReadOnlyList<int> path, int length) = result;
                control.PathLength = length;
                control.PathLabel = string.Join(" -> ", path);
            }));

    private int PathLength
    {
        get { return (int)GetValue(PathLengthProperty); }
        set { SetValue(PathLengthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PathLength.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PathLengthProperty =
        DependencyProperty.Register("PathLength", typeof(int), typeof(PathResultControl), new PropertyMetadata(0));
    private string PathLabel
    {
        get { return (string)GetValue(PathLabelProperty); }
        set { SetValue(PathLabelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PathLabel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PathLabelProperty =
        DependencyProperty.Register("PathLabel", typeof(string), typeof(PathResultControl), new PropertyMetadata(string.Empty));


}
