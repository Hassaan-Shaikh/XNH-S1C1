using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SmilingSammy : CharacterBody3D
{
    [Signal] public delegate void RandomizeNavigationEventHandler();

    [ExportCategory("References")]
    [Export] public NavigationAgent3D sammyNav;
    [Export] public BoneAttachment3D head;
    [Export] public Timer sammyTimer;
    [Export] public AnimationTree sammyAnimTree;
    [Export] public Player player;
    [Export] public Timer sammyNavTimer;

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

    public static float s_Velocity;
    public static SammyStates currentState;

    private Vector3 targetPos;
    private float moveSpeed;

    const float walkSpeed = 3.2f;
    const float runSpeed = 4.93f;
    const float walkSpeedHard = 4.3f;
    const float runSpeedHard = 5.72f;

    public override void _Ready()
    {
        base._Ready();
        sammyNav = GetNode<NavigationAgent3D>("SammyNav");
        head = GetNode<BoneAttachment3D>("%Head");
        sammyTimer = GetNode<Timer>("SammyTimer");
        sammyNavTimer = GetNode<Timer>("NavUpdateTimer");
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammyTimer.Start();
        waypoints = GetTree().GetNodesInGroup("Waypoints").Select(saar => saar as Marker3D).ToList();
        currentState = SammyStates.Idle;
        int waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        targetPos = waypoints[waypointIndex].GlobalPosition;
        sammyNav.TargetPosition = targetPos;
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
        s_Velocity = Velocity.Length();
        switch (currentState)
        {
            case SammyStates.Idle:
                moveSpeed = 0;
                Velocity = Vector3.Zero;
                sammyAnimTree.Set("parameters/conditions/idle", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);        
                sammyAnimTree.Set("parameters/conditions/playerSpotted", false);
                return;
            case SammyStates.Patrolling:
                if (sammyNav.IsNavigationFinished())
                {
                    sammyTimer.Stop();
                    Repath();
                    /*currentState = SammyStates.Idle;
                    moveSpeed = 0;*/
                    return;
                }
                NavigateAround((float)delta);
                break;
            case SammyStates.Chasing:
                if (sammyNav.IsNavigationFinished())
                {
                    currentState = SammyStates.Hunting;
                    moveSpeed = 0;
                    return;
                }
                NavigateAround((float)delta);
                break;
            case SammyStates.Hunting:
                moveSpeed = 0;
                Velocity = Vector3.Zero;
                sammyAnimTree.Set("parameters/conditions/idle", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);
                sammyAnimTree.Set("parameters/conditions/playerSpotted", false);
                return;
            case SammyStates.Stunned:
                moveSpeed = 0;
                Velocity = Vector3.Zero;
                return;
            default:
                break;
        }
        
        MoveAndSlide();
    }

    private void NavigateAround(float delta)
    {
        Vector3 nextPos;
        Vector3 direction;
        switch(currentState)
        {
            case SammyStates.Patrolling:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? walkSpeed : walkSpeedHard;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(delta);
                Velocity = direction * moveSpeed;
                sammyAnimTree.Set("parameters/conditions/notIdle", true);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                break;
            case SammyStates.Chasing:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? runSpeed : runSpeedHard;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(delta);
                Velocity = direction * moveSpeed;
                sammyAnimTree.Set("parameters/conditions/playerSpotted", true);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                break;
            default:
                break;
        }
    }

    private void TurnToDirection(float delta)
    {
        Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y ,Mathf.Atan2(-Velocity.X, -Velocity.Z), delta * 10), Rotation.Z);
        //LookAt(new Vector3(direction.X, GlobalPosition.Y, direction.Z), Vector3.Up);
    }

    public void ChasePlayer()
    {
        currentState = SammyStates.Chasing;
        targetPos = player.GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    public void LostPlayer()
    {
        currentState = SammyStates.Chasing;
        targetPos = player.GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    void Repath()
    {
        EmitSignal(SignalName.RandomizeNavigation);
        sammyTimer.WaitTime = GD.RandRange(4f, 10f);
        sammyTimer.Start();
        currentState = SammyStates.Patrolling;
        int waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        targetPos = waypoints[waypointIndex].GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    private void OnNavUpdateTimerTimeout()
    {
        sammyNav.TargetPosition = targetPos;

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

    public static string GetSammySpeed()
    {
        return s_Velocity.ToString();
    }

    public static string GetCurrentState()
    {
        return currentState.ToString();
    }
}
