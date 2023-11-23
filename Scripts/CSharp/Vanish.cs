using Godot;
using System;

public partial class Vanish : Area3D
{
    [Export] public GameControl gameRoot;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameRoot = GetTree().GetNodesInGroup("Game")[0] as GameControl;
    }

    private void OnVanishPickup(Node3D body)
    {
        switch(Xalkomak.difficulty)
        {
            case Xalkomak.Difficulty.Normal:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksVanish(body);
                }
                else if(body.IsInGroup("Monster"))
                {
                    return; // Sammy does not use the power rune on Normal mode
                }
                break;
            case Xalkomak.Difficulty.Hard:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksVanish(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    body.Call("GoInvisible", true);
                    Xalkomak.isVanishCollectedBySammy = true;
                    gameRoot.StartVanishTimer();
                    QueueFree();
                }
                break;
            default:
                break;
        }
    }

    void PlayerPicksVanish(Node3D body)
    {
        body.Call("SetVanish", true);
        Xalkomak.isVanishCollected = true;
        gameRoot.StartVanishTimer();
        QueueFree();
    }
}
