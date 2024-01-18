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
    [Export] public ScorePanel scorePanel { get; private set; }
    [Export] public HuePicker huePicker { get; private set; }


    [Export] private Button _hostButton;
    [Export] private Button _connectButton;
    [Export] private Button _startGameButton;
    [Export] private Control _clientServerMenu { get; set; }
    [Export] private Control _clientMenu { get; set; }

    public void Initialize(Action onHostButtonPressed, Action onConnectButtonPressed, Action onStartGameButtonPressed)
    {
        _hostButton.Pressed += onHostButtonPressed;
        _connectButton.Pressed += onConnectButtonPressed;
        _startGameButton.Pressed += onStartGameButtonPressed;
    }

    public void ShowClientServerMenu()
    {
        _clientServerMenu.Show();
        _clientMenu.Hide();
        lobbyView.Hide();
    }

    public void ShowClientMenu()
    {
        _clientServerMenu.Hide();
        _clientMenu.Show();
        lobbyView.Show();
    }

    public void HideAllMenus()
    {
        _clientServerMenu.Hide();
        _clientMenu.Hide();
        lobbyView.Hide();
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
        LobbyClientData lobbyClientData = new LobbyClientData();

        lobbyClientData.id = Multiplayer.GetUniqueId();
        lobbyClientData.color = huePicker.color;
        lobbyClientData.name = nicknameInput.Text;
        lobbyClientData.isReady = readyCheckBox.ButtonPressed;

        return JsonSerializer.Serialize(lobbyClientData);
    }
}