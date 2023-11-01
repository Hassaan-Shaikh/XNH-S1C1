using Godot;
using System;

public partial class CamControl : Node3D
{
    [Export] public float sens = 4f;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion)@event;
            Rotation = new Vector3(Mathf.Clamp(Rotation.X - mouseMotion.Relative.Y / 1000 * sens, Xalkomak.DegToRad(-75), Xalkomak.DegToRad(75)), 0f, 0f);
        }
    }
}
