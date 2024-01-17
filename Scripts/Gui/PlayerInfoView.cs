using AXTanks.Scripts.Scenes;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class PlayerInfoView : Node
{
    [Export] private Label _name;
    [Export] private ColorRect _color;
    [Export] private ReadyState _readyState;

    public void UpdateView(ClientData clientData)
    {
        _name.Text = clientData.name;
        _color.Color = clientData.color;
        
        if (clientData.isReady) _readyState.ToReadyState();
        else _readyState.ToNotReadyState();
    }

}