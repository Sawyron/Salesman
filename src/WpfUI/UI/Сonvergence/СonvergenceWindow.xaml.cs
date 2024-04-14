using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using ScottPlot.Plottables;
using System.Windows;
using static WpfUI.UI.Сonvergence.ConvergenceResult;

namespace WpfUI.UI.Сonvergence;

/// <summary>
/// Interaction logic for СonvergenceWindow.xaml
/// </summary>
public partial class СonvergenceWindow : Window
{
    private readonly IMessenger? _messenger;
    private readonly DataLogger _dataLogger;

    public СonvergenceWindow()
    {
        InitializeComponent();
        _dataLogger = plotView.Plot.Add.DataLogger();
        _dataLogger.LineStyle = new LineStyle
        {
            Width = 3,
            Color = Colors.DarkOrange
        };
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<ConvergenceViewModel>();
            _messenger = app.Services.GetRequiredService<IMessenger>();
            _messenger.Register<ConvergenceResultChangedMessage>(this, (r, m) =>
            {
                if (m.Value.Times.Count <= 1)
                {
                    _dataLogger.Data.Clear();
                }
                if (m.Value.Values.Count > 0)
                {
                    _dataLogger.Add(m.Value.Times[^1], m.Value.Values[^1]);
                    plotView.Refresh();
                } 
            });
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _messenger?.UnregisterAll(this);
        if (DataContext is ConvergenceViewModel vm)
        {
            vm.RunTestCommand.Cancel();
        }
    }
}
