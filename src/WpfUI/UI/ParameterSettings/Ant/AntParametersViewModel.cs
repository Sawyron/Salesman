using CommunityToolkit.Mvvm.ComponentModel;
using Salesman.Domain.Pathfinders.Ant;
using System.ComponentModel.DataAnnotations;
using WpfUI.Common;

namespace WpfUI.UI.ParameterSettings.Ant;

public class AntParametersViewModel : ObservableValidator
{
    private readonly Store<AntParameters> _store;

    public AntParametersViewModel(Store<AntParameters> store)
    {
        _store = store;
        AntParameters parameters = store.Value;
        _alpha = parameters.Alpha;
        _beta = parameters.Beta;
        _p = parameters.P;
        _q = parameters.Q;
        _initialPheromone = parameters.InitialPheromone;
        _iterations = parameters.IterationsWithoutImprovementsThreshold;
    }

    private double _alpha;
    public double Alpha
    {
        get => _alpha;
        set
        {
            SetProperty(ref _alpha, value);
            _store.Value = _store.Value with { Alpha = value };
        }
    }

    private double _beta;
    public double Beta
    {
        get => _beta;
        set
        {
            SetProperty(ref _beta, value);
            _store.Value = _store.Value with { Beta = value };
        }
    }

    private double _p;

    [Range(0, 1)]
    public double P
    {
        get => _p;
        set
        {
            SetProperty(ref _p, value, true);
            _store.Value = _store.Value with { P = value };
        }
    }

    private double _q;
    public double Q
    {
        get => _q;
        set
        {
            SetProperty(ref _q, value);
            _store.Value = _store.Value with { Q = value };
        }
    }

    private double _initialPheromone;

    [Range(1e-10, double.MaxValue)]
    public double InitialPheromone
    {
        get => _initialPheromone;
        set
        {
            SetProperty(ref _initialPheromone, value, true);
            _store.Value = _store.Value with { InitialPheromone = value };
        }
    }

    private int _iterations;

    [Range(1, int.MaxValue)]
    public int Iterations
    {
        get => _iterations;
        set
        {
            SetProperty(ref _iterations, value, true);
            _store.Value = _store.Value with { IterationsWithoutImprovementsThreshold = value };
        }
    }
}
