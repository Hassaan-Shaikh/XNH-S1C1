using Godot;
using System;

public partial class SammyRay : RayCast3D
{
    public Player player;
    public SmilingSammy sammy;
    [Export] public AnimationPlayer rayAnim;
    [Export] public BoneAttachment3D head;

    Vector3 lastPos;
    int flag = 0;
    bool playerSpotted = false;

    public override void _Ready()
    {
        base._Ready();
        Enabled = false;
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammy = GetTree().GetNodesInGroup("Monster")[0] as SmilingSammy;
        Timer starter = new Timer();
        AddChild(starter);
        starter.WaitTime = 6f;
        starter.OneShot = true;
        starter.Start();
        starter.Timeout += () => { Enabled = true; GD.Print("Sammy starts searching now."); starter.QueueFree(); };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition = head.GlobalPosition;
        Vector3 playerLookAt = new Vector3(player.GlobalPosition.X, Mathf.Clamp(Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60)), player.GlobalPosition.Z);
        LookAt(playerLookAt);
        //Rotation = new Vector3(0, Mathf.Clamp(Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60)), 0);
        if (IsColliding())
        {
            GodotObject detected = GetCollider();
            if(detected is Player)
            {
                if (!playerSpotted)
                {
                    if (Xalkomak.isVanishCollected || Xalkomak.isStunCollected)
                    {
                        return;
                    }
                    playerSpotted = true;
                    GD.Print("Player spotted.");
                    sammy.ChasePlayer();
                    flag = 0;
                }
            }
            else if(flag == 0)
            {
                sammy.LostPlayer();
                GD.Print("Player lost.");
                flag++;
                playerSpotted = false;
            }
        }
    }

    private void StartSearch()
    {

    }
}
