using Godot;
using System;

public partial class Document : Node3D
{
	private string prompt = "Click to collect Document.";

	public string GetPrompt()
	{		
		return (prompt);
	}
	public void DocumentCollected()
	{
		Xalkomak.documentsCollected += 1;
		GD.Print(Xalkomak.documentsCollected);
		QueueFree();
	}
}
