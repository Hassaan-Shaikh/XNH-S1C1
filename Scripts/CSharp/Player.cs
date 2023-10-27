using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float speed = 5.0f;
	[Export] public float acceleration = 5f;
	[Export] public float camSensitivity = 4f;
	[Export] public bool playerCanControl = true;
	[Export] public Node3D cam;
	

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		/*if (!playerCanControl)
		{
			return;
		}*/

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        //base._Input(@event);
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            Rotation = new Vector3(0, Rotation.Y - mouseMotion.Relative.X / 1000 * camSensitivity, 0);
			cam.Rotation = new Vector3(cam.Rotation.X - mouseMotion.Relative.Y / 1000 * camSensitivity, 0, 0);
        }
    }
}
