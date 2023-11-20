using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] public GameControl gameScene;
    [Export] public LevelLoader levelLoader;

    const string mainMenuPath = "res://Scenes/MainMenu.tscn";

    private void OnResumePressed()
    {
        gameScene.PauseGame();
    }

    private void OnOptionsPressed()
    {

    }

    private void OnQuitToTitlePressed()
    {
        levelLoader.SwitchScene(mainMenuPath);
    }

    private void OnQuitToDesktopPressed()
    {
        GetTree().Quit();
    }
}
