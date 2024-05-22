using Godot;
using System.Collections.Generic;

public partial class OptionsMenu : Control
{
    [Signal] public delegate void SettingsValueChangedEventHandler(string optionName, Variant value);

    [Export] public PackedScene confirmQuitScene;
    [ExportGroup("References")]
    [Export] public HSlider bgmSlider, sfxSlider, sensitivitySlider;
    [Export] public Button backButton, applyButton;
    [Export] public OptionButton resolutionButton, screenSizeButton, fpsButton;

    public AnimationPlayer animPlayer;
    public TabContainer tabContainer;

    public static Godot.Collections.Dictionary<string, Vector2I> screenResolutions = new Godot.Collections.Dictionary<string, Vector2I>
    {
        { "1280 x 720", new Vector2I(1280, 720) },
        { "1366 x 768", new Vector2I(1366, 768) },
        { "1440 x 810", new Vector2I(1440, 810) },
        { "1600 x 900", new Vector2I(1600, 900) },
        { "1920 x 1080", new Vector2I(1920, 1080) }
    };
    public static int[] frameRates = 
    {
        30,
        60,
        0
    };
    List<string> screenSizes = new List<string>
    {
        "Fullscreen",
        "Windowed",
        "Borderless Windowed",
        "Exclusive Fullscreen"
    };
    Godot.Collections.Dictionary<string, Variant> audioSettings;
    Godot.Collections.Dictionary<string, Variant> videoSettings;
    Godot.Collections.Dictionary<string, Variant> controlSettings;
    int previousResolutionIndex;
    int previousScreenSizeIndex;

    public override void _Ready()
    {
        base._Ready();
        audioSettings = UserConfig.GetAudioSettings();
        videoSettings = UserConfig.GetVideoSettings();
        controlSettings = UserConfig.GetControlSettings();
        //tabContainer = GetNode<TabContainer>("PanelContainer/Panel/TabContainer");
        //tabContainer.CurrentTab = Xalkomak.currentTabIndex;
        animPlayer = GetNode<AnimationPlayer>("PopupAnim");
        foreach (var item in screenResolutions)
        {
            resolutionButton.AddItem(item.Key);
        }
        foreach (string item in screenSizes)
        {
            screenSizeButton.AddItem(item);
        }
        foreach (int item in frameRates)
        {
            fpsButton.AddItem(item == 0 ? "Unlimited" : item.ToString() + " FPS");
        }
        previousResolutionIndex = (int)videoSettings["resolution_index"]; //Xalkomak.currentResIndex;
        previousScreenSizeIndex = (int)videoSettings["screen_index"]; //Xalkomak.currentScreenIndex;
        resolutionButton.Selected = previousResolutionIndex;
        screenSizeButton.Selected = previousScreenSizeIndex;
        fpsButton.Selected = (int)videoSettings["fps_index"]; //Xalkomak.fpsIndex;
        sensitivitySlider.Value = (float)controlSettings["mouse_sensitivity"]; //Xalkomak.camSens;
        bgmSlider.Value = (float)audioSettings["music_volume"];
        sfxSlider.Value = (float)audioSettings["sound_volume"];
        //applyButton.Disabled = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        applyButton.Disabled = (int)videoSettings["resolution_index"] == previousResolutionIndex && (int)videoSettings["screen_index"] == previousScreenSizeIndex;
    }

    private void OnBgmSliderValueChanged(float value)
    {
        //EmitSignal(SignalName.SettingsValueChanged, bgmSlider.Name, value);
        UserConfig.SaveAudioSettings("music_volume", value);
    }

    private void OnSfxSliderValueChanged(float value)
    {
        //EmitSignal(SignalName.SettingsValueChanged, sfxSlider.Name, value);
        UserConfig.SaveAudioSettings("sound_volume", value);
    }

    private void OnResOptionItemSelected(int index)
    {
        //DisplayServer.WindowSetSize(GetValues()[index]);
        //GD.Print(GetValues()[index]);
        //applyButton.Disabled = false;
        videoSettings["resolution_index"] = index;
        //EmitSignal(SignalName.SettingsValueChanged, resolutionButton.Name, index);
    }

    public static List<Vector2I> GetValues()
    {
        List<Vector2I> values = new List<Vector2I>();
        foreach (Vector2I item in screenResolutions.Values)
        {
            values.Add(item);
        }
        return values;
    }

    private void OnSizeOptionItemSelected(int index)
    {
        videoSettings["screen_index"] = index;
        //applyButton.Disabled = false;
    }

    private void OnApplyVideoSettingsPressed()
    {
        switch ((int)videoSettings["screen_index"])
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
        DisplayServer.WindowSetSize(GetValues()[(int)videoSettings["resolution_index"]]);
        UserConfig.SaveVideoSettings("resolution_index", (int)videoSettings["resolution_index"]);
        UserConfig.SaveVideoSettings("screen_index", (int)videoSettings["screen_index"]);
        //EmitSignal(SignalName.SettingsValueChanged, resolutionButton.Name, Xalkomak.currentResIndex);
        //EmitSignal(SignalName.SettingsValueChanged, screenSizeButton.Name, Xalkomak.currentScreenIndex);
        previousResolutionIndex = (int)videoSettings["resolution_index"];
        previousScreenSizeIndex = (int)videoSettings["screen_index"];
        //applyButton.Disabled = true;
    }

    private void OnBackButtonPressed()
    {
        if (applyButton.Disabled)
        {
            animPlayer.Play("PopOut");
            return;
        }
        ConfirmQuit quitBox = confirmQuitScene.Instantiate<ConfirmQuit>();
        AddChild(quitBox);
        quitBox.title.Text = "\n[center][u]Unsaved Changes[/u][/center]";
        quitBox.quitMessage.Text = "[center]Video settings not applied. Leaving now will reset them to the previously set values.[/center]";
        quitBox.confirm.Text = "OK";
        quitBox.cancel.Text = "Go Back";
        bool confirmingQuit = false;
        quitBox.QuitConfirmed += (bool quitConfirmed) =>
        {
            confirmingQuit = quitConfirmed;
        };
        ProcessMode = ProcessModeEnum.Disabled;
        quitBox.popAnim.AnimationFinished += (StringName animName) =>
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.Always;
            }
            if (animName.Equals("PopOut"))
            {
                if (confirmingQuit)
                {
                    Xalkomak.currentResIndex = previousResolutionIndex;
                    Xalkomak.currentScreenIndex = previousScreenSizeIndex;
                    animPlayer.Play("PopOut");
                }
                quitBox.QueueFree();
            }
        };
    }

    private void OnFpsOptionItemSelected(int index)
    {
        videoSettings["frames_per_second"] = fpsButton.GetItemText(index).Equals("Unlimited") ? 0 : int.Parse(fpsButton.GetItemText(index).TrimSuffix(" FPS"));
        //GD.Print(Xalkomak.gameFrameRate);
        videoSettings["fps_index"] = index;
        Engine.MaxFps = (int)videoSettings["frames_per_second"];
        UserConfig.SaveVideoSettings("fps_index", index);
        UserConfig.SaveVideoSettings("frames_per_second", (int)videoSettings["frames_per_second"]);
        //EmitSignal(SignalName.SettingsValueChanged, fpsButton.Name, Xalkomak.gameFrameRate);
        //EmitSignal(SignalName.SettingsValueChanged, "FPS Index", Xalkomak.fpsIndex);
    }

    private void OnSenSliderValueChanged(float value)
    {
        controlSettings["mouse_sensitivity"] = value;
        Xalkomak.camSens = value;
        UserConfig.SaveControlSettings("mouse_sensitivity", value);
        //EmitSignal(SignalName.SettingsValueChanged, sensitivitySlider.Name, Xalkomak.camSens);
    }
}
