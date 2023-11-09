using Godot;
using System;

public partial class SmilingSammy : CharacterBody3D
{
    [ExportCategory("References")]
    [Export] public NavigationAgent3D sammyNav;
    [Export] public BoneAttachment3D head;
    [Export] public Timer sammyTimer;
    [Export] public Player player;
    [ExportCategory("Waypoints")]
    [Export] public Marker3D[] waypoints;

    public enum SammyStates
    {
        Idle,
        Patrolling,
        Chasing,
        Hunting,
        Stunned
    };
    [ExportCategory("Sammy's Movement Handling")]
    [Export] private SammyStates currentState;
    [Export] private float moveSpeed;

    const float walkSpeed = 3.2f;
    const float runSpeed = 4.93f;
    const float walkSpeedHard = 4.3f;
    const float runSpeedHard = 5.72f;

    public override void _Ready()
    {
        base._Ready();
        sammyNav = GetNode<NavigationAgent3D>("SammyNav");
        head = GetNode<BoneAttachment3D>("Smiling Sammy2/Skeleton3D/Head");
        sammyTimer = GetNode<Timer>("SammyTimer");
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        if(waypoints == null)
        {
            waypoints = new Marker3D[GetTree().GetNodesInGroup("Waypoints").Count];
            for(int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = GetTree().GetNodesInGroup("Waypoints")[i] as Marker3D;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(CheckStunned())
        {
            currentState = SammyStates.Stunned;
        }
        else
        {
            currentState = SammyStates.Idle;
        }
    }

    private bool CheckStunned()
    {
        return Xalkomak.isStunCollected;
    }

    private void NavigateAround(SammyStates state)
    {
        int waypointIndex = GD.RandRange(0, waypoints.Length - 1);
        switch(state)
        {
            case SammyStates.Idle:
                sammyNav.TargetPosition = waypoints[waypointIndex].GlobalPosition;
                break;
            case SammyStates.Patrolling:
                if(Xalkomak.difficulty == Xalkomak.Difficulty.Hard)
                {
                    moveSpeed = walkSpeedHard;
                }
                else
                {
                    moveSpeed = walkSpeed;
                }
                sammyNav.TargetPosition = waypoints[waypointIndex].GlobalPosition;
                break;
            case SammyStates.Chasing:
                if (Xalkomak.difficulty == Xalkomak.Difficulty.Hard)
                {
                    moveSpeed = runSpeedHard;
                }
                else
                {
                    moveSpeed = runSpeed;
                }
                sammyNav.TargetPosition = player.GlobalPosition;
                break;
            case SammyStates.Hunting: 
                break;
            case SammyStates.Stunned:
                return;
        }
        if(sammyNav.IsNavigationFinished())
        {
            currentState = SammyStates.Idle;
            return;
        }
        Vector3 nextPos = sammyNav.GetNextPathPosition();
        Velocity = (nextPos - GlobalPosition).Normalized() * moveSpeed;
        LookAt(nextPos);
    }
}
