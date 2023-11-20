using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SmilingSammy : CharacterBody3D
{
    [Signal] public delegate void RandomizeNavigationEventHandler();
    [Signal] public delegate void StoppedByGuardianEventHandler();

    [ExportCategory("References")]
    [Export] public NavigationAgent3D sammyNav;
    [Export] public BoneAttachment3D head;
    [Export] public Timer sammyTimer;
    [Export] public AnimationTree sammyAnimTree;
    [Export] public Player player;
    [Export] public Timer sammyNavTimer;
    [Export] public Area3D jumpscareArea;

    private List<Marker3D> waypoints = new List<Marker3D>();
    private Vector3 lastLookingDir;

    public enum SammyStates
    {
        Idle,
        Patrolling,
        Chasing,
        Hunting,
        Stunned,
        SpeedBoosted
    };

    public static float s_Velocity;
    public static SammyStates currentState;
    public static int waypointIndex;
    public static int waypointCount;

    private Vector3 targetPos;
    private float moveSpeed;
    private int flag = 0;
    private int jumpscareNum;
    public bool isStunned = false;

    const float walkSpeed = 3.2f;
    const float runSpeed = 4.23f;
    const float walkSpeedHard = 4.3f;
    const float runSpeedHard = 5.72f;
    const string deathScreen = "res://Scenes/DeathScreen.tscn";

    public override void _Ready()
    {
        base._Ready();
        sammyNav = GetNode<NavigationAgent3D>("SammyNav");
        head = GetNode<BoneAttachment3D>("%Head");
        sammyTimer = GetNode<Timer>("SammyTimer");
        sammyNavTimer = GetNode<Timer>("NavUpdateTimer");
        jumpscareArea = GetNode<Area3D>("JumpscareArea");
        player = GetTree().GetNodesInGroup("Player")[0] as Player;
        sammyTimer.Start();
        waypoints = GetTree().GetNodesInGroup("Waypoints").Select(saar => saar as Marker3D).ToList();
        currentState = SammyStates.Idle;
        waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        waypointCount = waypoints.Count;
        targetPos = waypoints[waypointIndex].GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Visible = !Xalkomak.isVanishCollectedBySammy; 
        if(Xalkomak.isSpeedBoostCollectedBySammy)
        {
            currentState = SammyStates.SpeedBoosted;
        }
        else
        {
            return;
        }
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
                if (sammyNav.IsNavigationFinished() || !sammyNav.IsTargetReachable())
                {
                    Repath();
                    return;
                }
                NavigateAround((float)delta);
                break;
            case SammyStates.Chasing:
                if (sammyNav.IsNavigationFinished() || !sammyNav.IsTargetReachable())
                {                    
                    currentState = SammyStates.Hunting;
                    moveSpeed = 0;
                    return;
                }
                NavigateAround((float)delta);
                break;
            case SammyStates.Hunting:
                if (flag == 0)
                {
                    SetTimer();
                    flag++;
                }
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
            case SammyStates.SpeedBoosted:
                if (sammyNav.IsNavigationFinished() || !sammyNav.IsTargetReachable())
                {
                    sammyTimer.Stop();
                    RepathSpeedBoost();
                    return;
                }
                break;
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
                sammyAnimTree.Set("parameters/conditions/playerSpotted", false);
                break;
            case SammyStates.Chasing:
                moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? runSpeed : runSpeedHard;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(delta);
                Velocity = direction * moveSpeed;
                sammyAnimTree.Set("parameters/conditions/playerSpotted", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                break;
            case SammyStates.SpeedBoosted:
                moveSpeed = 7.64f;
                nextPos = sammyNav.GetNextPathPosition();
                direction = GlobalPosition.DirectionTo(nextPos);
                TurnToDirection(delta);
                Velocity = direction * moveSpeed;                
                sammyAnimTree.Set("parameters/conditions/playerSpotted", true);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);
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
        sammyTimer.Stop();
    }

    public void LostPlayer()
    {
        currentState = SammyStates.Chasing;
        targetPos = player.GlobalPosition;
        sammyNav.TargetPosition = targetPos;
        //SetTimer();
    }

    void Repath()
    {
        sammyTimer.Stop();
        EmitSignal(SignalName.RandomizeNavigation);
        SetTimer();
        currentState = SammyStates.Patrolling;
        waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        targetPos = waypoints[waypointIndex].GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    void RepathSpeedBoost()
    {
        sammyTimer.Stop();
        EmitSignal(SignalName.RandomizeNavigation);
        //SetTimer();
        currentState = SammyStates.SpeedBoosted;
        waypointIndex = GD.RandRange(0, waypoints.Count - 1);
        targetPos = waypoints[waypointIndex].GlobalPosition;
        sammyNav.TargetPosition = targetPos;
    }

    private void OnNavUpdateTimerTimeout()
    {
        sammyNav.TargetPosition = targetPos;

    }

    public void GetStunned()
    {
        if(Xalkomak.isGuardianCollectedBySammy) // When Sammy is under the influence of the Guardian rune
        {
            return; // Just don't react to getting hit by the Stun rune.
        }
        currentState = SammyStates.Stunned;
        isStunned = true;
        sammyAnimTree.Set("parameters/conditions/idle", false);
        sammyAnimTree.Set("parameters/conditions/idle", false);
        sammyAnimTree.Set("parameters/conditions/notIdle", false);
        sammyAnimTree.Set("parameters/conditions/gotStunned", true);
        sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", false);
        moveSpeed = 0f;
        Velocity = Vector3.Zero;
        sammyTimer.Stop();
    }

    public void RecoverFromStun()
    {
        sammyAnimTree.Set("parameters/conditions/gotStunned", false);
        sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", true);
        moveSpeed = 0;
        SetTimer();
    }

    public static string GetSammySpeed()
    {
        return s_Velocity.ToString();
    }

    public static string GetCurrentState()
    {
        return currentState.ToString();
    }

    public static string GetWaypointIndex()
    {
        return waypointIndex.ToString();
    }

    public static string GetWaypointCount()
    {
        return waypointCount.ToString();
    }

    private void SetTimer()
    {
        sammyTimer.WaitTime = GD.RandRange(4f, 10f);
        sammyTimer.Start();
    }

    private void OnJumpscareAreaBodyEntered(Node3D body)
    {
        if(body.IsInGroup("Player"))
        {
            if (Xalkomak.isGuardianCollected)
            {
                EmitSignal(SignalName.StoppedByGuardian);
                jumpscareArea.SetDeferred("monitoring", false);
                GetStunned();
                return;
            }
            else
            {
                sammyAnimTree.Set("parameters/conditions/idle", false);
                sammyAnimTree.Set("parameters/conditions/idle", false);
                sammyAnimTree.Set("parameters/conditions/notIdle", false);
                sammyAnimTree.Set("parameters/conditions/gotStunned", false);
                sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", false);
                jumpscareArea.SetDeferred("monitoring", false);
                jumpscareNum = GD.RandRange(1, 2);
                currentState = SammyStates.Idle;
                Velocity = Vector3.Zero;
                Xalkomak.playerCanControl = false;
                player.GlobalPosition = new Vector3(player.GlobalPosition.X, player.GlobalPosition.Y + 0.1f, player.GlobalPosition.Z);
                player.LookAt(new Vector3(GlobalPosition.X, player.GlobalPosition.Y, GlobalPosition.Z), Vector3.Up);
                sammyAnimTree.Set("parameters/conditions/playerCaught" + jumpscareNum, true);
            }
        }
    }

    private void OnJumpscareFinished(string animName)
    {
        if(animName.Equals("StunEnd"))
        {
            isStunned = false;
            jumpscareArea.SetDeferred("monitoring", true);
            currentState = SammyStates.Idle;
        }

        if(animName.Equals("Jumpscare" + jumpscareNum))
        {
            if(Xalkomak.livesRemaining <= 1)
            {
                Xalkomak.livesRemaining = 0;
            }
            GetTree().ChangeSceneToFile(deathScreen);
        }
    }

    public void GoToPowerRune(Vector3 pos)
    {
        sammyNav.TargetPosition = pos;
    }
}
