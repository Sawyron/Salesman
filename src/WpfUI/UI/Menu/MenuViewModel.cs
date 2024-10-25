using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using WpfUI.UI.EdgeSettings;
using WpfUI.UI.InfoPanel;
using WpfUI.UI.ParameterSettings;
using WpfUI.UI.Convergence;

namespace WpfUI.UI.Menu;

public class MenuViewModel : ObservableObject, IRecipient<GraphUIState.ChangedMessage>
{
    public MenuViewModel(IMessenger messenger)
    {
        ExitCommand = new RelayCommand(() => Environment.Exit(0));
        OpenEdgeSettingsWindowCommand = new RelayCommand(() =>
        {
            var window = new EdgeSettingsWindow();
            window.ShowDialog();
        });
        OpenConvergenceWindowCommand = new RelayCommand(() =>
        {
            var window = new ConvergenceWindow();
            window.ShowDialog();
        });
        OpenMethodSettingsCommand = new RelayCommand(() =>
        {
            var window = new ParameterSettingsWindow();
            window.ShowDialog();
        });
        messenger.RegisterAll(this);
    }

    public ICommand ExitCommand { get; }

    public ICommand OpenEdgeSettingsWindowCommand { get; }

    public ICommand OpenConvergenceWindowCommand { get; }

    public ICommand OpenMethodSettingsCommand { get; }

    private bool isNotRunning;
    public bool IsNotRunning
    {
        get => isNotRunning;
        set => SetProperty(ref isNotRunning, value);
    }

    public void Receive(GraphUIState.ChangedMessage message) => IsNotRunning = !message.Value.IsInProgress;
}
