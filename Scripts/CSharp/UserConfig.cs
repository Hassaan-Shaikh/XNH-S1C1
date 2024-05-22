using Godot;
using Godot.Collections;
using System;

public partial class UserConfig : Node
{
    public static ConfigFile config = new ConfigFile();
    const string CONFIG_PATH = "user://UserSettings.cfg";

    public override void _Ready()
    {
        base._Ready();
        if (!FileAccess.FileExists(CONFIG_PATH))
        {
            config.SetValue("audio", "music_volume", 0.75f);
            config.SetValue("audio", "sound_volume", 1.0f);

            config.SetValue("video", "resolution_index", 4);
            config.SetValue("video", "screen_index", 0);
            config.SetValue("video", "frames_per_second", 60);
            config.SetValue("video", "fps_index", 1);

            config.SetValue("controls", "mouse_sensitivity", 4.0f);

            config.Save(CONFIG_PATH);
        }
        else
        {
            config.Load(CONFIG_PATH);
        }
    }

    public static void SaveAudioSettings(string key, Variant value)
    {
        config.SetValue("audio", key, value);
        config.Save(CONFIG_PATH);
    }

    public static Dictionary<string, Variant> GetAudioSettings()
    {
        Dictionary<string, Variant> audioSettings = new Dictionary<string, Variant>();
        foreach (string key in config.GetSectionKeys("audio"))
        {
            audioSettings[key] = config.GetValue("audio", key);
        }
        GD.Print(audioSettings);
        return audioSettings;
    }

    public static void SaveVideoSettings(string key, Variant value)
    {
        config.SetValue("video", key, value);
        config.Save(CONFIG_PATH);
    }

    public static Dictionary<string, Variant> GetVideoSettings()
    {
        Dictionary<string, Variant> videoSettings = new Dictionary<string, Variant>();
        foreach (string key in config.GetSectionKeys("video"))
        {
            videoSettings[key] = config.GetValue("video", key);
        }
        GD.Print(videoSettings);
        return videoSettings;
    }

    public static void SaveControlSettings(string key, Variant value)
    {
        config.SetValue("controls", key, value);
    }

    public static Dictionary<string, Variant> GetControlSettings()
    {
        Dictionary<string, Variant> controlSettings = new Dictionary<string, Variant>();
        foreach (string key in config.GetSectionKeys("controls"))
        {
            controlSettings[key] = config.GetValue("controls", key);
        }
        GD.Print(controlSettings);
        return controlSettings;
    }
}
