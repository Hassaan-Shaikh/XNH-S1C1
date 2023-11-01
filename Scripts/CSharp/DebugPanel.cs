using Godot;
using System;

public partial class DebugPanel : PanelContainer
{
    [Export] public Label FPS;
    [Export] public Label playerSpeed;
    [Export] public Player parent;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        parent = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("debug"))
        {
            Visible = !Visible;
        }
        if(!Visible)
        {
            return;
        }
        FPS.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
        playerSpeed.Text = "Player's Speed: " + parent.GetPlayerSpeed();
    }
}
