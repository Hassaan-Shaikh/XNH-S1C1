using Godot;
using System;

public partial class Door : AnimatableBody3D
{
	[ExportCategory("References")]
	[Export] public AnimationPlayer doorAnim;
	[Export] public NavigationObstacle3D doorObstacle;
	
	public NavigationRegion3D navRegion;

	private string prompt;
	private bool isOpen = false;

    public override void _Ready()
    {
        base._Ready();
		navRegion = GetTree().GetNodesInGroup("NavRegion")[0] as NavigationRegion3D;
    }

    public string GetPrompt()
	{
		prompt = isOpen ? "Click to close." : "Click to open.";
		return prompt;
	}

	public bool IsOpen()
	{
		return isOpen;
	}

	public void ToggleDoor()
	{
		isOpen = !isOpen;
		doorObstacle.AvoidanceEnabled = isOpen;
		switch(isOpen)
		{
			case true:
				doorAnim.Play("SwingDoor");
				break;
			case false:
				doorAnim.PlayBackwards("SwingDoor");
				break;
		}
	}

	private void TimeIt()
	{
        Timer temp = new Timer();
        temp.OneShot = true;
        AddChild(temp);
        temp.WaitTime = 1;
        temp.Start();
        temp.Timeout += () =>
        {
            navRegion.BakeNavigationMesh();
			temp.QueueFree();
        };
    }
}
