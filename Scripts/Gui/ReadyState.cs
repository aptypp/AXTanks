using Godot;

namespace AXTanks.Scripts.Gui;

public partial class ReadyState : Node
{
    [Export] private Label _readyState;
    [Export] private Label _notReadyState;

    public void ToReadyState()
    {
        _readyState.Show();
        _notReadyState.Hide();
    }

    public void ToNotReadyState()
    {
        _readyState.Hide();
        _notReadyState.Show();
    }
}