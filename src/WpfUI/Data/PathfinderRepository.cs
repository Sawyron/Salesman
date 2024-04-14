namespace WpfUI.Data;

public class PathfinderRepository
{
    private readonly List<Pathfinder> _pathfinders;
    private readonly List<ReportingPathfinder> _reportingPathfinders;

    public PathfinderRepository(
        IEnumerable<Pathfinder> pathfinders,
        IEnumerable<ReportingPathfinder> reportingPathfinders)
    {
        _pathfinders = pathfinders.ToList();
        _reportingPathfinders = reportingPathfinders.ToList();
    }

    public IEnumerable<Pathfinder> GetAll() => _pathfinders;

    public IEnumerable<ReportingPathfinder> GetReportingPathfinders() => _reportingPathfinders;
}
