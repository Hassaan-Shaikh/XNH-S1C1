using Godot;
using System;

public partial class ConfirmQuit : Control
{
    [Signal] public delegate void QuitConfirmedEventHandler(bool confirmed);

    [Export] public RichTextLabel quitMessage;
    
    public AnimationPlayer popAnim;

    public override void _Ready()
    {
        base._Ready();
        popAnim = GetNode<AnimationPlayer>("PopupAnim");
    }

    private void OnConfirmButtonPressed()
    {
        GD.Print("Conifrm button pressed.");
        popAnim.Play("PopOut");
        EmitSignal(SignalName.QuitConfirmed, true);
    }

    private void OnCancelButtonPressed()
    {
        GD.Print("Cancel button pressed.");
        popAnim.Play("PopOut");
        EmitSignal(SignalName.QuitConfirmed, false);
    }
}
