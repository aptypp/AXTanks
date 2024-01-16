using Godot;

namespace AXTanks.Scripts.Gui;

public partial class Ui : Control
{
    [Export] private Node2D _world;
    [Export] private LineEdit _addressInput;
    [Export] private PackedScene _worldScene;
    [Export] private PackedScene _tankScene;

    private const int _PORT = 12034;

    public override void _Ready()
    {
        Multiplayer.PeerConnected += MultiplayerOnPeerConnected;
        Multiplayer.ConnectedToServer += MultiplayerOnConnectedToServer;
    }

    private void MultiplayerOnConnectedToServer()
    {
    }

    private void MultiplayerOnPeerConnected(long id)
    {
        if (!Multiplayer.IsServer()) return;

        //Node tankInstance = _tankScene.Instantiate<TankView>();

        //GetParent().AddChild(tankInstance);
    }

    private void _on_host_button_pressed()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateServer(_PORT);

        if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting)
        {
            GD.Print("error: server unable to connect");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        StartGame();
    }

    private void _on_connect_button_pressed()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateClient(_addressInput.Text, _PORT);

        if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected)
        {
            GD.Print("error: client unable to connect");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        StartGame();
    }

    private void StartGame()
    {
        Hide();

        if (!Multiplayer.IsServer()) return;

        Node instance = _worldScene.Instantiate();
        _world.AddChild(instance);
    }
}