using System.Collections.Generic;
using System.Text.Json;
using AXTanks.Scripts.Scenes;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class LobbyView : Control
{
    public IReadOnlyDictionary<int, LobbyClientData> clientData => _clientData;

    [Export] private PackedScene _playerInfoViewScene;
    [Export] private VBoxContainer _infoContainer;

    private Dictionary<int, LobbyClientData> _clientData;
    private Dictionary<int, PlayerInfoView> _playerInfoViews;

    public override void _Ready()
    {
        _clientData = new Dictionary<int, LobbyClientData>();
        _playerInfoViews = new Dictionary<int, PlayerInfoView>();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestGetConnectedClientData(int id)
    {
        RpcId(id, nameof(ResponseGetConnectedClientData), JsonSerializer.Serialize(_clientData));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestUpdateClientData(string data)
    {
        Rpc(nameof(ResponseUpdateClientData), data);

        LobbyClientData lobbyClientData = JsonSerializer.Deserialize<LobbyClientData>(data);

        _clientData[lobbyClientData.id] = lobbyClientData;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestAddClientData(string data)
    {
        Rpc(nameof(ResponseAddClientData), data);

        LobbyClientData lobbyClientData = JsonSerializer.Deserialize<LobbyClientData>(data);

        _clientData.Add(lobbyClientData.id, lobbyClientData);
    }

    [Rpc]
    private void ResponseGetConnectedClientData(string data)
    {
        _clientData = JsonSerializer.Deserialize<Dictionary<int, LobbyClientData>>(data);

        foreach (KeyValuePair<int, LobbyClientData> clientData in _clientData)
        {
            CreatePlayerInfoView(clientData.Value);
        }
    }

    [Rpc]
    public void ResponseUpdateClientData(string data)
    {
        LobbyClientData lobbyClientData = JsonSerializer.Deserialize<LobbyClientData>(data);

        _clientData[lobbyClientData.id] = lobbyClientData;
        _playerInfoViews[lobbyClientData.id].UpdateView(lobbyClientData);
    }


    [Rpc]
    public void ResponseAddClientData(string data)
    {
        LobbyClientData lobbyClientData = JsonSerializer.Deserialize<LobbyClientData>(data);

        _clientData.Add(lobbyClientData.id, lobbyClientData);

        CreatePlayerInfoView(lobbyClientData);
    }

    private void CreatePlayerInfoView(LobbyClientData lobbyClientData)
    {
        PlayerInfoView instance = _playerInfoViewScene.Instantiate<PlayerInfoView>();

        _infoContainer.AddChild(instance, true);

        instance.UpdateView(lobbyClientData);

        _playerInfoViews.Add(lobbyClientData.id, instance);
    }
}