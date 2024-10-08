using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Salesman.Domain.Pathfinders;
using System.Windows;
using WpfUI.Common;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;
using WpfUI.UI.Menu;
using WpfUI.UI.Сonvergence;

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
        services.AddTransient<ConvergenceViewModel>();
        services.AddSingleton<GraphControlViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddSingleton<GraphHolder>();
        services.AddSingleton<IGraphFactory, GraphFactory>();
        services.AddSingleton<ExhaustiveSearchSalesmanPathfinder<int, int>>();
        services.AddSingleton<DynamicSalesmanPathfinder<int, int>>();
        services.AddSingleton<GreedySalesmanPathfinder<int, int>>();
        services.AddSingleton<BranchAndBoundSalesmanPathfinder<int, int>>();
        services.AddSingleton<BranchAndBoundSalesmanPathfinder<int, int>>();
        services.AddSingleton(s => new PathfinderRepository(
            [
                new()
                {
                    Id = 0,
                    Name = WpfUI.Resources.Pathfinders.Exhaustive,
                    Method = s.GetRequiredService<ExhaustiveSearchSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 1,
                    Name = WpfUI.Resources.Pathfinders.Dynamic,
                    Method = s.GetRequiredService<DynamicSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 2,
                    Name = WpfUI.Resources.Pathfinders.Greedy,
                    Method = s.GetRequiredService<GreedySalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 3,
                    Name = WpfUI.Resources.Pathfinders.BnB,
                    Method = s.GetRequiredService<BranchAndBoundSalesmanPathfinder<int, int>>()
                }
            ],
            [
                new ReportingPathfinder
                {
                    Name = WpfUI.Resources.Pathfinders.Exhaustive,
                    Metgod = s.GetRequiredService<ExhaustiveSearchSalesmanPathfinder<int, int>>()
                },
                new ReportingPathfinder
                {
                    Name = WpfUI.Resources.Pathfinders.BnB,
                    Metgod = s.GetRequiredService<BranchAndBoundSalesmanPathfinder<int, int>>()
                },
            ]));
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        return services.BuildServiceProvider();
    }
}
