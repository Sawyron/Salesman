using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfUI.UI.Convergence;

public record ConvergenceResult(List<double> Times, List<int> Values)
{
    public class ChangedMessage(ConvergenceResult result) : ValueChangedMessage<ConvergenceResult>(result);
}
