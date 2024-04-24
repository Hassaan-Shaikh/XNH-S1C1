using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] string audioBusName;

	int audioBusIndex;

    public override void _Ready()
    {
        base._Ready();
        audioBusIndex = AudioServer.GetBusIndex(audioBusName);
        Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(audioBusIndex));
        ValueChanged += (double value) =>
        {
            AudioServer.SetBusVolumeDb(audioBusIndex, Mathf.LinearToDb((float)value));
        };
    }
}
