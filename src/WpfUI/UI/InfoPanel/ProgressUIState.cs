using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfUI.UI.InfoPanel;

public record ProgressUIState(bool IsInProgress)
{
    public class ChangedMessage(ProgressUIState state) : ValueChangedMessage<ProgressUIState>(state);
    public class RequestMessage : RequestMessage<ProgressUIState>;
}
