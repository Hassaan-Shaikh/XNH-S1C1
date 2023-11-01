using Godot;
using System;

public partial class CamControl : Node3D
{
    [Export] public float camSensitivity = 4f;
    [Export] public float camAcceleration = 2f;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if(@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseMotion = (InputEventMouseMotion) @event;
            Rotation = new Vector3(Mathf.Clamp(Rotation.X - mouseMotion.Relative.Y / 1000 * camSensitivity, Mathf.DegToRad(-75), Mathf.DegToRad(75)), 0, 0);
        }
    }
}
