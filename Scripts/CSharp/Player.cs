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
            stamina = GetNode<TextureProgressBar>("%StaminaBar");
        }

        stamina.Value = 100f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Input.IsActionJustPressed("pause"))
        {
            GetTree().Quit(); // This line of code must be removed when the game is ready for release
        }

        playerCanControl = Xalkomak.playerCanControl;

		if(!playerCanControl) //Player cannot control, used for cinematics
		{
			return;
		}
        base._PhysicsProcess(delta);

        Vector3 direction = Input.GetAxis("left", "right") * Vector3.Right + Input.GetAxis("back", "forward") * Vector3.Forward;
        direction = Transform.Basis * direction.Normalized();

        if(stamina.Value <= stamina.MinValue)
        {
            isExhausted = true;
        }
        
        if(direction != Vector3.Zero)
        {
            if (Input.IsActionPressed("sprint") && !isExhausted)
            {
                speed = 5.64f;
                stamina.Value -= 0.2f;
            }
            else
            {
                speed = 3.2f;
            }
        }
        if(direction != Vector3.Zero || direction == Vector3.Zero)
        {
            if (!Input.IsActionPressed("sprint") || isExhausted)
            {
                stamina.Value += 0.1f;
                if(stamina.Value >= 45f)
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
        return Velocity.ToString();
    }

    public bool GetExhaustedState()
    {
        return isExhausted;
    }
}
