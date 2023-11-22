using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    public Player player;
    public SmilingSammy sammy;
    public Camera3D camera;

    Vector3 lastPos;
    int flag = 0;
    int flagTarget;
    bool playerSpotted = false;
    bool canSeePlayer;
    
    public static bool ignoreCheck;
    public static Vector3 angle;

    public override void _Ready()
    {
        base._Ready();
        flagTarget = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 30 : 60;
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
        canSeePlayer = sammy.isStunned;

        if (IsColliding())
        {
            GodotObject detected = GetCollider();
            if(detected is Player)
            {    
                if(Xalkomak.isVanishCollected || Xalkomak.isStunCollected || ignoreCheck || canSeePlayer || player.ignorePlayer)
                {
                    return;
                }
                playerSpotted = true;
                //GD.Print("Player spotted.");
                sammy.ChasePlayer();
                flag = 0;
            }
            /*else if(Xalkomak.difficulty == Xalkomak.Difficulty.Hard && (detected is SpeedBoost || detected is Stun || detected is Guardian || detected is Vanish))
            {
                Node3D powerRune = (Node3D)detected;
                sammy.GoToPowerRune(powerRune.GlobalPosition);
            }*/
        }
        else
        {
            if(flag <= flagTarget && !player.ignorePlayer)
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
