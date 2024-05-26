using Godot;
using System;

public partial class ChainPath : Path3D
{
	[Export] public float distanceBetweenLinks = 0.5f;
    [Export] MultiMeshInstance3D mmI;

	private bool isDirty = false;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (isDirty)
        {
            UpdateMultimesh();
            isDirty = false;
        }
    }

    private void UpdateMultimesh()
    {
        float pathLength = Curve.GetBakedLength();
        int count = (int)Mathf.Floor(pathLength / distanceBetweenLinks);

        MultiMesh mm = mmI.Multimesh;
        mm.InstanceCount = count;
        float offset = distanceBetweenLinks / 2;

        for (int i = 0; i <= count; i++)
        {
            float curveDistance = offset + distanceBetweenLinks * i;
            Vector3 pos = Curve.SampleBaked(curveDistance, true);

            Basis basis = Basis;
            Transform3D t3d = new Transform3D(basis, pos);
            mm.SetInstanceTransform(i, t3d);
        }
    }

    private void OnCurveChanged()
    {
        isDirty = true;
    }
}
