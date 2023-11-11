using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    public Player player;
    public SmilingSammy sammy;
    [Export] public AnimationPlayer rayAnim;

    public override void _Ready()
    {
        base._Ready();
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(IsColliding())
        {
            Vector3 lastKnownPos;
            GodotObject detected = GetCollider();
            if(detected is Player)
            { 
                Player player = (Player)detected;
                rayAnim.Play("RESET");
                lastKnownPos = player.GlobalPosition;
                LookAt(new Vector3(Rotation.X, Mathf.Clamp(Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60)), Rotation.Z));
                sammy.ChasePlayer(lastKnownPos);
            }
            else
            {
                lastKnownPos = GetCollisionPoint();
                sammy.ChasePlayer(lastKnownPos);
            }
        }
    }
}
