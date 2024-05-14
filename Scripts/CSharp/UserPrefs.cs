using Godot;
using System;

[GlobalClass]
public partial class UserPrefs : Resource
{
    [Export(PropertyHint.Range, "0, 1")] public float musicAudioLevel = 0.75f;
    [Export(PropertyHint.Range, "0, 1")] public float soundAudioLevel = 1.0f;
    [Export] public int resolutionIndex = 4;
    [Export] public int screenSizeIndex = 0;
    [Export] public int gameFps = 60;
    [Export] public int fpsIndex = 1;

    const string userPrefsSavePath = "user://UserPrefs.tres";
    
    public void SavePrefs()
    {
        ResourceSaver.Save(this, userPrefsSavePath);
        GD.Print("Saved the file at: ", userPrefsSavePath);
    }

    public static UserPrefs LoadOrCreate()
    {
        UserPrefs resource = ResourceLoader.Load<UserPrefs>(userPrefsSavePath);
        if (!IsInstanceValid(resource) || resource == null)
        {
            resource = new UserPrefs();
            GD.Print("Created a new user preferences data.");
        }
        GD.Print("Loaded/Created a user preferences file.");
        return resource;
    }
}
