using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Godot.WebSocketPeer;

public partial class SmilingSammy : CharacterBody3D
{
	[Signal] public delegate void RandomizeNavigationEventHandler();
	[Signal] public delegate void StoppedByGuardianEventHandler();

	[ExportCategory("References")]
	[Export] public Player player;
	[Export] public Camera3D playerCam, jumpscareCam;
	[Export] public NavigationAgent3D sammyNav;
	[Export] public Timer sammyTimer, sammyNavTimer;
	[Export] public AnimationTree sammyAnimTree;
	[Export] public Area3D jumpscareArea;
	[Export] public GpuParticles3D speedBoostParticles, guardianParticles;
	[Export] public MeshInstance3D sammyMesh;
	[Export] public AnimationPlayer sammyMeshAnim;
	[Export] public Marker3D playerTracker;
	[ExportCategory("Movement Handling")]
	[Export] public float rotationAcceleration = 10.0f;

	public enum SammyStates
	{
		Idle,
		Patrolling,
		Chasing,
		Hunting,
		Stunned,
		SpeedBoosted,
	};

	public static float s_Velocity;
	public static int waypointIndex, waypointCount, limitFlag = 0;
	public static string currentStateS, prevStateS;
	public static Vector3 trackerPos;

	public SammyStates currentState, previousState;
	public bool isStunned = false;

	private List<Marker3D> waypoints = new List<Marker3D>();
	private Vector3 lastLookingDir, targetPos;
	private float moveSpeed, animSpeed;
	private int flag = 0, jumpscareNum;

	const float walkSpeed = 3.2f;
	const float runSpeed = 4.4f;
	const float walkSpeedHard = 4f;
	const float runSpeedHard = 5.5f;
	const float animSpeedNormal = 1;
	const float animSpeedHard = 1.25f;
	const float animSpeedSpeedBoost = 1.75f;
	const string deathScreen = "res://Scenes/DeathScreen.tscn";

	public override void _Ready()
	{
		base._Ready();
		playerCam = GetViewport().GetCamera3D();
		sammyNav = GetNode<NavigationAgent3D>("SammyNav");
		jumpscareCam = GetNode<Camera3D>("JumpscareCam");
		sammyTimer = GetNode<Timer>("SammyTimer");
		sammyNavTimer = GetNode<Timer>("NavUpdateTimer");
		jumpscareArea = GetNode<Area3D>("JumpscareArea");
		playerTracker = GetNode<Marker3D>("PlayerTracker");
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		sammyTimer.Start();
		waypoints = GetTree().GetNodesInGroup("Waypoints").Select(saar => saar as Marker3D).ToList();
		speedBoostParticles = GetNode<GpuParticles3D>("SammyOnSB");
		guardianParticles = GetNode<GpuParticles3D>("SammyOnG");
		currentState = SammyStates.Idle;
		waypointIndex = GD.RandRange(0, waypoints.Count - 1);
		waypointCount = waypoints.Count;
		targetPos = waypoints[waypointIndex].GlobalPosition;
		sammyNav.TargetPosition = targetPos;
		animSpeed = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? animSpeedNormal : animSpeedHard;
		sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
		sammyAnimTree.Set("parameters/Walking Blend/WalkSpeed/scale", animSpeed);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		speedBoostParticles.Emitting = Xalkomak.isSpeedBoostCollectedBySammy;
		guardianParticles.Emitting = Xalkomak.isGuardianCollectedBySammy;
		currentStateS = currentState.ToString();
		prevStateS = previousState.ToString();
		s_Velocity = Velocity.Length();
		trackerPos = playerTracker.GlobalPosition;
	}

	private void GetSpeedBoost(bool sammyHasSpeedBoost)
	{
		//GD.Print("Signal received from Game scene. ", sammyHasSpeedBoost);
		if (sammyHasSpeedBoost)
		{
			sammyTimer.Stop();
			UpdateState();
			currentState = SammyStates.SpeedBoosted;
			animSpeed = animSpeedSpeedBoost;
			sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
			sammyAnimTree.Set("parameters/conditions/run", true);
			sammyAnimTree.Set("parameters/conditions/walk", false);
			sammyAnimTree.Set("parameters/conditions/idle", false);
		}
		else
		{
			UpdateState();
			currentState = SammyStates.Patrolling;
		}
	}

	public void SetSpeedBoost(bool hasSpeedBoost)
	{
		if (hasSpeedBoost)
		{
			UpdateState();
			currentState = SammyStates.SpeedBoosted;
			animSpeed = animSpeedSpeedBoost;
			sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
			sammyAnimTree.Set("parameters/conditions/run", true);
			sammyAnimTree.Set("parameters/conditions/walk", false);
			sammyAnimTree.Set("parameters/conditions/idle", false);
		}
		else
		{
			UpdateState();
			currentState = SammyStates.Patrolling;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		s_Velocity = Velocity.Length();

		switch (currentState)
		{
			case SammyStates.Idle:
				moveSpeed = 0;
				Velocity = Vector3.Zero;
				sammyAnimTree.Set("parameters/conditions/idle", true);
				sammyAnimTree.Set("parameters/conditions/walk", false);        
				sammyAnimTree.Set("parameters/conditions/run", false);
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
					UpdateState();
					currentState = SammyStates.Idle;
					moveSpeed = 0;
					return;
				}
				NavigateAround((float)delta);
				break;
			case SammyStates.Hunting:                
				if (sammyNav.IsNavigationFinished() || !sammyNav.IsTargetReachable())
				{
					if (flag == 0)
					{
						SetTimer();
						flag++;
					}
					UpdateState();
					currentState = SammyStates.Idle;
					moveSpeed = 0;
					return;
				}
				NavigateAround((float)delta);
				break;
			case SammyStates.Stunned:
				moveSpeed = 0;
				Velocity = Vector3.Zero;
				return;
			case SammyStates.SpeedBoosted:
				if (sammyNav.IsNavigationFinished() || !sammyNav.IsTargetReachable())
				{
					RepathSpeedBoost();
					return;
				}
				NavigateAround((float)delta);
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
				animSpeed = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? animSpeedNormal : animSpeedHard;
				sammyAnimTree.Set("parameters/Walking Blend/WalkSpeed/scale", animSpeed);
				sammyAnimTree.Set("parameters/conditions/walk", true);
				sammyAnimTree.Set("parameters/conditions/idle", false);
				sammyAnimTree.Set("parameters/conditions/run", false);
				break;
			case SammyStates.Chasing:
				moveSpeed = (Xalkomak.difficulty == Xalkomak.Difficulty.Normal) ? runSpeed : runSpeedHard;
				nextPos = sammyNav.GetNextPathPosition();
				direction = GlobalPosition.DirectionTo(nextPos);
				TurnToDirection(delta);
				Velocity = direction * moveSpeed;
				animSpeed = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? animSpeedNormal : animSpeedHard;
				sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
				sammyAnimTree.Set("parameters/conditions/run", true);
				sammyAnimTree.Set("parameters/conditions/walk", false);
				sammyAnimTree.Set("parameters/conditions/idle", false);
				break;
			case SammyStates.Hunting:
				if (Xalkomak.difficulty == Xalkomak.Difficulty.Normal)
				{
					moveSpeed = runSpeed;
				}
				else
				{
					if (Xalkomak.isSpeedBoostCollectedBySammy)
					{
						moveSpeed = 7.64f;
					}
					else
					{
						moveSpeed = runSpeedHard;
					}
				}                                
				nextPos = sammyNav.GetNextPathPosition();
				direction = GlobalPosition.DirectionTo(nextPos);
				TurnToDirection(delta);
				Velocity = direction * moveSpeed;
				if (Xalkomak.difficulty == Xalkomak.Difficulty.Normal)
				{
					animSpeed = animSpeedNormal;
				}
				else
				{
					if (Xalkomak.isSpeedBoostCollectedBySammy)
					{
						animSpeed = animSpeedSpeedBoost;
					}
					else
					{
						animSpeed = animSpeedHard;
					}
				}
				sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
				sammyAnimTree.Set("parameters/conditions/run", true);
				sammyAnimTree.Set("parameters/conditions/walk", false);
				sammyAnimTree.Set("parameters/conditions/idle", false);
				break;
			case SammyStates.SpeedBoosted:
				moveSpeed = 7.64f;
				nextPos = sammyNav.GetNextPathPosition();
				direction = GlobalPosition.DirectionTo(nextPos);
				TurnToDirection(delta);
				Velocity = direction * moveSpeed;
				animSpeed = animSpeedSpeedBoost;
				sammyAnimTree.Set("parameters/Running Blend/Speed/scale", animSpeed);
				sammyAnimTree.Set("parameters/conditions/run", true);
				sammyAnimTree.Set("parameters/conditions/walk", false);
				sammyAnimTree.Set("parameters/conditions/idle", false);
				break;
			default:
				break;
		}
	}

	private void TurnToDirection(float delta)
	{
		Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y ,Mathf.Atan2(-Velocity.X, -Velocity.Z), delta * rotationAcceleration), Rotation.Z);
		//LookAt(new Vector3(direction.X, GlobalPosition.Y, direction.Z), Vector3.Up);
	}

	public void ChasePlayer(Vector3 goTo)
	{
		if (currentState != SammyStates.Chasing && currentState != SammyStates.SpeedBoosted)
		{
			UpdateState();
			currentState = SammyStates.Chasing;
		}
		targetPos = player.GlobalPosition;
		sammyNav.TargetPosition = targetPos;
		flag = 0;
		sammyTimer.Stop();
	}

	public void LostPlayer()
	{
		if (currentState != SammyStates.Hunting && currentState != SammyStates.SpeedBoosted)
		{
			UpdateState();
			currentState = SammyStates.Hunting;
		}
		targetPos = playerTracker.GlobalPosition;
		sammyNav.TargetPosition = targetPos;
	}

	void Repath()
	{
		sammyTimer.Stop();
		EmitSignal(SignalName.RandomizeNavigation);
		SetTimer();
		UpdateState();
		currentState = SammyStates.Patrolling;
		waypointIndex = GD.RandRange(0, waypoints.Count - 1);
		targetPos = waypoints[waypointIndex].GlobalPosition;
		sammyNav.TargetPosition = targetPos;
	}

	public void RepathSpeedBoost()
	{
		sammyTimer.Stop();
		EmitSignal(SignalName.RandomizeNavigation);
		UpdateState();
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
		UpdateState();
		currentState = SammyStates.Stunned;
		isStunned = true;
		sammyAnimTree.Set("parameters/conditions/idle", false);
		sammyAnimTree.Set("parameters/conditions/run", false);
		sammyAnimTree.Set("parameters/conditions/walk", false);
		sammyAnimTree.Set("parameters/conditions/gotStunned", true);
		sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", false);
		moveSpeed = 0f;
		Velocity = Vector3.Zero;
		jumpscareArea.SetDeferred("monitoring", false);
		sammyTimer.Stop();
	}

	public void RecoverFromStun()
	{
		sammyAnimTree.Set("parameters/conditions/gotStunned", false);
		sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", true);
		moveSpeed = 0;
		jumpscareArea.SetDeferred("monitoring", true);
		SetTimer();
	}

	private void OnHuntCountdownTimeout()
	{
		UpdateState();
		currentState = SammyStates.Hunting;
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
				sammyAnimTree.Set("parameters/conditions/walk", false);
				sammyAnimTree.Set("parameters/conditions/gotStunned", false);
				sammyAnimTree.Set("parameters/Stun State/conditions/stunEnded", false);
				jumpscareArea.SetDeferred("monitoring", false);
				jumpscareNum = GD.RandRange(1, 2);
				UpdateState();
				currentState = SammyStates.Idle;
				moveSpeed = 0;
				sammyNav.TargetPosition = GlobalPosition;
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
			UpdateState();
			currentState = SammyStates.Idle;
		}

		if(animName.Equals("Jumpscare" + jumpscareNum))
		{
			if(Xalkomak.livesRemaining <= 1)
			{
				Xalkomak.livesRemaining = 0;
				Xalkomak.livesLost++;
			}
			GetTree().ChangeSceneToFile(deathScreen);
		}
	}

	public void GoInvisible(bool invisible)
	{
		if(invisible)
		{
			sammyMesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
		}
		else
		{
			sammyMesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.On;
		}
		sammyMeshAnim.CurrentAnimation = invisible ? "Invisible" : "Visible";
		sammyMeshAnim.Play();
	}

	private void UpdateState()
	{
		previousState = currentState;
	}

	public void HoldInPlace()
	{
		UpdateState();
		currentState = SammyStates.Idle;
		sammyNav.TargetPosition = GlobalPosition;
	}
}
