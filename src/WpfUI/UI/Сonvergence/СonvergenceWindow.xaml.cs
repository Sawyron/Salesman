using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;
using System.Windows;
using WpfUI.Resources;
using static WpfUI.UI.Сonvergence.ConvergenceResult;

namespace WpfUI.UI.Сonvergence;

/// <summary>
/// Interaction logic for СonvergenceWindow.xaml
/// </summary>
public partial class СonvergenceWindow : Window
{
    private readonly IMessenger? _messenger;

    public СonvergenceWindow()
    {
        InitializeComponent();
        plotView.Plot.Axes.Left.Label.Text = Labels.ObjectiveFunctionValues;
        plotView.Plot.Axes.Bottom.Label.Text = Labels.SolutionTime;
        if (Application.Current is App app)
        {
            DataContext = app.Services.GetRequiredService<ConvergenceViewModel>();
            _messenger = app.Services.GetRequiredService<IMessenger>();
            _messenger.Register<ConvergenceResultChangedMessage>(this, (r, m) =>
            {
                plotView.Plot.Clear();
                var scatter = plotView.Plot.Add.ScatterLine(m.Value.Times, m.Value.Values, Colors.DarkOrange);
                scatter.LineWidth = 3;
                plotView.Plot.Axes.AutoScale();
                plotView.Refresh();
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
