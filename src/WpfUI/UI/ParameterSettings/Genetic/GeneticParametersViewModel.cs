using CommunityToolkit.Mvvm.ComponentModel;
using Salesman.Domain.Pathfinders.Genetic;
using System.ComponentModel.DataAnnotations;
using WpfUI.Common;

namespace WpfUI.UI.ParameterSettings.Genetic;

public class GeneticParametersViewModel : ObservableValidator
{
    private readonly Store<GeneticParameters> _store;

    public GeneticParametersViewModel(Store<GeneticParameters> store)
    {
        _store = store;
        _mutationProbability = store.Value.MutationProbability;
        _iterations = store.Value.Iterations;
    }

    private double _mutationProbability;

    [Range(0, 1)]
    public double MutationProbability
    {
        get => _mutationProbability;
        set
        {
            SetProperty(ref _mutationProbability, value, true);
            _store.Value = _store.Value with { MutationProbability = value };
        }
    }

    private int _iterations;

    [Range(0, int.MaxValue)]
    public int Iterations
    {
        get => _iterations;
        set
        {
            SetProperty(ref _iterations, value, true);
            _store.Value = _store.Value with { Iterations = value };
        }
    }
}
