using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfUI.Data;
using WpfUI.Domain;
using WpfUI.Pathfinders;
using WpfUI.UI.EdgeSettings;
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
        services.AddTransient<MainViewModel>();
        services.AddTransient<EdgeSettingsViewModel>();
        services.AddSingleton<GraphHolder>();
        services.AddSingleton<IGraphFactory, GraphFactory>();
        services.AddSingleton(typeof(ISalesmanPathfinder<,>), typeof(DummySalesmanPathfinder<,>));
        return services.BuildServiceProvider();
    }
}
