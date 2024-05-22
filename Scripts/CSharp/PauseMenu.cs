using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] public GameControl gameScene;
    [Export] public LevelLoader levelLoader;
    [Export] public AnimationPlayer fadeAnim;
    [Export] public PackedScene confirmQuit;
    [Export] public PackedScene optionsMenu;

    public UserPrefs userPrefs;

    private bool confirmingQuit;
    private byte source;

    const string mainMenuPath = "res://Scenes/MainMenu.tscn";

    public override void _Ready()
    {
        base._Ready();
        userPrefs = UserPrefs.LoadOrCreate();
        gameScene = GetTree().GetNodesInGroup("Game")[0] as GameControl;
        levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
        fadeAnim = GetNode<AnimationPlayer>("PauseFade");

        switch (Xalkomak.currentScreenIndex)
        {
            case 0: // Fulscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 1: // Windowed
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 2: // Borderless Windowed
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            case 3: // Exclusive Fullscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
        }
        DisplayServer.WindowSetSize(OptionsMenu.GetValues()[Xalkomak.currentResIndex]);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(userPrefs.soundAudioLevel));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), Mathf.LinearToDb(userPrefs.musicAudioLevel));
        Engine.MaxFps = Xalkomak.gameFrameRate;
        Xalkomak.camSens = userPrefs.sensitivityLevel;
    }

    private void OnResumePressed()
    {
        gameScene.PauseGame();
    }

    private void OnOptionsPressed()
    {
        OptionsMenu opMenu = optionsMenu.Instantiate<OptionsMenu>();
        AddChild(opMenu);
        ProcessMode = ProcessModeEnum.Disabled;
        opMenu.TreeExiting += () =>
        {
            opMenu.SettingsValueChanged -= OnOptionMenuSettingChanged;
        };
        opMenu.SettingsValueChanged += OnOptionMenuSettingChanged;
        opMenu.animPlayer.AnimationFinished += (StringName animName) =>
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.Always;
            }
            if (animName.Equals("PopOut"))
                opMenu.QueueFree();
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

    private void OnOptionMenuSettingChanged(string settingName, Variant settingValue)
    {
        switch (settingName)
        {
            case "BGMSlider":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.musicAudioLevel = (float)settingValue;
                userPrefs.SavePrefs();
                break;
            case "SFXSlider":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.soundAudioLevel = (float)settingValue;
                userPrefs.SavePrefs();
                break;
            case "ResOption":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.resolutionIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "SizeOption":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.screenSizeIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "FPSOption":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.gameFps = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "FPS Index":
                userPrefs.fpsIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "SenSlider":
                userPrefs.sensitivityLevel = (float)settingValue;
                userPrefs.SavePrefs();
                break;
            default:
                GD.PrintErr("Setting node ", settingName, " is unavailable or not defined.");
                break;
        }
    }
}
