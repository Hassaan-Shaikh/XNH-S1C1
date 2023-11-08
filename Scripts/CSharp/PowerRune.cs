using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public abstract partial class PowerRune : Area3D
{
	public Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		BodyEntered += OnRuneCollected;
	}

	public abstract void OnRuneCollected(Node3D body);
}
