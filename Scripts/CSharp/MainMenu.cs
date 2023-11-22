using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] public LevelLoader levelLoader;

    const string gamePath = "res://Scenes/Game.tscn";

    public override void _Ready()
    {
        base._Ready();
        levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
    }
    private void OnPlayButtonPressed()
    {
        Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Hard ? 1 : 3;
        Xalkomak.playerCanControl = true;
        Xalkomak.documentsCollected = 0;
        for (int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
        {
            Xalkomak.isDocumentCollected[i] = false;
        }
        levelLoader.SwitchScene(gamePath);
    }

    private void OnOptionsButtonPressed()
    {

    }

    private void OnExtrasButtonPressed()
    {

    }

    private void OnCreditsButtonPressed()
    {

    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
