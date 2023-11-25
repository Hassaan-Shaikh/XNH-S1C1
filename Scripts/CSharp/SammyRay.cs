using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    [Signal] public delegate void SpottedPlayerEventHandler(bool playerInSight);

    [Export] private float angleThreshold = 85.0f;
    [Export] public Timer huntCountdown;

    public Player player;
    public SmilingSammy sammy;
    public Camera3D camera;
    public Vector3 lastPos;

    int flag = 0;
    int flagTarget;
    bool playerSpotted = false;
    bool canSeePlayer;
    
    public static bool ignoreCheck;
    public static Vector3 angle;

    public override void _Ready()
    {
        base._Ready();
        flagTarget = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 60 : 120;
        Enabled = false;
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
        flag = flagTarget;
        camera = GetViewport().GetCamera3D();
        huntCountdown = GetNode<Timer>("HuntCountdown");
        huntCountdown.Stop();
        huntCountdown.Timeout += sammy.LostPlayer;
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

        if (IsColliding())
        {
            GodotObject detected = GetCollider();
            //canSeePlayer = IsColliding() && detected is Player;
            if (detected is Player)
            {
                if (Xalkomak.isVanishCollected || Xalkomak.isStunCollected || ignoreCheck || sammy.isStunned || player.ignorePlayer)
                {
                    return;
                }
                lastPos = GetCollisionPoint();
                Vector3 seenAt = new Vector3(lastPos.X, 0, lastPos.Z);
                seenAt = new Vector3(player.GlobalPosition.X, 0, player.GlobalPosition.Z);
                //sammy.sammyNav.TargetPosition = player.GlobalPosition;
                sammy.ChasePlayer(seenAt);
                flag = 0;
                huntCountdown.Stop();
                sammy.playerTracker.GlobalPosition = new Vector3(seenAt.X, 0, seenAt.Z); // Follow the player's position
                GD.Print("Sammy can see player: " + seenAt);
            }
            else
            {
                if(huntCountdown.IsStopped() && flag == 0)
                {
                    flag++;
                    huntCountdown.WaitTime = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 2.32f : 4.64f;
                    huntCountdown.Start();
                    sammy.playerTracker.GlobalPosition = new Vector3(sammy.playerTracker.GlobalPosition.X, 0, sammy.playerTracker.GlobalPosition.Z);
                    GD.Print("Sammy can't see player:");
                    //sammy.LostPlayer();
                }
            }
        }
    }

    private void LookTowards(Vector3 target)
    {
        LookAt(new Vector3(target.X, GlobalPosition.Y, target.Z), Vector3.Up);
        //GD.Print(Mathf.RadToDeg(Rotation.Y));
        if (Rotation.Y < Mathf.DegToRad(angleThreshold * -1) || Rotation.Y > Mathf.DegToRad(angleThreshold))
        {
            ignoreCheck = true;
        }
        else
        {
            ignoreCheck = false;
        }
    }

    private bool CheckAngle()
    {
        if (Rotation.Y >= Mathf.DegToRad(-110.0f) && Rotation.Y <= Mathf.DegToRad(110.0f))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
