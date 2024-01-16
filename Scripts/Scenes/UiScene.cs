using System;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class UiScene : Control
{
    [Export] public LineEdit addressInput { get; private set; }
    [Export] public LineEdit nicknameInput { get; private set; }

    [Export] private Button _hostButton;
    [Export] private Button _connectButton;
    [Export] private Button _startGameButton;

    public void Initialize(Action onHostButtonPressed, Action onConnectButtonPressed, Action onStartGameButtonPressed)
    {
        _hostButton.Pressed += onHostButtonPressed;
        _connectButton.Pressed += onConnectButtonPressed;
        _startGameButton.Pressed += onStartGameButtonPressed;
    }
}