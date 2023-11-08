using Godot;
using System;

public partial class GameControl : Node3D
{
	[ExportGroup("References")]
	[ExportSubgroup("Characters")]
	[Export] public Player player;
	[Export] public PackedScene stunVFX;
    [ExportSubgroup("Power Rune Timers")] 
	[Export] public Timer speedBoostTimer;
	[Export] public Timer stunTimer;
	[Export] public Timer guardianTimer;
	[Export] public Timer vanishTimer;
    [ExportSubgroup("Power Rune Paricles")]
	[Export] public GpuParticles3D speedBoostparticles;
    [Export] public GpuParticles3D stunparticles;
    [Export] public GpuParticles3D guardianparticles;
    [Export] public GpuParticles3D vanishparticles;

    public override void _Ready()
    {
        base._Ready();
		player = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public void StartSpeedBoostTimer()
	{
		speedBoostTimer.Start();
		speedBoostparticles.GlobalPosition = GetNode<SpeedBoost>("SpeedBoost").GlobalPosition;
		speedBoostparticles.Emitting = true;
	}

	public void StartStunTimer()
	{
		Stun stunRune = GetNode<Stun>("Stun");
        Node3D stunVFXInstance = stunVFX.Instantiate() as Node3D;
		AddChild(stunVFXInstance);
		stunVFXInstance.GlobalPosition = new Vector3(stunRune.GlobalPosition.X, 0, stunRune.GlobalPosition.Z);
		stunTimer.Start();
	}
	
	public void StartGuardianTimer()
	{
        guardianTimer.Start();
    }

	private void OnSpeedBoostExpired()
	{
		Xalkomak.isSpeedBoostCollected = false;
		player.SetSpeedBoost(false);
	}

	private void OnStunExpired()
	{
		Xalkomak.isStunCollected = false;
	}

    private void OnGuardianExpired()
	{
		Xalkomak.isGuardianCollected = false;
		player.SetGuardian(false);
	}

	private void OnVanishExpired()
	{
		Xalkomak.isVanishCollected = false;
	}
}
