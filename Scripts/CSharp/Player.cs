using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float speed = 3.2f;
	[Export] public float acceleration = 5f;
    [Export] public float camSensitivity = 4f;
    [Export] public bool playerCanControl = true;
    [Export] public TextureProgressBar stamina;

    bool gamePause;
    bool isExhausted = false;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;

        if(stamina == null)
        {
            GetNode<TextureProgressBar>("%StaminaBar");
        }

        stamina.Value = 100f;
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
        direction = Transform.Basis * direction.Normalized();

        //isExhausted = stamina.Value <= stamina.MinValue;
        
        if(direction != Vector3.Zero)
        {
            if (Input.IsActionPressed("sprint"))
            {
                speed = 5.3f;
                stamina.Value -= 0.2f;
            }
            else
            {
                speed = 3.2f;
            }
        }
        if(direction != Vector3.Zero || direction == Vector3.Zero)
        {
            if (!Input.IsActionPressed("sprint"))
            {
                stamina.Value += 0.2f;
                if(stamina.Value >= 30f)
                {
                    isExhausted = false;
                }
            }
        }

		Velocity = Velocity.Lerp(direction * speed + Velocity.Y * Vector3.Up, acceleration * (float)delta);

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

    public string GetPlayerSpeed()
    {
        return speed.ToString();
    }
}
