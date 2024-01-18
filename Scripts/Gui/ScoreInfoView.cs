using Godot;

namespace AXTanks.Scripts.Gui;

public partial class ScoreInfoView : Node
{
    [Export] private Label _name;
    [Export] private Label _score;
    [Export] private ColorRect _colorRect;

    public void Initialize(string name, Color color)
    {
        _name.Text = name;
        _colorRect.Color = color;
    }

    public void SetScore(int score)
    {
        _score.Text = $"{score}";
    }
}