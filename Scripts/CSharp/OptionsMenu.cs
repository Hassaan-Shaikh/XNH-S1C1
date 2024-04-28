using Godot;
using System.Collections.Generic;

public partial class OptionsMenu : Control
{
    [Signal] public delegate void SettingsValueChangedEventHandler(string optionName, Variant value);

    public AnimationPlayer animPlayer;
    public TabContainer tabContainer;
    public Button backButton;

    [Export] public VolumeSlider bgmSlider;
    [Export] public VolumeSlider sfxSlider;

    public OptionButton resolutionButton;
    public OptionButton screenSizeButton;
    public Button applyButton;

    public static Godot.Collections.Dictionary<string, Vector2I> screenResolutions = new Godot.Collections.Dictionary<string, Vector2I>
    {
        { "1280 x 720", new Vector2I(1280, 720) },
        { "1366 x 768", new Vector2I(1366, 768) },
        { "1440 x 810", new Vector2I(1440, 810) },
        { "1600 x 900", new Vector2I(1600, 900) },
        { "1920 x 1080", new Vector2I(1920, 1080) }
    };
    List<string> screenSizes = new List<string>
    {
        "Fullscreen",
        "Windowed",
        "Borderless Windowed",
        "Borderless Fullscreen",
        "Exclusive Fullscreen"
    };

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
        foreach (var item in screenResolutions)
        {
            resolutionButton.AddItem(item.Key);
        }
        foreach (string item in screenSizes)
        {
            screenSizeButton.AddItem(item);
        }
        resolutionButton.Selected = Xalkomak.currentResIndex;
        screenSizeButton.Selected = Xalkomak.currentScreenIndex;
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
            case 3: // Borderless Fullscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            case 4: // Exclusive Fullscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
        }
        DisplayServer.WindowSetSize(GetValues()[Xalkomak.currentResIndex]);
        EmitSignal(SignalName.SettingsValueChanged, resolutionButton.Name, Xalkomak.currentResIndex);
        EmitSignal(SignalName.SettingsValueChanged, screenSizeButton.Name, Xalkomak.currentScreenIndex);
        applyButton.Disabled = true;
    }
}
