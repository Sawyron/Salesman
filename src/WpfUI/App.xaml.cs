using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Salesman.Domain.Pathfinders;
using Salesman.Domain.Pathfinders.Ant;
using Salesman.Domain.Pathfinders.Genetic;
using Salesman.Domain.Pathfinders.SimulatedAnnealing;
using System.Windows;
using WpfUI.Common;
using WpfUI.Navigation;
using WpfUI.UI;
using WpfUI.UI.Convergence;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;
using WpfUI.UI.Menu;
using WpfUI.UI.ParameterSettings;
using WpfUI.UI.ParameterSettings.Ant;
using WpfUI.UI.ParameterSettings.Genetic;
using WpfUI.UI.ParameterSettings.SimulatedAnnealing;

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
        services.AddSingleton<ParametersSettingsViewModel>();
        services.AddSingleton<SimulatedAnnealingParametersViewModel>();
        services.AddSingleton<AntParametersViewModel>();
        services.AddSingleton<GeneticParametersViewModel>();

        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        services.AddSingleton<GraphHolder>();
        services.AddSingleton<IGraphFactory, GraphFactory>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider =>
            type => (ObservableObject)serviceProvider.GetRequiredService(type));

        services.AddSingleton<ExhaustiveSearchSalesmanPathfinder<int, int>>();
        services.AddSingleton<DynamicSalesmanPathfinder<int, int>>();
        services.AddSingleton<GreedySalesmanPathfinder<int, int>>();
        services.AddSingleton<BranchAndBoundSalesmanPathfinder<int, int>>();
        services.AddSingleton<BranchAndBoundSalesmanPathfinder<int, int>>();
        services.AddSingleton<RandomSearchSalesmanPathfinder<int, int>>();
        services.AddSingleton(_ => new BacktrackingRandomSearchSalesmanPathfinder<int, int>(1000));
        services.AddSingleton<SimulatedAnnealingSalesmanPathfinder<int, int>>();
        services.AddSingleton<AntSalesmanPathfinder<int, int>>();
        services.AddSingleton<GeneticSalesmanPathfinder<int, int>>();

        services.AddSingleton(_ => new Store<UIParameters>(new UIParameters(1000, 50)));
        services.AddSingleton(_ => new Store<SimulatedAnnealingParameters>(new(20, 0.000001)));
        services.AddSingleton(_ => new Store<AntParameters>(new(
            Alpha: 1,
            Beta: 4,
            Q: 4,
            P: 0.4,
            InitialPheromone: 0.2,
            IterationsWithoutImprovementsThreshold: 1000)));
        services.AddSingleton(_ => new Store<GeneticParameters>(new(
            MutationProbability: 0.6,
            Iterations: 1000)));

        services.AddSingleton<Func<SimulatedAnnealingParameters>>(serviceProvider => () =>
        {
            var store = serviceProvider.GetRequiredService<Store<SimulatedAnnealingParameters>>();
            return store.Value;
        });
        services.AddSingleton<Func<AntParameters>>(serviceProvider => () =>
        {
            var store = serviceProvider.GetRequiredService<Store<AntParameters>>();
            return store.Value;
        });
        services.AddSingleton<Func<GeneticParameters>>(serviceProvider => () =>
        {
            var store = serviceProvider.GetRequiredService<Store<GeneticParameters>>();
            return store.Value;
        });

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
                },
                new()
                {
                    Id = 4,
                    Name = WpfUI.Resources.Pathfinders.RandomSearch,
                    Method = s.GetRequiredService<RandomSearchSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 5,
                    Name = WpfUI.Resources.Pathfinders.BacktrackingRandomSearch,
                    Method = s.GetRequiredService<BacktrackingRandomSearchSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 6,
                    Name = WpfUI.Resources.Pathfinders.SimulatedAnnealing,
                    Method = s.GetRequiredService<SimulatedAnnealingSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 7,
                    Name = WpfUI.Resources.Pathfinders.Ant,
                    Method = s.GetRequiredService<AntSalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 8,
                    Name = WpfUI.Resources.Pathfinders.Genetic,
                    Method = s.GetRequiredService<GeneticSalesmanPathfinder<int, int>>()
                },
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
        return services.BuildServiceProvider();
    }
}
