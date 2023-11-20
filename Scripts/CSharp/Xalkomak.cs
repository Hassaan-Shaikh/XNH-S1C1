using Godot;
using System;
using System.Collections.Generic;

public static class Xalkomak
{
    public static int livesRemaining = 3;
    
    public static int documentsCollected = 0;

    public static bool playerCanControl = true;

    public static bool isGuardianCollected = false;
    public static bool isSpeedBoostCollected = false;
    public static bool isStunCollected = false;
    public static bool isVanishCollected = false;

    public static bool isGuardianCollectedBySammy = false;
    public static bool isSpeedBoostCollectedBySammy = false;
    public static bool isStunCollectedBySammy = false;
    public static bool isVanishCollectedBySammy = false;

    public enum Difficulty
    {
        Normal,
        Hard
    }
    public static Difficulty difficulty = Difficulty.Hard;

    public static bool[] isThisSpotOccupied;
    public static bool[] isDocumentCollected = new bool[8];
}
