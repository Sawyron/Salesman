using CommunityToolkit.Mvvm.ComponentModel;
using Salesman.Domain.Pathfinders.RandomSearch;
using System.ComponentModel.DataAnnotations;
using WpfUI.Common;

namespace WpfUI.UI.ParameterSettings.RandomSearch;

public class RandomSearchParametersViewModel : ObservableValidator
{
    private readonly Store<RandomSearchParameters> _parametersStore;

    public RandomSearchParametersViewModel(Store<RandomSearchParameters> parametersStore)
    {
        _parametersStore = parametersStore;
        _iterations = parametersStore.Value.Iterations;
    }

    private int _iterations;

    [Range(0, int.MaxValue)]
    public int Iterations
    {
        get => _iterations;
        set
        {
            if (TrySetProperty(ref _iterations, value, out _))
            {
                _parametersStore.Value = _parametersStore.Value with { Iterations = value };
            }
        }
    }
}
