using Godot;
using System;

public partial class Footstep : AudioStreamPlayer
{
    [Export] public Node3D footstepTarget;
    [Export] public Node3D footstepCheck;
    [Export] float distanceThreshold = 3.0f;

    public Player parent;

    bool playFootsteps;

    public override void _Ready()
    {
        base._Ready();
        parent = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        distanceThreshold = Xalkomak.isSpeedBoostCollected ? 2.0f : 3.0f;
        if(CheckDistance() && parent.IsOnFloor())
        {
            footstepCheck.GlobalPosition = footstepTarget.GlobalPosition;
            Play();
        }
    }

    private bool CheckDistance()
    {
        return footstepTarget.GlobalPosition.DistanceTo(footstepCheck.GlobalPosition) > distanceThreshold;
    }
}
