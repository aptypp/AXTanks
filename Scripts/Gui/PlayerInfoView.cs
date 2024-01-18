using AXTanks.Scripts.Scenes;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class PlayerInfoView : Node
{
    [Export] private Label _name;
    [Export] private ColorRect _color;
    [Export] private ReadyState _readyState;

    public void UpdateView(LobbyClientData lobbyClientData)
    {
        _name.Text = lobbyClientData.name;
        _color.Color = lobbyClientData.color;
        
        if (lobbyClientData.isReady) _readyState.ToReadyState();
        else _readyState.ToNotReadyState();
    }

}