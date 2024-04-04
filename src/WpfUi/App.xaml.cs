using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Salesman.Domain.Pathfinders;
using System.Windows;
using WpfUI.Data;
using WpfUI.UI.EdgeSettings;

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
        services.AddSingleton<ExhaustiveSearchSalesmanPathfinder<int, int>>();
        services.AddSingleton<DynamicSalesmanPathfinder<int, int>>();
        services.AddSingleton<GreedySalesmanPathfinder<int, int>>();
        services.AddSingleton<BranchAndBoundSalesmanPathfinder<int, int>>();
        services.AddSingleton(s => new PathfinderRepository(
            [
                new()
                {
                    Id = 0,
                    Name = "Exhaustive",
                    Method = s.GetRequiredService<ExhaustiveSearchSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 1,
                    Name = "Dynamic",
                    Method = s.GetRequiredService<DynamicSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 2,
                    Name = "Greedy",
                    Method = s.GetRequiredService<GreedySalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 3,
                    Name = "Branch and bounds",
                    Method = s.GetRequiredService<BranchAndBoundSalesmanPathfinder<int, int>>()
                }
            ]));
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        return services.BuildServiceProvider();
    }
}
