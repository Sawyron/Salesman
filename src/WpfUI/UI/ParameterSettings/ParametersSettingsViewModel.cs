using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfUI.Navigation;
using WpfUI.UI.ParameterSettings.Ant;
using WpfUI.UI.ParameterSettings.Genetic;
using WpfUI.UI.ParameterSettings.SimulatedAnnealing;

namespace WpfUI.UI.ParameterSettings;

public class ParametersSettingsViewModel : ObservableObject
{
    public ParametersSettingsViewModel(NavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigateToSa = new RelayCommand(() => NavigationService.NavigateTo<SimulatedAnnealingParametersViewModel>());
        NavigateToAnt = new RelayCommand(() => NavigationService.NavigateTo<AntParametersViewModel>());
        NavigateToGenetic = new RelayCommand(() => NavigationService.NavigateTo<GeneticParametersViewModel>());
    }

    public NavigationService NavigationService { get; }
    public IRelayCommand NavigateToSa { get; }
    public IRelayCommand NavigateToAnt { get; }
    public IRelayCommand NavigateToGenetic { get; }
}
