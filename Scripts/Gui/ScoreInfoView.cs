using Godot;

namespace AXTanks.Scripts.Gui;

public partial class ScoreInfoView : Node
{
    [Export] private Label _name;
    [Export] private Label _score;

    public void Initialize(string name)
    {
        _name.Text = name;
    }

    public void SetScore(int score)
    {
        _score.Text = $"{score}";
    }
}