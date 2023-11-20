using Godot;
using System;

public partial class GuardianShield : MeshInstance3D
{
    float timeElapsed;

    public override void _Ready()
    {
        base._Ready();
        Scale = new Vector3(1.0f, 1.0f, 1.0f);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        timeElapsed += (float)delta;
        Scale = new Vector3(timeElapsed * 50, 1, timeElapsed * 50);
        if (timeElapsed >= 10f)
        {
            QueueFree();
        }
    }
}
