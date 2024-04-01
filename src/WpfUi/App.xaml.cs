﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfUI.Data;
using WpfUI.Domain;
using WpfUI.Pathfinders;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.Graph;

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
        services.AddSingleton<DummySalesmanPathfinder<int, int>>();
        services.AddSingleton<ExhaustiveSearchSalesmanPathfinder<int, int>>();
        services.AddSingleton(s => new PathfinderRepository(
            [
                new()
                {
                    Id=0,
                    Name = "Dummy",
                    Method = s.GetRequiredService<DummySalesmanPathfinder<int, int>>()
                },
                new()
                {
                    Id = 1,
                    Name = "Exhaustive",
                    Method = s.GetRequiredService<ExhaustiveSearchSalesmanPathfinder<int, int>>()
                }
            ]));
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        return services.BuildServiceProvider();
    }
}
