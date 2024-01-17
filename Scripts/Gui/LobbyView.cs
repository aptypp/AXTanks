using System.Collections.Generic;
using System.Text.Json;
using AXTanks.Scripts.Scenes;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class LobbyView : Node
{
    [Export] private PackedScene _playerInfoViewScene;
    [Export] private VBoxContainer _infoContainer;

    private Dictionary<int, ClientData> _clientData;
    private Dictionary<int, PlayerInfoView> _playerInfoViews;

    public override void _Ready()
    {
        _clientData = new Dictionary<int, ClientData>();
        _playerInfoViews = new Dictionary<int, PlayerInfoView>();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestGetConnectedClientData(int id)
    {
        RpcId(id, nameof(ResponseGetConnectedClientData), JsonSerializer.Serialize(_clientData));

        foreach (KeyValuePair<int, ClientData> clientData in _clientData)
        {
            GD.Print($"{clientData.Value.id}_{clientData.Value.name}");
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestUpdateClientData(string data)
    {
        Rpc(nameof(ResponseUpdateClientData), data);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestAddClientData(string data)
    {
        Rpc(nameof(ResponseAddClientData), data);

        ClientData clientData = JsonSerializer.Deserialize<ClientData>(data);

        _clientData.Add(clientData.id, clientData);
    }

    [Rpc]
    private void ResponseGetConnectedClientData(string data)
    {
        _clientData = JsonSerializer.Deserialize<Dictionary<int, ClientData>>(data);

        foreach (KeyValuePair<int, ClientData> clientData in _clientData)
        {
            CreatePlayerInfoView(clientData.Value);
        }
    }

    [Rpc]
    public void ResponseUpdateClientData(string data)
    {
        ClientData clientData = JsonSerializer.Deserialize<ClientData>(data);

        _clientData[clientData.id] = clientData;
        _playerInfoViews[clientData.id].UpdateView(clientData);
    }


    [Rpc]
    public void ResponseAddClientData(string data)
    {
        ClientData clientData = JsonSerializer.Deserialize<ClientData>(data);

        _clientData.Add(clientData.id, clientData);

        CreatePlayerInfoView(clientData);
    }

    private void CreatePlayerInfoView(ClientData clientData)
    {
        PlayerInfoView instance = _playerInfoViewScene.Instantiate<PlayerInfoView>();

        _infoContainer.AddChild(instance, true);

        instance.UpdateView(clientData);

        _playerInfoViews.Add(clientData.id, instance);
    }
}