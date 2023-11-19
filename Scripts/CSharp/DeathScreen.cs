using Godot;
using System;

public partial class DeathScreen : Control
{
    [Export] LevelLoader levelLoader;
    [Export] Label livesRemaining;
    [Export] Label gameOver;
    [Export] Button retryButton;
    [Export] Button quitButton;
    [Export] AnimationPlayer deathAnim;

    int counter = 0;

    const string game = "res://Scenes/Game.tscn";
    const string mainMenu = "res://Scenes/MainMenu.tscn";

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        livesRemaining.Visible = Xalkomak.livesRemaining > 0;
        gameOver.Visible = Xalkomak.livesRemaining == 0;
        retryButton.Visible = Xalkomak.livesRemaining == 0;
        quitButton.Visible = Xalkomak.livesRemaining == 0;

        if (Xalkomak.livesRemaining > 0)
        {
            deathAnim.Play("LivesRemaining");
        }
        else
        {
            deathAnim.Play("GameOver");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        livesRemaining.Text = "Lives Remaining: " + Xalkomak.livesRemaining;
    }

    public void UpdateLifeCounter()
    {
        Xalkomak.livesRemaining -= 1;
        Xalkomak.playerCanControl = true;
    }

    private void OnDeathAnimAnimationFinished(string animName)
    {
        if(animName == "LivesRemaining")
        {
            levelLoader.SwitchScene(game);
        }
    }

    public void OnRetryButtonPressed()
    {
        Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 3 : 1;
        Xalkomak.playerCanControl = true;
        Xalkomak.documentsCollected = 0;
        for(int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
        {
            Xalkomak.isDocumentCollected[i] = false;
        }
        levelLoader.SwitchScene(game);
    }

    public void OnQuitButtonPressed()
    {
        GetTree().Quit();
        //levelLoader.SwitchScene(mainMenu);
    }

    public void ReleaseMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
