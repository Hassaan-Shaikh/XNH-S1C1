using Godot;
using System;
using System.Collections;

public partial class SmilingSammy : CharacterBody3D
{
    [ExportCategory("References")]
    [Export] public NavigationAgent3D sammyNav;
    [Export] public BoneAttachment3D head;
    [Export] public Timer sammyTimer;
    [Export] public AnimationTree sammyAnimTree;
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
        sammyTimer.Start();
        if(waypoints == null)
        {
            waypoints = new Marker3D[GetTree().GetNodesInGroup("Waypoints").Count];
            for (int i = 0; i < waypoints.Length; i++) 
            {
                waypoints[i] = GetTree().GetNodesInGroup("Waypoints")[i] as Marker3D;
            }
        }
        currentState = SammyStates.Idle;
        moveSpeed = walkSpeed;
        //sammyNav.TargetPosition = waypoints[0].GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(sammyNav.IsNavigationFinished())
        {
            currentState = SammyStates.Idle;
            return;
        }

        switch(currentState)
        {
            case SammyStates.Patrolling:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? walkSpeed : walkSpeedHard;
                Vector3 nextPos = sammyNav.GetNextPathPosition();
                Vector3 direction = GlobalTransform.Origin.DirectionTo(nextPos);
                TurnToDirection(nextPos);
                Velocity = direction * moveSpeed;
                break;
        }
        
        MoveAndSlide();
    }

    private void TurnToDirection(Vector3 direction)
    {
        //GlobalRotation.Lerp(new Vector3(direction.X, Position.Y, direction.Z), 3);
        LookAt(new Vector3(direction.X, GlobalPosition.Y, direction.Z), Vector3.Up);
    }

    void Repath()
    {
        sammyTimer.WaitTime = GD.RandRange(4f, 10f);
        sammyTimer.Start();
        currentState = SammyStates.Patrolling;
        int waypointIndex = GD.RandRange(0, waypoints.Length - 1);
        sammyNav.TargetPosition = waypoints[waypointIndex].GlobalPosition;
    }
}