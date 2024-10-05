using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI.Graph;
/// <summary>
/// Interaction logic for GraphControl.xaml
/// </summary>
public partial class GraphControl : UserControl
{
    public GraphControl()
    {
        InitializeComponent();
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<GraphControlViewModel>();
        }
    }
}
