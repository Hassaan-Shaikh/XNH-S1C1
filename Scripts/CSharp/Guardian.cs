using Godot;
using System;

public partial class Guardian : Area3D
{
	[Export] public GameControl gameRoot;

	public override void _Ready()
	{
		gameRoot = GetTree().GetFirstNodeInGroup("Game") as GameControl;
	}

	private void OnGuardianPickup(Node3D body)
    {
        switch (Xalkomak.difficulty)
        {
            case Xalkomak.Difficulty.Normal:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksGuardian(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    return; // Sammy does not use the power rune on Normal mode
                }
                break;
            case Xalkomak.Difficulty.Hard:
                if (body.IsInGroup("Player"))
                {
                    PlayerPicksGuardian(body);
                }
                else if (body.IsInGroup("Monster"))
                {
                    body.Call("SetGuardian", true);
                    Xalkomak.isGuardianCollectedBySammy = true;
                    gameRoot.StartGuardianTimer();
                    QueueFree();
                }
                break;
            default:
                break;
        }
    }

    void PlayerPicksGuardian(Node3D body)
    {
        body.Call("SetGuardian", true);
        Xalkomak.isGuardianCollected = true;
        gameRoot.StartGuardianTimer();
        QueueFree();
    }
}
