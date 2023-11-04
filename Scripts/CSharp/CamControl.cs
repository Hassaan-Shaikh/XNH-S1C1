using Godot;
using System;

public partial class CamControl : Node3D
{
    [Export] public float sens = 4f;
    [Export] public float bobFreq = 2f;
    [Export] public float bobAmp = 0.8f;
    [Export] Player parent;

    float tBob = 0;

    public override void _Ready()
    {
        base._Ready();

        parent = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (parent.allowHeadBob)
        {
            tBob += (float)delta * parent.p_Velocity.Length();
            GlobalPosition = HeadBob(tBob);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(!Xalkomak.playerCanControl)
        {
            return;
        }
        base._Input(@event);
        if(@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            Rotation = new Vector3(Mathf.Clamp(Rotation.X - mouseMotion.Relative.Y / 1000 * sens, Mathf.DegToRad(-75), Mathf.DegToRad(75)), 0f, 0f);
        }
    }

    private Vector3 HeadBob(float t)
    {
        Vector3 pos = Vector3.Zero;
        pos.Y = Mathf.Sin(t * bobFreq) * bobAmp;
        pos.X = Mathf.Sin(t * bobFreq / 2) * bobAmp;
        return pos;
    }
}
