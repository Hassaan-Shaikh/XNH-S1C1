using Godot;
using Godot.Collections;
using System;

public partial class GameControl : Node3D
{
	[Signal] public delegate void SammySpeedBoostedEventHandler(bool sammyHasSpeedBoost);

	[Export] public Area3D exitTrigger;
	[ExportCategory("References")]
	[ExportGroup("Prefabs")]
	[Export] public Player player;
	[Export] public SmilingSammy sammy;
	[Export] public PackedScene stunVFX;
	[Export] public PackedScene guardianVFX;
	[Export] public NavigationRegion3D navigationRegion;
	[ExportGroup("Light Maps")]
	[Export] public LightmapGI hardModeBlack;
    [ExportGroup("UI Elements")]
    [Export] public PauseMenu pauseMenu;
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

	public LevelLoader levelLoader;

	bool speedBoostUsed = false;
	bool stunUsed = false;
	bool guardianUsed = false;
	bool vanishUsed = false;
	bool isGamePaused = false;
	bool[] isThisSpotFree;

    const string pauseKey = "pause";
	const string endGamePath = "res://Scenes/EndGame.tscn";

    public override void _Ready()
    {
        base._Ready();
		levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
		//Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 3 : 1;
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
		hardModeBlack.Visible = Xalkomak.difficulty == Xalkomak.Difficulty.Hard;
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

		Xalkomak.isSpeedBoostCollected = false;
        Xalkomak.isStunCollected = false;
        Xalkomak.isGuardianCollected = false;
		Xalkomak.isVanishCollected = false;
        Xalkomak.isSpeedBoostCollectedBySammy = false;
        Xalkomak.isStunCollectedBySammy = false;
		Xalkomak.isGuardianCollectedBySammy = false;
        Xalkomak.isVanishCollectedBySammy = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		if (Input.IsActionJustPressed(pauseKey))
		{
			PauseGame();
		}
    }

    public void StartSpeedBoostTimer()
	{
		speedBoostTimer.Start();
		speedBoostUsed = true;
		speedBoostparticles.GlobalPosition = GetNode<SpeedBoost>("SpeedBoost").GlobalPosition;
		speedBoostparticles.Emitting = true;
		GD.Print(Xalkomak.isSpeedBoostCollectedBySammy);
		if (Xalkomak.isSpeedBoostCollectedBySammy)
		{

			EmitSignal(SignalName.SammySpeedBoosted, true);
			//sammy.RepathSpeedBoost();
		}
	}

	public void StartStunTimer()
	{
		StunExplosion stunInstance = stunVFX.Instantiate<StunExplosion>();
		AddChild(stunInstance);
		stunInstance.GlobalPosition = GetNode<Stun>("Stun").GlobalPosition;
		if (Xalkomak.isStunCollected)
		{
			sammy.GetStunned();
		}
		if(Xalkomak.isStunCollectedBySammy)
		{
			player.SetStunned(Xalkomak.isStunCollectedBySammy);
		}
		stunTimer.Start();
		stunUsed = true;
        stunParticles.GlobalPosition = GetNode<Stun>("Stun").GlobalPosition;
        stunParticles.Emitting = true;
    }

    public void StartStunTimer(string runeName)
    {        
		GuardianShield guardianShield = guardianVFX.Instantiate<GuardianShield>();
		AddChild(guardianShield);
		guardianShield.GlobalPosition = player.GlobalPosition;
        stunTimer.Start();
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
		//if(Xalkomak.isVanishCollectedBySammy)
		//{
		//	sammy.GoInvisible(true);
		//}
		vanishTimer.Start();
		vanishUsed = true;
        vanishParticles.GlobalPosition = GetNode<Vanish>("Vanish").GlobalPosition;
        vanishParticles.Emitting = true;
    }

	private void OnSpeedBoostExpired()
	{
		if(Xalkomak.isSpeedBoostCollected)
		{
            player.SetSpeedBoost(false);
        }
		//GD.Print(Xalkomak.isSpeedBoostCollectedBySammy);
        if (Xalkomak.isSpeedBoostCollectedBySammy)
        {
            EmitSignal(SignalName.SammySpeedBoosted, false);
        }
        Xalkomak.isSpeedBoostCollected = false;
		Xalkomak.isSpeedBoostCollectedBySammy = false;		
    }

	private void OnStunExpired()
	{
        if (Xalkomak.isStunCollected)
        {
            player.SetStun(false);
        }
        if (Xalkomak.isStunCollectedBySammy)
        {
            player.SetStunned(false);
        }
        Xalkomak.isStunCollected = false;
		Xalkomak.isStunCollectedBySammy = false;
		sammy.RecoverFromStun();
	}

    private void OnGuardianExpired()
	{
        if (Xalkomak.isGuardianCollected)
        {
            player.SetGuardian(false);
        }
        Xalkomak.isGuardianCollected = false;
		Xalkomak.isGuardianCollectedBySammy = false;				
	}

	private void OnVanishExpired()
	{
        if (Xalkomak.isVanishCollectedBySammy)
        {
            sammy.GoInvisible(false);
        }
		if(Xalkomak.isVanishCollected)
		{
            player.SetVanish(false);
        }
        Xalkomak.isVanishCollected = false;
		Xalkomak.isVanishCollectedBySammy = false;		
    }

	private void RepositionSB()
	{
		if (!speedBoostUsed) //Indicates that Speed Boost has not been used
		{
			int newPosIndex = GD.RandRange((int)0, (int)powerRunePoints.Length - 1);
			if (Xalkomak.isThisSpotOccupied[newPosIndex] || Mathf.Abs(powerRunePoints[newPosIndex].GlobalPosition.DistanceTo(player.GlobalPosition)) < 5f)
			{
				if (Xalkomak.isThisSpotOccupied[newPosIndex])
				{
					FreeSpots();
				}
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
            if (Xalkomak.isThisSpotOccupied[newPosIndex] || Mathf.Abs(powerRunePoints[newPosIndex].GlobalPosition.DistanceTo(player.GlobalPosition)) < 5f)
            {
                if (Xalkomak.isThisSpotOccupied[newPosIndex])
                {
                    FreeSpots();
                }
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
            if (Xalkomak.isThisSpotOccupied[newPosIndex] || Mathf.Abs(powerRunePoints[newPosIndex].GlobalPosition.DistanceTo(player.GlobalPosition)) < 5f)
            {
                if (Xalkomak.isThisSpotOccupied[newPosIndex])
                {
                    FreeSpots();
                }
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
                if (Xalkomak.isThisSpotOccupied[newPosIndex])
                {
                    FreeSpots();
                }
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
		navigationRegion.EnterCost = 100;//GD.RandRange(1, 200);
		navigationRegion.TravelCost = 100;//GD.RandRange(1, 200);
		/*GD.Print("Travel Cost: " + navigationRegion.TravelCost);
        GD.Print("Enter Cost: " + navigationRegion.EnterCost);*/
    }

	private void OnGuardianStoppedSammy()
	{
		GD.Print("Guardian has stopped Sammy.");
		StartStunTimer("Guardian");
		guardianTimer.Stop();
        guardianTele.Stop();
        OnGuardianExpired();
	}

	public void PauseGame()
	{
		//Tween tween = GetTree().CreateTween();
		isGamePaused = !isGamePaused;		

		if(isGamePaused)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			//tween.TweenProperty(pauseMenu, "modulate", new Color(1, 1, 1, 1), 1.0);
        }
		else
		{
            Input.MouseMode = Input.MouseModeEnum.Captured;
            //tween.TweenProperty(pauseMenu, "modulate", new Color(1, 1, 1, 0), 1.0);
        }

		pauseMenu.FadePauseMenu(isGamePaused);		
		//pauseMenu.Visible = isGamePaused;
		//GetTree().Paused = isGamePaused;
    }

	private void OnExitAreaBodyEntered(Node3D body)
	{
		if (body.IsInGroup("Player") && Xalkomak.documentsCollected >= 7)
		{
			levelLoader.SwitchScene(endGamePath);
		}
	}
}
