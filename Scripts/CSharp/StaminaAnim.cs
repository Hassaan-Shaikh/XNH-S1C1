using Godot;
using System;

public partial class StaminaAnim : AnimationPlayer
{
    private TextureProgressBar staminaBar;
    
	private const string animFadeIn = "FadeIn";
    private const string animFadeOut = "FadeOut";
    private const string animCannotSprint = "CannotSprint";

    public override void _Ready()
    {
        base._Ready();
        staminaBar = GetNode<TextureProgressBar>("%StaminaBar");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
