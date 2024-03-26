using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfUI.UI.Main;
public record GraphUIState(bool IsInProgress)
{
    public class GrapgUIStateChangedMessage(GraphUIState state) : ValueChangedMessage<GraphUIState>(state);
}
