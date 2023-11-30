using Godot;
using System;

public partial class DoorChecker : RayCast3D
{
	[ExportCategory("References")]
	[Export] public SmilingSammy sammy;
    [Export] public Timer waitTimer;

    private bool isDoorOpen = false;

    public override void _Ready()
    {
        base._Ready();

        waitTimer.Timeout += () =>
        {
            
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GodotObject detected = GetCollider();
        if (detected is Door)
        {            
            isDoorOpen = (bool)detected.Call("IsOpen");
            if(!isDoorOpen)
            {
                detected.Call("ToggleDoor");
            }
        }
    }
}
