﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using WpfUI.Data;
using WpfUI.UI.Graph;
using WpfUI.UI.Main;

namespace WpfUI.UI.EdgeSettings;
public class EdgeSettingsViewModel
    : ObservableObject, IRecipient<GraphUIState.GrapgUIStateChangedMessage>
{
    private readonly GraphHolder _graphHolder;

    public ObservableCollection<Edge> Edges => _graphHolder.Edges;


    public EdgeSettingsViewModel(GraphHolder graphHolder, IMessenger messenger)
    {
        _graphHolder = graphHolder;
        messenger.RegisterAll(this);
        UnsubscribeCommand = new RelayCommand(() =>
        {
            messenger.UnregisterAll(this);
        });
    }

    private bool _isReadOnly = false;
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }

    public IRelayCommand UnsubscribeCommand { get; }

    public void Receive(GraphUIState.GrapgUIStateChangedMessage message)
    {
        IsReadOnly = message.Value.IsInProgress;
    }
}