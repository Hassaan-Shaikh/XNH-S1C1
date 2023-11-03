using Godot;
using System;

public partial class PlayerRay : RayCast3D
{
	[Export] public Label promptLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        promptLabel.Text = "";
        if (IsColliding())
        {
            GodotObject detected = GetCollider();
			//GD.Print(detected.Name);
            if (detected is Document)
            {
                promptLabel.Text = (string)detected.Call("GetPrompt");
            }
        }
		else 
		{
            promptLabel.Text = "";
        }
        if (Input.IsActionJustPressed("interact"))
		{
			if(IsColliding())
			{
				GodotObject detected = GetCollider();
				if(detected is Document)
				{
					detected.Call("DocumentCollected");
				}
			}
		}
	}
}
