using Salesman.Domain.Graph;

namespace WpfUI.Common;

public class ReportingPathfinder
{
    public string Name { get; set; } = string.Empty;
    public required IReportingSalesmanPathfinder<int, int> Metgod { get; set; }
}
