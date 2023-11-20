using Godot;
using System;

public partial class DebugPanel : PanelContainer
{
    [Export] public Player parent;
    [ExportGroup("Attributes")]
    [Export] public Label FPS;
    [Export] public Label playerSpeed;
    [Export] public Label isPlayerExhausted;
    [Export] public Label sammySpeed;
    [Export] public Label sammyState;
    [Export] public Label currentDifficulty;
    [Export] public Label sammyWaypointIndex;
    [Export] public Label waypointCount;
    [Export] public Label angleToPlayer;
    [Export] public Label sammyHasSB;
    [Export] public Label sammyHasS;
    [Export] public Label sammyHasG;
    [Export] public Label sammyHasV;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        parent = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("debug"))
        {
            Visible = !Visible;
        }
        if(!Visible)
        {
            return;
        }
        FPS.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
        playerSpeed.Text = "Player's Speed: " + parent.GetPlayerSpeed();
        isPlayerExhausted.Text = "Exhausted: " + parent.GetExhaustedState();
        sammySpeed.Text = "Sammy's Speed: " + SmilingSammy.GetSammySpeed();
        sammyState.Text = "Sammy's Current State: " + SmilingSammy.GetCurrentState();
        currentDifficulty.Text = "Difficulty: " + Xalkomak.difficulty.ToString();
        sammyWaypointIndex.Text = "Lives Remaining: " + Xalkomak.livesRemaining;
        waypointCount.Text = "No. Of Waypoints: " + SmilingSammy.GetWaypointCount();
        angleToPlayer.Text = "Sammy Ignoring Check: " + SammyRay.ignoreCheck.ToString();
        sammyHasSB.Text = "Sammy has Speed Boost: " + Xalkomak.isSpeedBoostCollectedBySammy;
        sammyHasS.Text = "Sammy has Stun: " + Xalkomak.isStunCollectedBySammy;
        sammyHasG.Text = "Sammy has Guardian: " + Xalkomak.isGuardianCollectedBySammy;
        sammyHasV.Text = "Sammy has Vanish: " + Xalkomak.isVanishCollectedBySammy;
    }
}
