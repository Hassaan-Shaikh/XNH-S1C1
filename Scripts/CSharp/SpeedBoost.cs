using Godot;
using System;

public partial class SpeedBoost : Area3D
{
    [Export] public GameControl gameRoot;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameRoot = GetTree().GetNodesInGroup("Game")[0] as GameControl;
    }

    private void OnSpeedBoostPickup(Node3D body)
    {
        if(body.IsInGroup("Player"))
        {
            body.Call("SetSpeedBoost", true);
            Xalkomak.isSpeedBoostCollected = true;
            gameRoot.StartSpeedBoostTimer();
            QueueFree();
        }
    }
}
