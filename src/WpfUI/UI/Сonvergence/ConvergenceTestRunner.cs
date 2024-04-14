using Salesman.Domain.Graph;
using System.Diagnostics;

namespace WpfUI.UI.Сonvergence;
public class ConvergenceTestRunner
{
    private readonly IRepotingSalesmanPathfinder<int, int> _pathfinder;
    private readonly int _periodInMs;
    private readonly IProgress<int> _progress;
    private int _lastValue = 0;
    private int _lastTime = 0;
    private readonly Stopwatch _sw = new();

    public ConvergenceTestRunner(
        IRepotingSalesmanPathfinder<int, int> pathfinder,
        int periodInMs)
    {
        _pathfinder = pathfinder;
        _periodInMs = periodInMs;
        _progress = new Progress<int>(value =>
        {
            if (_sw.ElapsedMilliseconds - _lastTime < periodInMs)
            {
                return;
            }
            _lastValue = value;
        });
    }

    public async Task<List<(int Time, int Value)>> RunTestAsync(int timeInMs)
    {
        var result = new List<(int Time, int Value)>(timeInMs / _periodInMs);
        var sw = new Stopwatch();
        throw new NotImplementedException();
    }
}
