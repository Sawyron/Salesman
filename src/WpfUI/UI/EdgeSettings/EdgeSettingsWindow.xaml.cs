using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfUI.UI.EdgeSettings;
/// <summary>
/// Interaction logic for EdgeSettingsWindow.xaml
/// </summary>
public partial class EdgeSettingsWindow : Window
{
    public EdgeSettingsWindow()
    {
        InitializeComponent();
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<EdgeSettingsViewModel>();
        }
    }
}
