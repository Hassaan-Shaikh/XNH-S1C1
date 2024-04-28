using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] string audioBusName;

    UserPrefs prefs;

	int audioBusIndex;

    public override void _Ready()
    {
        base._Ready();
        audioBusIndex = AudioServer.GetBusIndex(audioBusName);
        prefs = UserPrefs.LoadOrCreate();
        switch (audioBusIndex)
        {
            case 2:
                Value = prefs.musicAudioLevel;
                //AudioServer.SetBusVolumeDb(audioBusIndex, Mathf.DbToLinear((float)Value));
                GD.Print(audioBusIndex, " ", audioBusName, " Music volume: ", prefs.musicAudioLevel);
                break;
            case 1:
                Value = prefs.soundAudioLevel;
                //AudioServer.SetBusVolumeDb(audioBusIndex, Mathf.DbToLinear((float)Value));
                GD.Print(audioBusIndex, " ", audioBusName, " Sound volume: ", prefs.soundAudioLevel);
                break;
        }
        //Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(audioBusIndex));
        ValueChanged += (double value) =>
        {
            AudioServer.SetBusVolumeDb(audioBusIndex, Mathf.LinearToDb((float)value));
            prefs.SavePrefs();
        };
    }
}
