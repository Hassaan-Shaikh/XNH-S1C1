using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] public GameControl gameScene;
    [Export] public LevelLoader levelLoader;
    [Export] public AnimationPlayer fadeAnim;
    [Export] public PackedScene confirmQuit;
    [Export] public PackedScene optionsMenu;

    private bool confirmingQuit;
    private byte source;

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
        Node opMenu = optionsMenu.Instantiate();
        AddChild(opMenu);
        ProcessMode = ProcessModeEnum.Disabled;
        AnimationPlayer opMenuPopAnim = opMenu.GetNode<AnimationPlayer>("PopupAnim");
        opMenuPopAnim.AnimationFinished += (StringName animName) =>
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.Always;
                GD.Print("Animation finished.");
            }
        };
        opMenu.GetNode<Button>("PanelContainer/BackButton").Pressed += () =>
        {
            opMenuPopAnim.Play("PopOut");
        };
    }

    private void OnQuitToTitlePressed()
    {
        PromptConfirmQuit(0);
    }

    private void OnQuitToDesktopPressed()
    {
        PromptConfirmQuit(1);
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

    private void PromptConfirmQuit(byte sourceButton)
    {
        ConfirmQuit quitBox = confirmQuit.Instantiate() as ConfirmQuit;
        AddChild(quitBox);
        quitBox.QuitConfirmed += (bool confirmedQuit) =>
        {
            confirmingQuit = confirmedQuit;
        };
        ProcessMode = ProcessModeEnum.Disabled;
        quitBox.popAnim.AnimationFinished += (StringName animName) =>
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.WhenPaused;
            }
            if (animName.Equals("PopOut") && confirmingQuit)
            {
                switch(sourceButton)
                {
                    case 0:
                        quitBox.QueueFree();
                        levelLoader.SwitchScene(mainMenuPath);
                        break;
                    case 1:
                        quitBox.QueueFree();
                        GetTree().Quit();
                        break;
                    default:
                        break;
                }              
            }
        };
    }
}
