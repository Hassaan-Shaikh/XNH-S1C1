using Godot;
using System;

public partial class Document : Node3D
{
	[Export] public Node3D parent;
	[Export] public int docNumber;

	private string prompt = "Click to collect ";

	public string GetPrompt()
	{		
		return (prompt + Name);
	}
	public void DocumentCollected()
	{
		Xalkomak.documentsCollected += 1;
		GD.Print(Xalkomak.documentsCollected);
		parent.QueueFree();
	}
}
