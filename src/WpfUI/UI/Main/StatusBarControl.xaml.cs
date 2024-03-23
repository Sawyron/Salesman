using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI.Main;
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



    public string TimeLabel
    {
        get { return (string)GetValue(TimeLabelProperty); }
        set { SetValue(TimeLabelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TimeLabel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TimeLabelProperty =
        DependencyProperty.Register("TimeLabel", typeof(string), typeof(StatusBarControl), new PropertyMetadata(string.Empty));


}
