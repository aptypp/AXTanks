using System.Collections.Generic;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class ScorePanel : Node
{
    [Export] private PackedScene _scoreInfoViewScene;
    [Export] private VBoxContainer _scoreContainer;

    private Dictionary<int, ScoreInfoView> _scoreInfoViews;

    public override void _Ready()
    {
        _scoreInfoViews = new Dictionary<int, ScoreInfoView>();
    }

    public void RequestUpdateView(int id, int score)
    {
        Rpc(nameof(ResponseUpdateView), id, score);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestCreateScoreInfoView(int id, StringName name)
    {
        Rpc(nameof(ResponseCreateScoreInfoView), id, name);
    }

    [Rpc]
    private void ResponseUpdateView(int id, int score)
    {
        _scoreInfoViews[id].SetScore(score);
    }

    [Rpc]
    private void ResponseCreateScoreInfoView(int id, StringName name)
    {
        CreateScoreInfoView(id, name);
    }

    private void CreateScoreInfoView(int id, string name)
    {
        ScoreInfoView instance = _scoreInfoViewScene.Instantiate<ScoreInfoView>();

        _scoreContainer.AddChild(instance, true);

        instance.Initialize(name);
        instance.SetScore(0);

        _scoreInfoViews.Add(id, instance);
    }
}