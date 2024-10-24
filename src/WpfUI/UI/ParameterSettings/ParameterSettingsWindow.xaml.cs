using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfUI.UI.ParameterSettings;
/// <summary>
/// Interaction logic for ParameterSettingsWindow.xaml
/// </summary>
public partial class ParameterSettingsWindow : Window
{
    public ParameterSettingsWindow()
    {
        InitializeComponent();
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<ParametersSettingsViewModel>();
        }
    }
}
