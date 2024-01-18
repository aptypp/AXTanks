using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class GameMode : Node
{
    private Action _restartGame;
    private Dictionary<int, GameClientData> _gameClientData;

    public override void _Ready()
    {
        _gameClientData = new Dictionary<int, GameClientData>();
    }

    public void Initialize(Action restartGame)
    {
        _restartGame = restartGame;
    }

    public void OnStartGame(IReadOnlyDictionary<int, LobbyClientData> lobbyClientData)
    {
        foreach (KeyValuePair<int, LobbyClientData> clientData in lobbyClientData)
        {
            GameClientData gameClientData = new();
            gameClientData.id = clientData.Value.id;
            gameClientData.score = 0;
            gameClientData.isDead = false;

            _gameClientData.Add(gameClientData.id, gameClientData);
        }
    }

    public GameClientData GetWinner() => _gameClientData.First(data => !data.Value.isDead).Value;

    public void OnClientDead(int id)
    {
        GameClientData gameClientData = _gameClientData[id];
        gameClientData.isDead = true;

        _gameClientData[id] = gameClientData;

        if (_gameClientData.Count(data => !data.Value.isDead) > 1) return;

        OnClientAddScore(GetWinner().id);

        _restartGame();

        foreach (KeyValuePair<int, GameClientData> clientData in _gameClientData)
        {
            ClientAlive(clientData.Value.id);
        }
    }

    private void ClientAlive(int id)
    {
        GameClientData gameClientData = _gameClientData[id];
        gameClientData.isDead = false;

        _gameClientData[id] = gameClientData;
    }

    private void OnClientAddScore(int id)
    {
        GameClientData gameClientData = _gameClientData[id];
        gameClientData.score++;

        _gameClientData[id] = gameClientData;
    }
}