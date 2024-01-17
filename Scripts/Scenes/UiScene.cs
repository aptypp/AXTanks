using System;
using System.Text.Json;
using AXTanks.Scripts.Extensions;
using AXTanks.Scripts.Gui;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class UiScene : Control
{
    [Export] public CheckBox readyCheckBox { get; private set; }
    [Export] public LineEdit addressInput { get; private set; }
    [Export] public LineEdit nicknameInput { get; private set; }
    [Export] public LobbyView lobbyView { get; private set; }
    [Export] public HuePicker huePicker { get; private set; }


    [Export] private Button _hostButton;
    [Export] private Button _connectButton;
    [Export] private Button _startGameButton;

    public void Initialize(Action onHostButtonPressed, Action onConnectButtonPressed, Action onStartGameButtonPressed)
    {
        _hostButton.Pressed += onHostButtonPressed;
        _connectButton.Pressed += onConnectButtonPressed;
        _startGameButton.Pressed += onStartGameButtonPressed;
    }

    public void GetConnectedClientData()
    {
        lobbyView.RpcServerOnly(nameof(lobbyView.RequestGetConnectedClientData), Multiplayer.GetUniqueId());
    }

    public void AddClientData()
    {
        lobbyView.RpcServerOnly(nameof(lobbyView.RequestAddClientData), GetClientData());
    }

    public void UpdateClientData()
    {
        lobbyView.RpcServerOnly(nameof(lobbyView.RequestUpdateClientData), GetClientData());
    }

    private string GetClientData()
    {
        ClientData clientData = new ClientData();

        clientData.id = Multiplayer.GetUniqueId();
        clientData.color = huePicker.color;
        clientData.name = nicknameInput.Text;
        clientData.isReady = readyCheckBox.ButtonPressed;

        return JsonSerializer.Serialize(clientData);
    }
}