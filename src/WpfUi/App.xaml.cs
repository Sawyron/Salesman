using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfUI.UI.Graph;
using WpfUI.UI.Main;

namespace WpfUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Services = ConfigureServices();
    }

    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddTransient<GraphViewModel>();
        services.AddTransient<MainViewModel>();
        return services.BuildServiceProvider();
    }
}
