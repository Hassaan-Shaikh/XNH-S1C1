using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    public Player player;
    public SmilingSammy sammy;
    [Export] public AnimationPlayer rayAnim;

    Vector3 lastPos;

    public override void _Ready()
    {
        base._Ready();
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        LookAt(new Vector3(player.GlobalPosition.X, Mathf.Clamp(Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60)), player.GlobalPosition.Z));
        if (IsColliding())
        {
            /*GodotObject detected = GetCollider();
            if(detected is Player)
            { 
                sammy.ChasePlayer();
            }
            else
            {
                sammy.LostPlayer();
            }*/
        }
    }
}
