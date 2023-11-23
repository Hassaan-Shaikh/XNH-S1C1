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
        switch (Xalkomak.difficulty)
        {
            case Xalkomak.Difficulty.Normal:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksSpeedBoost(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    return; // Sammy does not use the power rune on Normal mode
                }
                break;
            case Xalkomak.Difficulty.Hard:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksSpeedBoost(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    body.Call("SetSpeedBoost", true);
                    Xalkomak.isSpeedBoostCollectedBySammy = true;
                    gameRoot.StartSpeedBoostTimer();
                    QueueFree();
                }
                break;
            default:
                break;
        }
    }

    void PlayerPicksSpeedBoost(Node3D body)
    {
        body.Call("SetSpeedBoost", true);
        Xalkomak.isSpeedBoostCollected = true;
        gameRoot.StartSpeedBoostTimer();
        QueueFree();
    }
}
