using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using WpfUI.Common;
using WpfUI.UI.Main;

namespace WpfUI.UI.EdgeSettings;

public class EdgeSettingsViewModel : ObservableObject, IRecipient<GraphUIState.ChangedMessage>
{
    private readonly GraphHolder _graphHolder;

    public ObservableCollection<Edge> Edges => _graphHolder.Edges;

    public EdgeSettingsViewModel(GraphHolder graphHolder, IMessenger messenger)
    {
        _graphHolder = graphHolder;
        IsReadOnly = messenger.Send<GraphUIState.RequestMessage>().Response.IsInProgress;
        messenger.RegisterAll(this);
        UnsubscribeCommand = new RelayCommand(() =>
        {
            messenger.UnregisterAll(this);
        });
    }

    private bool _isReadOnly;
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }

    public IRelayCommand UnsubscribeCommand { get; }

    public void Receive(GraphUIState.ChangedMessage message)
    {
        IsReadOnly = message.Value.IsInProgress;
    }
}
