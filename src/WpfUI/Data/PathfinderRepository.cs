namespace WpfUI.Data;

public class PathfinderRepository
{
    private readonly List<Pathfinder> _pathfinders;

    public PathfinderRepository(IEnumerable<Pathfinder> pathfinders)
    {
        _pathfinders = pathfinders.ToList();
    }

    public IEnumerable<Pathfinder> GetAll() => _pathfinders;
}
