using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI.Menu;

/// <summary>
/// Interaction logic for MenuControl.xaml
/// </summary>
public partial class MenuControl : UserControl
{
    public MenuControl()
    {
        InitializeComponent();
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<MenuViewModel>();
        }
    }
}
