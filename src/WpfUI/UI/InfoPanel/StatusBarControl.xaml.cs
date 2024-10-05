using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI.InfoPanel;
/// <summary>
/// Interaction logic for StatusBarControl.xaml
/// </summary>
public partial class StatusBarControl : UserControl
{
    public StatusBarControl()
    {
        InitializeComponent();
    }

    public bool IsInProcess
    {
        get { return (bool)GetValue(IsInProcessProperty); }
        set { SetValue(IsInProcessProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsInProcess.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsInProcessProperty =
        DependencyProperty.Register("IsInProcess", typeof(bool), typeof(StatusBarControl), new PropertyMetadata(false));

    public double ExecutionTime
    {
        get { return (double)GetValue(ExecutionTimeProperty); }
        set { SetValue(ExecutionTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ExecutionTime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ExecutionTimeProperty =
        DependencyProperty.Register(
            "ExecutionTime",
            typeof(double),
            typeof(StatusBarControl),
            new PropertyMetadata(0.0));
}
