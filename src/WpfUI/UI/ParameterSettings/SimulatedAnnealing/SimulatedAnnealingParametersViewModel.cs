using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Pathfinders.SimulatedAnnealing;
using WpfUI.Common;

namespace WpfUI.UI.ParameterSettings.SimulatedAnnealing;

public class SimulatedAnnealingParametersViewModel : ObservableObject
{
    private readonly Store<SimulatedAnnealingParameters> _store;

    public SimulatedAnnealingParametersViewModel(Store<SimulatedAnnealingParameters> store)
    {
        _store = store;
        InitialTemperature = store.Value.InitialTemperature;
        MinTemperature = store.Value.MinTemperature;
    }

    private double _initialTemperature;
	public double InitialTemperature
    {
        get => _initialTemperature;
        set
        {
            _store.Value = _store.Value with { InitialTemperature = value };
            SetProperty(ref _initialTemperature, value);
        }
    }

    private double _minTemperature;

    public double MinTemperature
    {
        get => _minTemperature;
        set
        {
            _store.Value = _store.Value with { MinTemperature = value };
            SetProperty(ref _minTemperature, value);
        }
    }
}
