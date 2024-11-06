using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Salesman.Domain.Pathfinders.SimulatedAnnealing;
using System.ComponentModel.DataAnnotations;
using WpfUI.Common;

namespace WpfUI.UI.ParameterSettings.SimulatedAnnealing;

public class SimulatedAnnealingParametersViewModel : ObservableValidator
{
    private readonly Store<SimulatedAnnealingParameters> _store;

    public SimulatedAnnealingParametersViewModel(Store<SimulatedAnnealingParameters> store)
    {
        _store = store;
        InitialTemperature = store.Value.InitialTemperature;
        MinTemperature = store.Value.MinTemperature;
    }

    private double _initialTemperature;

    [Range(0, double.MaxValue)]
    public double InitialTemperature
    {
        get => _initialTemperature;
        set
        {
            if (TrySetProperty(ref _initialTemperature, value, out _))
            {
                _store.Value = _store.Value with { InitialTemperature = value };
            }
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
