using System;
using Godot;

namespace AXTanks.Scripts.Gui;

public partial class HuePicker : Node
{
    public Color color => _colorRect.Color;
    
    public event Action<Color> ColorChanged;

    [Export] private Color _baseColor;
    [Export] private HSlider _slider;
    [Export] private ColorRect _colorRect;

    public override void _Ready()
    {
        _slider.ValueChanged += SliderOnValueChanged;
    }

    private void SliderOnValueChanged(double value)
    {
        Color color = _colorRect.Color;
        color.H = (float)value;
        _colorRect.Color = color;

        ColorChanged?.Invoke(color);
    }
}