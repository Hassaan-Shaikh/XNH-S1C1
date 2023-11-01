using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float speed = 3.0f;
	[Export] public float acceleration = 5f;
    [Export] public float camSensitivity = 4f;
    [Export] public bool playerCanControl = true;

    bool gamePause;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
		if(!playerCanControl)
		{
			return;
		}
        base._PhysicsProcess(delta);

        if (Input.IsActionJustPressed("pause"))
        {
            gamePause = !gamePause;
            if (gamePause)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else if (!gamePause)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }

        Vector3 direction = Input.GetAxis("left", "right") * Vector3.Right + Input.GetAxis("back", "forward") * Vector3.Forward;
        direction = Transform.Basis * direction;
		Velocity = Velocity.Lerp(direction * speed + Velocity.Y * Vector3.Up, acceleration * (float)delta);

		//LookAt(LookAtPoint(camLookat.GlobalPosition));

		MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            Rotation = new Vector3(0, Rotation.Y - mouseMotion.Relative.X / 1000 * camSensitivity, 0f);         
        }
    }

    private Vector3 LookAtPoint(Vector3 point)
	{
		return new Vector3(point.X, GlobalPosition.Y, point.Z);
	}
}
