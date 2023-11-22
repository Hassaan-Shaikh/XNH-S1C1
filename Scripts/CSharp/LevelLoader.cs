using Godot;
using System;

public partial class LevelLoader : Control
{
    [Signal]
    public delegate void SceneSwitchedEventHandler(string scenePath);

    AnimationPlayer crossfadeAnim;
    Timer loaderDelay;

    public override void _Ready()
    {
        base._Ready();

        crossfadeAnim = GetNode<AnimationPlayer>("CrossfadeAnim");
        loaderDelay = GetNode<Timer>("LoadDelay");

        loaderDelay.WaitTime = GD.RandRange(0.3f, 0.5f);
    }
    public void SwitchScene(string scenePath)
    {
        crossfadeAnim.Play("FadeOut");
        crossfadeAnim.AnimationFinished += (StringName animName) => 
        {
            if (animName.Equals("FadeOut"))
            {
                GetTree().ChangeSceneToFile(scenePath);
                GetTree().Paused = false;
            }
        };
    }

    private void OnLoadDelayTimeout()
    {
        crossfadeAnim.Play("FadeIn");
    }
}
