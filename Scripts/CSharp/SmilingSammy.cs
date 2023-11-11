using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SmilingSammy : CharacterBody3D
{
    [ExportCategory("References")]
    [Export] public NavigationAgent3D sammyNav;
    [Export] public BoneAttachment3D head;
    [Export] public Timer sammyTimer;
    [Export] public AnimationTree sammyAnimTree;
    [Export] public Player player;

    private List<Marker3D> waypoints = new List<Marker3D>();
    private Vector3 lastLookingDir;

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
    [Export] private float rotationAcceleration = 0.3f;

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
        waypoints = GetTree().GetNodesInGroup("Waypoints").Select(saar => saar as Marker3D).ToList();
        currentState = SammyStates.Idle;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        /*if (sammyNav.IsNavigationFinished())
        {
            currentState = SammyStates.Idle;
            moveSpeed = 0;
            return;
        }*/

        switch (currentState)
        {
            case SammyStates.Idle:
                moveSpeed = 0;
                sammyAnimTree.Set("parameters/conditions/idle", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);        
                sammyAnimTree.Set("parameters/conditions/playerSpotted", false);
                return;
            case SammyStates.Patrolling:
                if (sammyNav.IsNavigationFinished())
                {
                    currentState = SammyStates.Idle;
                    moveSpeed = 0;
                    return;
                }
                NavigateAround();
                break;
            case SammyStates.Chasing:
                if (sammyNav.IsNavigationFinished())
                {
                    currentState = SammyStates.Hunting;
                    moveSpeed = 0;
                    return;
                }
                NavigateAround();
                break;
            case SammyStates.Hunting:
                moveSpeed = 0;
                sammyAnimTree.Set("parameters/conditions/idle", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);
                sammyAnimTree.Set("parameters/conditions/playerSpotted", false);
                return;
            case SammyStates.Stunned:
                return;
            default:
                break;
        }
        
        MoveAndSlide();
    }

    private void NavigateAround()
    {
        Vector3 nextPos;
        Vector3 direction;
        switch(currentState)
        {
            case SammyStates.Patrolling:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? walkSpeed : walkSpeedHard;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(nextPos);
                Velocity = direction * moveSpeed;
                sammyAnimTree.Set("parameters/conditions/notIdle", true);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                break;
            case SammyStates.Chasing:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? runSpeed : runSpeedHard;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(nextPos);
                Velocity = direction * moveSpeed;
                sammyAnimTree.Set("parameters/conditions/playerSpotted", true);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                break;
            default:
                break;
        }
    }

    private void TurnToDirection(Vector3 direction)
    {
        LookAt(new Vector3(direction.X, GlobalPosition.Y, direction.Z), Vector3.Up);
    }

    public void ChasePlayer(Vector3 targetPos)
    {
        currentState = SammyStates.Chasing;
        sammyNav.TargetPosition = targetPos;
    }

    void Repath()
    {
        sammyTimer.WaitTime = GD.RandRange(4f, 10f);
        sammyTimer.Start();
        currentState = SammyStates.Patrolling;
        int waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        sammyNav.TargetPosition = waypoints[waypointIndex].GlobalPosition;
    }

    public void GetStunned()
    {
        currentState = SammyStates.Stunned;
        sammyAnimTree.Set("parameters/conditions/gotStunned", Xalkomak.isStunCollected);
        sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", false);
        moveSpeed = 0f;
        sammyTimer.Stop();
    }

    public void RecoverFromStun()
    {
        sammyAnimTree.Set("parameters/conditions/gotStunned", false);
        sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", true);
        moveSpeed = 0;
        currentState = SammyStates.Idle;
        sammyTimer.Start();
    }
}
