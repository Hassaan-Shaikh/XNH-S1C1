using Godot;
using System.Collections.Generic;

public partial class OptionsMenu : Control
{
    [Signal] public delegate void SettingsValueChangedEventHandler(string optionName, Variant value);

    [Export] public PackedScene confirmQuitScene;
    [Export] public VolumeSlider bgmSlider;
    [Export] public VolumeSlider sfxSlider;

    public AnimationPlayer animPlayer;
    public TabContainer tabContainer;
    public Button backButton;
    public OptionButton resolutionButton;
    public OptionButton screenSizeButton;
    public CheckBox vSyncCheckBox;
    public OptionButton fpsButton;
    public Button applyButton;

    public static Godot.Collections.Dictionary<string, Vector2I> screenResolutions = new Godot.Collections.Dictionary<string, Vector2I>
    {
        { "1280 x 720", new Vector2I(1280, 720) },
        { "1366 x 768", new Vector2I(1366, 768) },
        { "1440 x 810", new Vector2I(1440, 810) },
        { "1600 x 900", new Vector2I(1600, 900) },
        { "1920 x 1080", new Vector2I(1920, 1080) },
        { "3840 x 2160", new Vector2I(3840, 2160) }
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
    int previousResolutionIndex;
    int previousScreenSizeIndex;

    public override void _Ready()
    {
        base._Ready();
        tabContainer = GetNode<TabContainer>("PanelContainer/Panel/TabContainer");
        tabContainer.CurrentTab = Xalkomak.currentTabIndex;
        animPlayer = GetNode<AnimationPlayer>("PopupAnim");
        applyButton = GetNode<Button>("PanelContainer/Panel/TabContainer/Video/MarginContainer/ApplyVideoSettings");
        backButton = GetNode<Button>("PanelContainer/BackButton");
        resolutionButton = GetNode<OptionButton>("PanelContainer/Panel/TabContainer/Video/MarginContainer/VBoxContainer/Resolution/ResOption");
        screenSizeButton = GetNode<OptionButton>("PanelContainer/Panel/TabContainer/Video/MarginContainer/VBoxContainer/ScreenSize/SizeOption");
        vSyncCheckBox = GetNode<CheckBox>("PanelContainer/Panel/TabContainer/Video/MarginContainer/VBoxContainer/VSync/VSyncEnabled");
        fpsButton = GetNode<OptionButton>("PanelContainer/Panel/TabContainer/Video/MarginContainer/VBoxContainer/FPS/FPSOption");
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
            fpsButton.AddItem(item == 0 ? "Unlimited" : item.ToString());
        }
        previousResolutionIndex = Xalkomak.currentResIndex;
        previousScreenSizeIndex = Xalkomak.currentScreenIndex;
        resolutionButton.Selected = previousResolutionIndex;
        screenSizeButton.Selected = previousScreenSizeIndex;
        vSyncCheckBox.SetPressedNoSignal(Xalkomak.vSyncEnabled);
        fpsButton.Selected = Xalkomak.fpsIndex;
        applyButton.Disabled = true;
    }

    private void OnTabContainerTabChanged(int tab)
    {
        Xalkomak.currentTabIndex = tab;
    }

    private void OnBgmSliderValueChanged(float value)
    {
        EmitSignal(SignalName.SettingsValueChanged, bgmSlider.Name, value);
    }

    private void OnSfxSliderValueChanged(float value)
    {
        EmitSignal(SignalName.SettingsValueChanged, sfxSlider.Name, value);
    }

    private void OnResOptionItemSelected(int index)
    {
        //DisplayServer.WindowSetSize(GetValues()[index]);
        //GD.Print(GetValues()[index]);
        applyButton.Disabled = false;
        Xalkomak.currentResIndex = index;
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
        Xalkomak.currentScreenIndex = index;
        applyButton.Disabled = false;
    }

    private void OnApplyVideoSettingsPressed()
    {
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
        DisplayServer.WindowSetSize(GetValues()[Xalkomak.currentResIndex]);
        EmitSignal(SignalName.SettingsValueChanged, resolutionButton.Name, Xalkomak.currentResIndex);
        EmitSignal(SignalName.SettingsValueChanged, screenSizeButton.Name, Xalkomak.currentScreenIndex);
        applyButton.Disabled = true;
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

    private void OnVSyncEnabledToggled(bool buttonPressed)
    {
        Xalkomak.vSyncEnabled = buttonPressed;
        DisplayServer.WindowSetVsyncMode(buttonPressed ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        GD.Print(DisplayServer.WindowGetVsyncMode());
        EmitSignal(SignalName.SettingsValueChanged, vSyncCheckBox.Name, buttonPressed);
    }

    private void OnFpsOptionItemSelected(int index)
    {
        Xalkomak.gameFrameRate = fpsButton.GetItemText(index).Equals("Unlimited") ? 0 : int.Parse(fpsButton.GetItemText(index));
        GD.Print(Xalkomak.gameFrameRate);
        Xalkomak.fpsIndex = index;
        Engine.MaxFps = Xalkomak.gameFrameRate;
        EmitSignal(SignalName.SettingsValueChanged, fpsButton.Name, Xalkomak.gameFrameRate);
        EmitSignal(SignalName.SettingsValueChanged, "FPS Index", Xalkomak.fpsIndex);
    }
}
