using System;
using System.Collections.Generic;
using System.Linq;
using AXTanks.Scripts.Extensions;
using AXTanks.Scripts.Player;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class StartScene : Node
{
    [Export] private UiScene _uiScene;
    [Export] private Camera2D _camera2D;
    [Export] private GameMode _gameMode;
    [Export] private WorldScene _worldScene;
    [Export] private PlayerInput _playerInput;
    [Export] private PackedScene _tankViewScene;

    private List<BulletView> _bulletViews;
    private Dictionary<int, TankView> _tankViews;

    private const int _PORT = 12043;

    public override void _Ready()
    {
        _tankViews = new Dictionary<int, TankView>();
        _bulletViews = new List<BulletView>();
        _uiScene.Initialize(Host, Connect, TryStartGame);
        _uiScene.ShowClientServerMenu();
    }

    public void AddBullet(BulletView bulletView)
    {
        _bulletViews.Add(bulletView);
    }

    public void RemoveBullet(BulletView bulletView)
    {
        _bulletViews.Remove(bulletView);

        bulletView.Rpc(nameof(bulletView.Destroy));
    }

    public void RemoveAllBullets()
    {
        foreach (BulletView bulletView in _bulletViews)
        {
            bulletView.Rpc(nameof(bulletView.Destroy));
        }

        _bulletViews.Clear();
    }

    private void TryStartGame() => this.RpcServerOnly(nameof(RequestStartGame));

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestStartGame()
    {
        IReadOnlyDictionary<int, LobbyClientData> lobbyViewClientData = _uiScene.lobbyView.clientData;

        if (lobbyViewClientData.Any(data => !data.Value.isReady)) return;

        int seed = Random.Shared.Next(int.MaxValue);
        _worldScene.Rpc(nameof(_worldScene.InitializeMaze), seed);
        _worldScene.Rpc(nameof(_worldScene.GenerateMaze));

        foreach (KeyValuePair<int, LobbyClientData> clientData in lobbyViewClientData)
        {
            _uiScene.scorePanel.RequestCreateScoreInfoView(clientData.Value.id, clientData.Value.name,
                clientData.Value.color);
        }

        Rpc(nameof(ResponseStartGame));

        CreateTankViews();
        _gameMode.OnStartGame(lobbyViewClientData);
    }

    [Rpc]
    private void ResponseStartGame()
    {
        CreateTankViews();
        InitializeLocalPlayer(_tankViews[Multiplayer.GetUniqueId()]);
        _uiScene.HideAllMenus();
    }

    private void RestartGame()
    {
        AddScore();

        _worldScene.Rpc(nameof(_worldScene.ClearMaze));
        _worldScene.Rpc(nameof(_worldScene.GenerateMaze));
        RemoveAllBullets();

        foreach (KeyValuePair<int, TankView> tankView in _tankViews)
        {
            tankView.Value.Rpc(nameof(tankView.Value.Respawn), GetRandomPositionInMaze());
        }
    }

    private void AddScore()
    {
        GameClientData gameClientData = _gameMode.GetWinner();

        _uiScene.scorePanel.RequestUpdateView(gameClientData.id, gameClientData.score);
    }

    private Vector2 GetRandomPositionInMaze()
    {
        Vector2I mazeSize = _worldScene.GetMazeSize();

        return new Vector2(Random.Shared.Next(30, 30 + mazeSize.Y * 30),
            Random.Shared.Next(30, 30 + mazeSize.X * 30));
    }

    private void CreateTankViews()
    {
        IReadOnlyDictionary<int, LobbyClientData> clientDatas = _uiScene.lobbyView.clientData;

        foreach (KeyValuePair<int, LobbyClientData> clientData in clientDatas)
        {
            TankView tankView = _tankViewScene.Instantiate<TankView>();
            tankView.Initialize(Multiplayer.GetUniqueId() == clientData.Value.id, clientData.Value.color,
                () => OnClientDead(clientData.Value.id), AddBullet, RemoveBullet);
            if (Multiplayer.IsServer()) tankView.SubscribeHitBox();
            tankView.SetMultiplayerAuthority(clientData.Value.id);

            _tankViews.Add(clientData.Value.id, tankView);

            _worldScene.CallDeferredExt(nameof(_worldScene.AddChild), tankView, true);
        }
    }

    private void OnClientDead(int id)
    {
        if (!Multiplayer.IsServer()) return;

        _gameMode.OnClientDead(id);
    }

    private void InitializeLocalPlayer(TankView tankView)
    {
        _playerInput.MoveInputChanged += tankView.OnMoveInputChanged;
        _playerInput.ShootInputTriggered += tankView.OnShootInputTriggered;

        tankView.Position = GetRandomPositionInMaze();
    }

    private void ServerReady()
    {
        _gameMode.Initialize(RestartGame);
        _uiScene.HideAllMenus();
    }

    private void ClientReady()
    {
        _uiScene.ShowClientMenu();

        _uiScene.GetConnectedClientData();
        _uiScene.AddClientData();
        _uiScene.UpdateClientData();

        _uiScene.nicknameInput.TextChanged += _ => _uiScene.UpdateClientData();
        _uiScene.huePicker.ColorChanged += _ => _uiScene.UpdateClientData();
        _uiScene.readyCheckBox.Toggled += _ => _uiScene.UpdateClientData();
    }

    private void Connect()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateClient(_uiScene.addressInput.Text, _PORT);

        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.ConnectedToServer += ClientReady;
    }

    private void Host()
    {
        ENetMultiplayerPeer peer = new();

        peer.CreateServer(_PORT);

        Multiplayer.MultiplayerPeer = peer;
        ServerReady();
    }
}