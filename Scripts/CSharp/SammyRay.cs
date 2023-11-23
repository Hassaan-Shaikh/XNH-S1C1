using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    [Export] private float angleThreshold = 110.0f;

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
        flagTarget = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 60 : 120;
        Enabled = false;
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
        flag = flagTarget;
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

        //TargetPosition = ToLocal(camera.GlobalPosition);

        /*GodotObject detected = GetCollider();
        canSeePlayer = IsColliding() && detected is Player;

        if (canSeePlayer && (!Xalkomak.isVanishCollected || !Xalkomak.isStunCollected || !player.ignorePlayer || !ignoreCheck))
        {
            sammy.ChasePlayer();
            flag = 0;
        }
        else if(flag <= flagTarget)
        {
            sammy.playerTracker.GlobalPosition = GlobalPosition + new Vector3((float)GD.RandRange(-2.0f, 2.0f), GlobalPosition.Y, (float)GD.RandRange(-2.0f, 2.0f));
            sammy.LostPlayer(sammy.playerTracker.GlobalPosition);
        }*/
        //GD.Print(canSeePlayer + ", " + detected);
        LookTowards(camera.GlobalPosition);

        if (IsColliding())
        {
            GodotObject detected = GetCollider();
            if(detected is Player)
            {    
                if(Xalkomak.isVanishCollected || Xalkomak.isStunCollected || ignoreCheck || sammy.isStunned || player.ignorePlayer)
                {
                    return;
                }
                canSeePlayer = IsColliding() && detected is Player;
                playerSpotted = true;
                //GD.Print("Player spotted.");
                lastPos = player.GlobalPosition;
                sammy.ChasePlayer();
                flag = 0;
            }
            //else
            //{
            //    sammy.LostPlayer();
            //    //GD.Print("Player lost.");
            //    flag += 1;
            //    playerSpotted = false;
            //}
        }
        else
        {
            if (flag == 0)
            {
                sammy.LostPlayer();
                //GD.Print("Player lost.");
                flag += 1;
                playerSpotted = false;
            }
        }
    }

    private void LookTowards(Vector3 target)
    {
        LookAt(new Vector3(target.X, GlobalPosition.Y, target.Z), Vector3.Up);
        //GD.Print(Mathf.RadToDeg(Rotation.Y));
        if(Rotation.Y <= Mathf.DegToRad(-110.0f) && Rotation.Y >= Mathf.DegToRad(110.0f))
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
        if (Rotation.Y <= Mathf.DegToRad(angleThreshold * -1) || Rotation.Y >= Mathf.DegToRad(angleThreshold))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
