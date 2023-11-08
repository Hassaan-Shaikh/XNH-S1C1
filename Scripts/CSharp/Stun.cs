using Godot;
using System;

public partial class Stun : Area3D
{
    [Export] public GameControl gameRoot;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameRoot = GetTree().GetNodesInGroup("Game")[0] as GameControl;
    }

    private void OnStunPickup(Node3D body)
    {
        if (body.IsInGroup("Player"))
        {
            body.Call("SetStun", true);
            Xalkomak.isStunCollected = true;
            gameRoot.StartStunTimer();
            QueueFree();
        }
    }
}
