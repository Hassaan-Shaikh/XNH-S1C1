using Godot;
using Godot.Collections;
using System;

public partial class GameControl : Node3D
{
	[ExportCategory("References")]
	[ExportGroup("Prefabs")]
	[Export] public Player player;
	[Export] public SmilingSammy sammy;
	[Export] public PackedScene stunVFX;
	[Export] public NavigationRegion3D navigationRegion;
	[ExportGroup("Power Runes")]
	[Export] public SpeedBoost speedBoost;
	[Export] public Stun stun;
	[Export] public Guardian guardian;
	[Export] public Vanish vanish;
    [ExportSubgroup("Power Rune Timers")] 
	[Export] public Timer speedBoostTimer;
	[Export] public Timer stunTimer;
	[Export] public Timer guardianTimer;
	[Export] public Timer vanishTimer;
    [ExportSubgroup("Power Rune Telport Timers")]
    [Export] public Timer speedBoostTele;
    [Export] public Timer stunTele;
    [Export] public Timer guardianTele;
    [Export] public Timer vanishTele;
    [ExportSubgroup("Power Rune Paricles")]
	[Export] public CpuParticles3D speedBoostparticles;
    [Export] public CpuParticles3D stunParticles;
    [Export] public CpuParticles3D guardianParticles;
    [Export] public CpuParticles3D vanishParticles;
	[ExportSubgroup("Power Rune Locations")]
	[Export] public Node3D[] powerRunePoints;

	bool speedBoostUsed = false;
	bool stunUsed = false;
	bool guardianUsed = false;
	bool vanishUsed = false;

	bool[] isThisSpotFree;

    public override void _Ready()
    {
        base._Ready();
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		if(powerRunePoints.Length == 0)
		{
            Array<Node> points = GetTree().GetNodesInGroup("PowerRuneLocation");
			for(int i = 0; i < points.Count; i++)
			{
				powerRunePoints[i] = (Node3D)points[i];
			}
		}
		Xalkomak.isThisSpotOccupied = new bool[powerRunePoints.Length];
		isThisSpotFree = new bool[powerRunePoints.Length];
    }

    public void StartSpeedBoostTimer()
	{
		speedBoostTimer.Start();
		speedBoostUsed = true;
		speedBoostparticles.GlobalPosition = GetNode<SpeedBoost>("SpeedBoost").GlobalPosition;
		speedBoostparticles.Emitting = true;
	}

	public void StartStunTimer()
	{
		StunExplosion stunInstance = stunVFX.Instantiate<StunExplosion>();
		AddChild(stunInstance);
		stunInstance.GlobalPosition = GetNode<Stun>("Stun").GlobalPosition;
		sammy.GetStunned();
		stunTimer.Start();
		stunUsed = true;
        stunParticles.GlobalPosition = GetNode<Stun>("Stun").GlobalPosition;
        stunParticles.Emitting = true;
    }
	
	public void StartGuardianTimer()
	{
        guardianTimer.Start();
		guardianUsed = true;
        guardianParticles.GlobalPosition = GetNode<Guardian>("Guardian").GlobalPosition;
        guardianParticles.Emitting = true;
    }

	public void StartVanishTimer()
	{
		vanishTimer.Start();
		vanishUsed = true;
        vanishParticles.GlobalPosition = GetNode<Vanish>("Vanish").GlobalPosition;
        vanishParticles.Emitting = true;
    }

	private void OnSpeedBoostExpired()
	{
		Xalkomak.isSpeedBoostCollected = false;
		Xalkomak.isSpeedBoostCollectedBySammy = false;
		player.SetSpeedBoost(false);
	}

	private void OnStunExpired()
	{
		Xalkomak.isStunCollected = false;
		Xalkomak.isStunCollectedBySammy = false;
		sammy.RecoverFromStun();
	}

    private void OnGuardianExpired()
	{
		Xalkomak.isGuardianCollected = false;
		Xalkomak.isGuardianCollectedBySammy = false;
		player.SetGuardian(false);
	}

	private void OnVanishExpired()
	{
		Xalkomak.isVanishCollected = false;
		Xalkomak.isVanishCollectedBySammy = false;
		player.SetVanish(false);
	}

	private void RepositionSB()
	{
		if (!speedBoostUsed) //Indicates that Speed Boost has not been used
		{
			int newPosIndex = GD.RandRange((int)0, (int)powerRunePoints.Length - 1);
			if (Xalkomak.isThisSpotOccupied[newPosIndex])
			{
                FreeSpots();
                speedBoostTele.WaitTime = GD.RandRange(30.0f, 45.0f);
                speedBoostTele.Start();
                return;
			}
			speedBoost.GlobalPosition = powerRunePoints[newPosIndex].GlobalPosition;
			Xalkomak.isThisSpotOccupied[newPosIndex] = true;
			speedBoostTele.WaitTime = GD.RandRange(30.0f, 45.0f);
			speedBoostTele.Start();
		}
	}

    private void RepositionS()
    {
		if(!stunUsed)
		{
            int newPosIndex = GD.RandRange((int)0, (int)powerRunePoints.Length - 1);
            if (Xalkomak.isThisSpotOccupied[newPosIndex])
            {
                FreeSpots();
                stunTele.WaitTime = GD.RandRange(30.0f, 45.0f);
                stunTele.Start();
                return;
            }
            stun.GlobalPosition = powerRunePoints[newPosIndex].GlobalPosition;
            Xalkomak.isThisSpotOccupied[newPosIndex] = true;
            stunTele.WaitTime = GD.RandRange(30.0f, 45.0f);
            stunTele.Start();
        }
    }

    private void RepositionG()
    {
		if(!guardianUsed)
		{

            int newPosIndex = GD.RandRange((int)0, (int)powerRunePoints.Length - 1);
            if (Xalkomak.isThisSpotOccupied[newPosIndex])
            {
                FreeSpots();
                guardianTele.WaitTime = GD.RandRange(30.0f, 45.0f);
                guardianTele.Start();
                return;
            }
            guardian.GlobalPosition = powerRunePoints[newPosIndex].GlobalPosition;
            Xalkomak.isThisSpotOccupied[newPosIndex] = true;
            guardianTele.WaitTime = GD.RandRange(30.0f, 45.0f);
            guardianTele.Start();
        }
    }

    private void RepositionV()
    {
		if(!vanishUsed)
		{
            int newPosIndex = GD.RandRange((int)0, (int)powerRunePoints.Length - 1);
            if (Xalkomak.isThisSpotOccupied[newPosIndex] || Mathf.Abs(powerRunePoints[newPosIndex].GlobalPosition.DistanceTo(player.GlobalPosition)) < 5f)
            {
                FreeSpots();
                vanishTele.WaitTime = GD.RandRange(30.0f, 45.0f);
                vanishTele.Start();
                return;
            }
            vanish.GlobalPosition = powerRunePoints[newPosIndex].GlobalPosition;
            Xalkomak.isThisSpotOccupied[newPosIndex] = true;
            vanishTele.WaitTime = GD.RandRange(30.0f, 45.0f);
            vanishTele.Start();
        }
    }

	private void FreeSpots()
	{
		for(int i = 0; i < powerRunePoints.Length; i++)
		{
			Xalkomak.isThisSpotOccupied[i] = false;
		}
	}

	private void RandomizeNavigation()
	{
		navigationRegion.EnterCost = GD.RandRange(0, 100);
        navigationRegion.TravelCost = GD.RandRange(0, 100);
		GD.Print("Travel Cost: " + navigationRegion.TravelCost);
        GD.Print("Enter Cost: " + navigationRegion.EnterCost);
    }
}
