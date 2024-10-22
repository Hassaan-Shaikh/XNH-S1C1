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
    [Export] public Label trackerPos;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        parent = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("debug") && OS.IsDebugBuild())
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
        sammySpeed.Text = "Sammy's Speed: " + SmilingSammy.s_Velocity;
        sammyState.Text = "Sammy's Current State: " + SmilingSammy.currentStateS;
        currentDifficulty.Text = "Difficulty: " + Xalkomak.difficulty.ToString();
        sammyWaypointIndex.Text = "Sammy's Previous State: " + SmilingSammy.prevStateS;
        waypointCount.Text = "Sammy's Waypoint Index: " + SmilingSammy.waypointIndex;
        angleToPlayer.Text = "Sammy Ignoring Check: " + SammyRay.ignoreCheck.ToString();
        sammyHasSB.Text = "Sammy has Speed Boost: " + Xalkomak.isSpeedBoostCollectedBySammy;
        sammyHasS.Text = "Sammy has Stun: " + Xalkomak.isStunCollectedBySammy;
        sammyHasG.Text = "Sammy has Guardian: " + Xalkomak.isGuardianCollectedBySammy;
        sammyHasV.Text = "Sammy has Vanish: " + Xalkomak.isVanishCollectedBySammy;
        trackerPos.Text = "Tracker's position: " + SmilingSammy.trackerPos;
    }
}
