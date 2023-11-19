using Godot;
using System;

public partial class Document : Node3D
{
	private string prompt = "Click to collect Document.";

    public override void _Ready()
    {
        base._Ready();

        string docNum = Name.ToString().Substring(8);
        int docIndex = int.Parse(docNum);
		if (Xalkomak.isDocumentCollected[docIndex])
		{
			QueueFree();
		}
    }

    public string GetPrompt()
	{		
		return (prompt);
	}
	public void DocumentCollected()
	{
		string docNum = Name.ToString().Substring(8);
		int docIndex = int.Parse(docNum);
		Xalkomak.isDocumentCollected[docIndex] = true;
		GD.Print(docNum);
		Xalkomak.documentsCollected += 1;
		//GD.Print(Xalkomak.documentsCollected);
		QueueFree();
	}
}
