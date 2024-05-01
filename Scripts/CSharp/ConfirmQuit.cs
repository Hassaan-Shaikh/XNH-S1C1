using Godot;
using System;

public partial class ConfirmQuit : Control
{
    [Signal] public delegate void QuitConfirmedEventHandler(bool confirmed);

    [Export] public RichTextLabel title;
    [Export] public RichTextLabel quitMessage;
    [Export] public Button confirm, cancel;
    
    public AnimationPlayer popAnim;

    public override void _Ready()
    {
        base._Ready();
        popAnim = GetNode<AnimationPlayer>("PopupAnim");
    }

    public void SetQuitMessage(string message)
    {
        quitMessage.Text = message;
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
