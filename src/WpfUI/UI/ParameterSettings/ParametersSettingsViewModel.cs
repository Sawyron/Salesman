using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfUI.Navigation;
using WpfUI.UI.ParameterSettings.SimulatedAnnealing;

namespace WpfUI.UI.ParameterSettings;

public class ParametersSettingsViewModel : ObservableObject
{
    public ParametersSettingsViewModel(NavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigateToSa = new RelayCommand(() =>
        {
            NavigationService.NavigateTo<SimulatedAnnealingParametersViewModel>();
        });
    }

    public NavigationService NavigationService { get; }
    public IRelayCommand NavigateToSa { get; }
}
