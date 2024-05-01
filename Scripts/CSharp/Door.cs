using Godot;
using System;

public partial class Door : AnimatableBody3D
{
	[ExportCategory("References")]
	[Export] public Node3D rootNode;
	[Export] public AnimationPlayer doorAnim;
	[Export] public CollisionShape3D doorColShape;
	
	public NavigationRegion3D navRegion;

	private string prompt;
	private bool isOpen = false;

    public override void _Ready()
    {
        base._Ready();
		navRegion = GetTree().GetNodesInGroup("NavRegion")[0] as NavigationRegion3D;
		doorAnim.AnimationFinished += (StringName animName) => 
		{
			doorColShape.Disabled = false;
		};
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
		if(!IsExitDoor())
		{
            isOpen = !isOpen;
            switch (isOpen)
            {
                case true:
                    doorAnim.Play("SwingDoor");
                    doorColShape.Disabled = true;
                    break;
                case false:
                    doorAnim.PlayBackwards("SwingDoor");
					doorColShape.Disabled = true;
                    break;
            }
        }
	}

	public bool IsExitDoor()
	{
		return rootNode.Name.Equals("Door6");
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
