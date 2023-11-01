using Godot;
using System;

public static class Xalkomak
{
    public static float DegToRad(float degrees)
    {
        float radians = degrees * (Mathf.Pi / 180);
        return radians;
    }

    public static float RadToDeg(float radians)
    {
        float degress = radians * (180 / Mathf.Pi);
        return degress;
    }
}
