using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfUI.UI.Сonvergence;

public record ConvergenceResult(List<double> Times, List<int> Values)
{
    public class ConvergenceResultChangedMessage(ConvergenceResult result) : ValueChangedMessage<ConvergenceResult>(result);
}
