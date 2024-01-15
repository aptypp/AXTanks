using Godot;

namespace AXTanks.Scripts;

public partial class Entry : Node
{
    [Export] private PackedScene uiScene;
    [Export] private PackedScene worldScene;

    public override void _Ready()
    {
        Node uiSceneInstance = uiScene.Instantiate();
        Node worldSceneInstance = worldScene.Instantiate();
        AddChild(worldSceneInstance);
        AddChild(uiSceneInstance);
    }
}