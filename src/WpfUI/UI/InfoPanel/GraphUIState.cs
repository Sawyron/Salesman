using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfUI.UI.InfoPanel;

public record GraphUIState(bool IsInProgress)
{
    public class ChangedMessage(GraphUIState state) : ValueChangedMessage<GraphUIState>(state);
    public class RequestMessage : RequestMessage<GraphUIState>;
}
