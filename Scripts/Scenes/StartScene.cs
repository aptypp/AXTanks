using System;
using AXTanks.Scripts.Player;
using Godot;
using Godot.Collections;

namespace AXTanks.Scripts.Scenes;

public partial class StartScene : Node
{
    [Export] private UiScene _uiScene;
    [Export] private Camera2D _camera2D;
    [Export] private WorldScene _worldScene;
    [Export] private PlayerInput _playerInput;
    [Export] private PackedScene _tankViewScene;

    private const int _PORT = 12043;

    public override void _Ready()
    {
        _uiScene.Initialize(Host, Connect, StartGame);
    }

    private void StartGame()
    {
        if (!Multiplayer.IsServer()) return;
        
        int seed = Random.Shared.Next(int.MaxValue);
        _worldScene.Rpc(nameof(_worldScene.GenerateMaze), seed);

        int[] peers = Multiplayer.GetPeers();

        for (int peerIndex = 0; peerIndex < peers.Length; peerIndex++)
        {
            SpawnPlayer(peers[peerIndex]);
        }

        for (int peerIndex = 0; peerIndex < peers.Length; peerIndex++)
        {
            RpcId(peers[peerIndex], nameof(InitializeLocalPlayer), peers[peerIndex]);
        }
    }

    private void SpawnPlayer(int peerId)
    {
        TankView tankView = _tankViewScene.Instantiate<TankView>();

        _worldScene.AddChild(tankView, true);

        tankView.Rpc(nameof(tankView.Initialize), peerId);
        tankView.Position = new Vector2(500, -500);
    }

    [Rpc]
    private void InitializeLocalPlayer(int peerId)
    {
        Array<Node> children = _worldScene.GetChildren();

        foreach (Node child in children)
        {
            if (child is not TankView tankView) continue;
            if (tankView.id != peerId) continue;

            _playerInput.MoveInputChanged += tankView.OnMoveInputChanged;
            _playerInput.ShootInputTriggered += tankView.OnShootInputTriggered;
        }
    }

    public override void _Process(double delta)
    {
    }

    private void ServerReady()
    {
    }

    private void ClientReady()
    {
    }

    private void Connect()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateClient(_uiScene.addressInput.Text, _PORT);

        Multiplayer.MultiplayerPeer = peer;
        ClientReady();
    }

    private void Host()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateServer(_PORT);

        Multiplayer.MultiplayerPeer = peer;
        ServerReady();
    }
}