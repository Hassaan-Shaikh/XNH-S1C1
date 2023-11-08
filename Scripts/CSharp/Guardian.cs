using Godot;
using System;

public partial class Guardian : Area3D
{
	[Export] public GameControl gameRoot;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameRoot = GetTree().GetFirstNodeInGroup("Game") as GameControl;
	}

	private void OnGuardianPickup(Node3D body)
	{
		if(body.IsInGroup("Player"))
		{
			body.Call("SetGuardian", true);
			Xalkomak.isGuardianCollected = true;
			gameRoot.StartGuardianTimer();
			QueueFree();
		}
	}
}
