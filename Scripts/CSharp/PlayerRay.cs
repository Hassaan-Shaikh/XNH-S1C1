using Godot;
using System;

public partial class PlayerRay : RayCast3D
{
	[Export] public Label promptLabel;
    [Export] public TextureRect crosshair;
	[Export] public Texture2D crosshairNormal;
    [Export] public Texture2D crosshairHand;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        promptLabel.Text = "";
        crosshair.Texture = crosshairNormal;
        if (IsColliding())
        {
            GodotObject detected = GetCollider();
			//GD.Print(detected.Name);
            if (detected is Document)
            {
                crosshair.Texture = crosshairHand;
                promptLabel.Text = (string)detected.Call("GetPrompt");
            }
			else if(detected is Door)
			{
                crosshair.Texture = crosshairHand;
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
                else if (detected is Door)
                {
                    detected.Call("ToggleDoor");
                }
            }
		}
	}
}
