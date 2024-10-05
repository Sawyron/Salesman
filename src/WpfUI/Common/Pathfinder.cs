using Salesman.Domain.Graph;

namespace WpfUI.Common;

public class Pathfinder
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required ISalesmanPathfinder<int, int> Method { get; set; }
}
