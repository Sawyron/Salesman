using Salesman.Domain.Graph;

namespace WpfUI.Data;

public class ReportingPathfinder
{
    public string Name { get; set; } = string.Empty;
    public required IRepotingSalesmanPathfinder<int, int> Metgod { get; set; }
}
