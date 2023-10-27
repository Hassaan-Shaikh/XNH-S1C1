using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float movementSpeed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public bool suspendPlayerControl = false;
	[Export] public bool canJump = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * movementSpeed;
			velocity.Z = direction.Z * movementSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, movementSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, movementSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
