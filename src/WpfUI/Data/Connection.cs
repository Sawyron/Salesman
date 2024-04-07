namespace WpfUI.UI.Graph;
public class Connection
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public int FromNodeId { get; set; }
    public int ToNodeId { get; set; }
}
