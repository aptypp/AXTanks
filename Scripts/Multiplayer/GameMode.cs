using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class GameMode : Node
{
    private Action<GameClientData> _restartGame;
    private Dictionary<int, GameClientData> _gameClientData;

    public override void _Ready()
    {
        _gameClientData = new Dictionary<int, GameClientData>();
    }

    public void Initialize(Action<GameClientData> restartGame)
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
            gameClientData.bulletsCount = lobbyClientData.Count;

            _gameClientData.Add(gameClientData.id, gameClientData);
        }
    }

    public bool IsAnyoneAlive() => _gameClientData.Count(data => !data.Value.isDead) > 0;
    public GameClientData GetWinner() => _gameClientData.First(data => !data.Value.isDead).Value;

    public GameClientData GetData(int clientId) => _gameClientData[clientId];

    public async void OnClientDead(int id)
    {
        GameClientData gameClientData = _gameClientData[id];
        gameClientData.isDead = true;

        _gameClientData[id] = gameClientData;

        if (_gameClientData.Count(data => !data.Value.isDead) != 1) return;

        await Task.Delay(2500);

        GameClientData winnerData = new();
        winnerData.id = -1;

        if (IsAnyoneAlive())
        {
            winnerData = OnClientAddScore(GetWinner().id);
        }

        _restartGame(winnerData);
    }

    public bool CanClientShoot(int id) => _gameClientData[id].bulletsCount > 0;

    public void DecreaseClientBullet(int id)
    {
        GameClientData gameClientData = _gameClientData[id];

        gameClientData.bulletsCount--;

        _gameClientData[id] = gameClientData;
    }

    public void AddClientBullet(int id)
    {
        GameClientData gameClientData = _gameClientData[id];

        gameClientData.bulletsCount++;

        _gameClientData[id] = gameClientData;
    }

    public void ResetGame()
    {
        foreach (int clientId in _gameClientData.Keys)
        {
            GameClientData gameClientData = _gameClientData[clientId];

            gameClientData.bulletsCount = _gameClientData.Count;
            gameClientData.isDead = false;

            _gameClientData[clientId] = gameClientData;
        }
    }

    private GameClientData OnClientAddScore(int id)
    {
        GameClientData gameClientData = _gameClientData[id];
        gameClientData.score++;

        _gameClientData[id] = gameClientData;

        return gameClientData;
    }
}