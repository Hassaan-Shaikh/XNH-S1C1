using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] public GameControl gameScene;
    [Export] public LevelLoader levelLoader;
    [Export] public AnimationPlayer fadeAnim;

    const string mainMenuPath = "res://Scenes/MainMenu.tscn";

    public override void _Ready()
    {
        gameScene = GetTree().GetNodesInGroup("Game")[0] as GameControl;
        levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
        fadeAnim = GetNode<AnimationPlayer>("PauseFade");
    }

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

    public void ImmediatePause()
    {
        GetTree().Paused = true;
    }

    public void FadePauseMenu(bool resumed)
    {
        fadeAnim.CurrentAnimation = resumed ? "PauseFadeIn" : "PauseFadeOut";
        fadeAnim.Play();
        fadeAnim.AnimationFinished += (StringName animName) =>
        {
            if(animName == "PauseFadeOut")
                GetTree().Paused = resumed;
        };
    }
}
