using Salesman.Domain.Graph;

namespace WpfUI.Data;

public class Pathfinder
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required ISalesmanPathfinder<int, int> Method { get; set; }
}
