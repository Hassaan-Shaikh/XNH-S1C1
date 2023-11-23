using Godot;
using System;

public partial class DifficultySelectionMenu : Control
{
    [Signal] public delegate void DifficultySelectedEventHandler(StringName difficulty);

    public AnimationPlayer popUpAnim;

    public override void _Ready()
    {
        base._Ready();
        popUpAnim = GetNode<AnimationPlayer>("PopupAnim");
    }
    private void OnNormalModeButtonPressed()
    {
        //GD.Print("This should start the game on Normal difficulty.");
        popUpAnim.Play("PopOut");
        EmitSignal(SignalName.DifficultySelected, "Normal");
    }

    private void OnHardModeButtonPressed()
    {
        //GD.Print("This should start the game on Hard difficulty.");
        popUpAnim.Play("PopOut");
        EmitSignal(SignalName.DifficultySelected, "Hard");
    }

    private void OnBackButtonPressed()
    {
        popUpAnim.Play("PopOut");
        EmitSignal(SignalName.DifficultySelected, "None");
    }
}
