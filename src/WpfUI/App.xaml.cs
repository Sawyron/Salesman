﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Salesman.Domain.Pathfinders;
using Salesman.Domain.Pathfinders.SimulatedAnnealing;
using System.Windows;
using WpfUI.Common;
using WpfUI.Navigation;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;
using WpfUI.UI.Menu;
using WpfUI.UI.ParameterSettings;
using WpfUI.UI.ParameterSettings.SimulatedAnnealing;
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
        services.AddSingleton<ParametersSettingsViewModel>();
        services.AddSingleton<SimulatedAnnealingParametersViewModel>();

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

        services.AddSingleton(_ => new Store<SimulatedAnnealingParameters>(new(20, 0.000001)));

        services.AddSingleton<Func<SimulatedAnnealingParameters>>(serviceProvider => () =>
        {
            var store = serviceProvider.GetRequiredService<Store<SimulatedAnnealingParameters>>();
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
