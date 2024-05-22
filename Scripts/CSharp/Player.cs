using Godot;
using System;
using System.Reflection;

public partial class Player : CharacterBody3D
{
    [ExportGroup("Player Controls")]
    [ExportSubgroup("Movement")]    
    [Export] public float speed = 3.2f;
    [Export] public float acceleration = 5f;
    [Export] public float gravity = 9.8f;
    [Export] public float jumpForce = 6.3f;
    [ExportSubgroup("Camera")]
    [Export] public float camSensitivity = 4f;  
    [ExportSubgroup("Misc.")]
    [Export] public bool canControl = true;
    [Export] public bool canJump = false;
    [Export] public bool allowHeadBob = true;
    [Export] public bool ignorePlayer = false;
    [Export] public bool isStunned = false;
    [ExportGroup("References")]
    [Export] public TextureProgressBar stamina;
    [Export] public SpotLight3D flashlight;
    [Export] public AnimationPlayer staminaAnim;
    [Export] public Camera3D playerCamera;
    [ExportSubgroup("Power Rune Animators")]
    [Export] public AnimationPlayer speedBoostAnim;
    [Export] public AnimationPlayer stunAnim;
    [Export] public AnimationPlayer playerStunnedAnim;
    [Export] public AnimationPlayer guardianAnim;
    [Export] public AnimationPlayer vanishAnim;

    const string forwardKey = "forward";
    const string backKey = "back";
    const string leftKey = "left";
    const string rightKey = "right";
    const string sprintKey = "sprint";
    const string jumpKey = "jump";
    const string anyMovement = "anyMovement";
    const string pauseKey = "pause";

    bool isExhausted = false;
    bool hasSpeedBoost = false;
    public Vector3 p_Velocity;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;

        if(stamina == null)
        {
            stamina = GetNode<TextureProgressBar>("%StaminaBar");
        }

        flashlight.SpotRange = Xalkomak.difficulty == Xalkomak.Difficulty.Hard ? 15.0f : 25.0f;

        stamina.Value = 100f;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        camSensitivity = Xalkomak.camSens;
    }

    public override void _PhysicsProcess(double delta)
    {
        canControl = Xalkomak.playerCanControl;

		if(!canControl || Xalkomak.isStunCollectedBySammy) //Player cannot control, used for cinematics
		{
			return;
		}
        base._PhysicsProcess(delta);

        //allowHeadBob = IsOnFloor();

        p_Velocity = Velocity;

        Vector3 direction = Input.GetAxis(leftKey, rightKey) * Vector3.Right + Input.GetAxis(backKey, forwardKey) * Vector3.Forward;
        direction = Transform.Basis * direction.Normalized();

        hasSpeedBoost = Xalkomak.isSpeedBoostCollected;

        if (hasSpeedBoost)
        {
            speed = 8.31f;
            stamina.Value = stamina.MaxValue;
            isExhausted = false;
        }
        else
        {
            if (stamina.Value <= stamina.MinValue)
            {
                isExhausted = true;
                staminaAnim.Play("CannotSprint");
            }

            if (direction != Vector3.Zero)
            {
                if (Input.IsActionPressed(sprintKey) && !isExhausted)
                {
                    speed = 5.64f;
                    stamina.Value -= 0.2f;
                    if(stamina.Value != stamina.MaxValue)
                    {
                        //if (!staminaAnim.CurrentAnimation.Equals("FadeIn"))
                        //    staminaAnim.Play("FadeIn");
                    }
                }
                else
                {
                    speed = 3.2f;
                }
            }
            if (direction != Vector3.Zero || direction == Vector3.Zero)
            {
                if (!Input.IsActionPressed(sprintKey) || isExhausted || !Input.IsActionPressed(anyMovement))
                {
                    stamina.Value += 0.1f;
                    if (stamina.Value == stamina.MaxValue)
                    {
                        //if (!staminaAnim.CurrentAnimation.Equals("FadeOut"))
                        //    staminaAnim.Play("FadeOut");
                    }
                    if (stamina.Value >= 45f)
                    {
                        isExhausted = false;
                        staminaAnim.Play("RESET");
                    }
                }
            }
        }
        //HandleStaminaAnim();

		p_Velocity = p_Velocity.Lerp(direction * speed + p_Velocity.Y * Vector3.Up, acceleration * (float)delta);

        if (IsOnFloor() && Input.IsActionJustPressed(jumpKey))
        {
            if (canJump)
            {
                p_Velocity.Y = jumpForce;
            }
        }
        else if(!IsOnFloor())
        {
            p_Velocity.Y -= gravity * (float)delta;
        }

        Velocity = p_Velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (!canControl)
        {
            return;
        }
        base._Input(@event);
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            Rotation = new Vector3(0, Rotation.Y - mouseMotion.Relative.X / 1000 * camSensitivity, 0f);         
        }
    }

    public string GetPlayerSpeed()
    {
        return Velocity.Length().ToString();
    }

    public bool GetExhaustedState()
    {
        return isExhausted;
    }

    public void SetSpeedBoost(bool setter)
    {
        if (setter)
        {
            speedBoostAnim.Play("SB_FadeIn");
        }
        else
        {
            speedBoostAnim.Play("SB_FadeOut");
        }
    }

    public void SetGuardian(bool setter)
    {
        if (setter)
        {
            guardianAnim.Play("G_FadeIn");
        }
        else
        {
            guardianAnim.Play("G_FadeOut");
        }
    }

    public void SetStun(bool setter)
    {
        if (setter)
        {
            stunAnim.Play("S_FadeIn");
        }
        //else
        //{
        //    stunAnim.Play("S_FadeOut");
        //}
    }

    public void SetVanish(bool setter)
    {
        if (setter)
        {
            vanishAnim.Play("V_FadeIn");
        }
        else
        {
            vanishAnim.Play("V_FadeOut");
        }
    }

    public void SetStunned(bool playerStunned)
    {
        isStunned = playerStunned;
        playerStunnedAnim.CurrentAnimation = playerStunned ? "P_Stunned" : "P_Unstun";
        playerStunnedAnim.Play();
    }

    private void StartPersisting(string animName)
    {
        if(animName == "V_FadeIn")
        {
            vanishAnim.Play("V_Persist");
        }
    }

    private void HandleStaminaAnim()
    {
        if (isExhausted)
        {
            if (staminaAnim.CurrentAnimation != "CannotSprint")
                staminaAnim.Play("CannotSprint");
        }
        //else
        //{
        //    if (staminaAnim.CurrentAnimation != "RESET")
        //        staminaAnim.Play("RESET");
        //}
    }

    private void OnStaminaBarValueChanged(float value)
    {
        string fadeAnim = value == stamina.MaxValue ? "FadeOut" : "FadeIn";
        //if (staminaAnim.CurrentAnimation != fadeAnim)
        //    staminaAnim.Play(fadeAnim);
    }
}
