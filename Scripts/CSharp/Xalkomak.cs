using Godot;
using System;
using System.Collections.Generic;

public static class Xalkomak
{
    public static int livesRemaining = 3;
    public static int livesLost = 0; //

    public static bool gameCompletedOnNormalMode = false; //
    public static bool gameCompletedOnHardMode = false; //
    public static bool unlockExtras = false;

    public static int gamesWonOnNormalMode = 0; //
    public static int gamesWonOnHardMode = 0; //
    public static int totalGamesWon = 2; // Default: 0
    public static int totalGamesLost = 0; //

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
    public static Difficulty difficulty = Difficulty.Normal;

    public static bool[] isThisSpotOccupied;
    public static bool[] isDocumentCollected = new bool[8];

    public static string gamePath = "res://Scenes/Game.tscn";
    public static string deathScreenPath = "res://Scenes/DeathScreen.tscn";
    public static string mainMenuPath = "res://Scenes/MainMenu.tscn";

    public static int currentTabIndex = 0;
    public static int currentResIndex = 4;
    public static int currentScreenIndex = 0;
}
