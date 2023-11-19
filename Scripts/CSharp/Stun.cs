using Godot;
using System;

public partial class Stun : Area3D
{
    [Signal]
    public delegate void StunEnemyEventHandler();

    [Export] public GameControl gameRoot;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameRoot = GetTree().GetNodesInGroup("Game")[0] as GameControl;
    }

    private void OnStunPickup(Node3D body)
    {
        switch (Xalkomak.difficulty)
        {
            case Xalkomak.Difficulty.Normal:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksStun(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    return; // Sammy does not use the power rune on Normal mode
                }
                break;
            case Xalkomak.Difficulty.Hard:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksStun(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    //body.Call("SetStun", true);
                    Xalkomak.isStunCollectedBySammy = true;
                    gameRoot.StartStunTimer();
                    QueueFree();
                }
                break;
            default:
                break;
        }
    }

    void PlayerPicksStun(Node3D body)
    {
        body.Call("SetStun", true);
        Xalkomak.isStunCollected = true;
        gameRoot.StartStunTimer();
        QueueFree();
    }
}
