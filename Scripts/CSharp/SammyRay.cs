using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    public Player player;
    public SmilingSammy sammy;
    public Camera3D camera;
    [Export] public AnimationPlayer rayAnim;

    Vector3 lastPos;
    int flag = 0;
    bool playerSpotted = false;
    bool canSeePlayer;
    
    public static bool ignoreCheck;
    public static Vector3 angle;

    public override void _Ready()
    {
        base._Ready();
        Enabled = false;
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
        camera = GetViewport().GetCamera3D();
        Timer starter = new Timer();
        AddChild(starter);
        starter.WaitTime = 6f;
        starter.OneShot = true;
        starter.Start();
        starter.Timeout += () => { Enabled = true; starter.QueueFree(); };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if(!Enabled)
        {
            return;
        }
        
        LookTowards(camera.GlobalPosition);
        canSeePlayer = IsColliding();

        if (canSeePlayer)
        {
            GodotObject detected = GetCollider();
            if(detected is Player)
            {    
                if(Xalkomak.isVanishCollected || Xalkomak.isStunCollected || ignoreCheck)
                {
                    return;
                }
                playerSpotted = true;
                //GD.Print("Player spotted.");
                sammy.ChasePlayer();
                flag = 0;
            }
        }
        else
        {
            if(flag <= 60)
            {
                sammy.LostPlayer();
                //GD.Print("Player lost.");
                flag += (int)delta;
                playerSpotted = false;
            }
        }
    }

    private void LookTowards(Vector3 target)
    {
        LookAt(new Vector3(target.X, GlobalPosition.Y, target.Z), Vector3.Up);
        //GD.Print(Mathf.RadToDeg(Rotation.Y));
        if(Rotation.Y < Mathf.DegToRad(-110.0f) || Rotation.Y > Mathf.DegToRad(110.0f))
        {
            ignoreCheck = true;
        }
        else
        {
            ignoreCheck = false;
        }
    }
}
