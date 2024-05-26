using Godot;
using System;

[Tool]
public partial class ChainPath : Path3D
{
    [Export] public float distanceBetweenLinks
    {
        set
        {
            distanceBetweenLinks = value;
            SpawnLink();
        }
        get
        {
            return distanceBetweenLinks;
        }
    }
    [Export] PackedScene chainLink;

    private void SpawnLink()
    {
        float pathLength = Curve.GetBakedLength();
        int count = (int)Mathf.Floor(pathLength / distanceBetweenLinks);
        float offset = distanceBetweenLinks / 2;

        foreach (Node child in GetChildren())
        {
            child.Free();
        }
        
        for (int i = 0; i < count; i++)
        {
            float curveDistance = offset + distanceBetweenLinks * i;
            Vector3 pos = Curve.SampleBaked(curveDistance, true);
            Vector3 forward = pos.DirectionTo(Curve.SampleBaked(curveDistance + 0.1f, true));
            Vector3 upVector = Curve.SampleBakedUpVector(curveDistance, true);

            if (i % 2 == 0)
                upVector = upVector.Rotated(forward, Mathf.DegToRad(90));

            Node3D link = chainLink.Instantiate<Node3D>();
            AddChild(link);
            link.Position = pos;

            var bas = Basis.Identity;
            bas.Y = upVector;
            bas.Z = -forward;
            bas.X = forward.Cross(upVector).Normalized();
            link.Basis = bas;
        }
    }

    private void OnCurveChanged()
    {
        SpawnLink();
    }
}
