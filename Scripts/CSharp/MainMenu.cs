using Godot;
using System;
using Godot.Collections;

public partial class MainMenu : Control
{
    [Export] public LevelLoader levelLoader;
    [Export] public PackedScene difficultyMenu;
    [Export] public PackedScene confirmQuit;
    [Export] public PackedScene optionsMenu;

    UserPrefs userPrefs;

    private Button extrasButton;

    private string selectedDifficulty;
    private bool confirmingQuit;

    public const string userGameDataPath = "user://XNH_Cellar_save.dat";
    const string gamePath = "res://Scenes/Game.tscn";

    public override void _Ready()
    {
        base._Ready();
        selectedDifficulty = "";
        if (FileAccess.FileExists(userGameDataPath))
        {
            GD.Print("It exists!");
            LoadGameData();
        }
        else
        {
            GD.Print("File does not exist.");
            SaveGameData();
        }
        userPrefs = UserPrefs.LoadOrCreate();
        Xalkomak.currentResIndex = userPrefs.resolutionIndex;
        Xalkomak.currentScreenIndex = userPrefs.screenSizeIndex;
        Xalkomak.vSyncEnabled = userPrefs.vSyncEnabled;
        Xalkomak.gameFrameRate = userPrefs.gameFps;
        Xalkomak.fpsIndex = userPrefs.fpsIndex;
        levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
        levelLoader.Visible = true;
        GetTree().Paused = false;
        extrasButton = GetNode<Button>("MarginContainer/VBoxContainer/ExtrasButton");
        extrasButton.Disabled = Xalkomak.totalGamesWon == 0;
        switch (Xalkomak.currentScreenIndex)
        {
            case 0: // Fulscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 1: // Windowed
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 2: // Borderless Windowed
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            case 3: // Exclusive Fullscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
        }
        DisplayServer.WindowSetSize(OptionsMenu.GetValues()[Xalkomak.currentResIndex]);
        DisplayServer.WindowSetVsyncMode(userPrefs.vSyncEnabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(userPrefs.soundAudioLevel));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), Mathf.LinearToDb(userPrefs.musicAudioLevel));
        Engine.MaxFps = Xalkomak.gameFrameRate;
    }

    public void SaveGameData()
    {
        var file = FileAccess.OpenEncryptedWithPass(userGameDataPath, FileAccess.ModeFlags.Write, "xalkomak");
        Dictionary<string, string> userData = PlayerData();
        file.StoreVar(userData);
    }

    public Dictionary<string, string> PlayerData()
    {
        Dictionary<string, string> userDict = new Dictionary<string, string>();
        userDict.Add("TotalWinCount", Xalkomak.totalGamesWon.ToString());
        userDict.Add("TotalLossCount", Xalkomak.totalGamesLost.ToString());
        userDict.Add("NormalModeWins", Xalkomak.gamesWonOnNormalMode.ToString());
        userDict.Add("HardModeWins", Xalkomak.gamesWonOnHardMode.ToString());
        userDict.Add("LivesLost", Xalkomak.livesLost.ToString());
        userDict.Add("NormalModeWon", Xalkomak.gameCompletedOnNormalMode.ToString());
        userDict.Add("HardModeWon", Xalkomak.gameCompletedOnHardMode.ToString());
        GD.Print(userDict);
        return userDict;
    }

    public void LoadGameData()
    {
        var file = FileAccess.OpenEncryptedWithPass(userGameDataPath, FileAccess.ModeFlags.Read, "xalkomak");
        if (file != null)
        {
            Dictionary<string, string> loadedData = file.GetVar().AsGodotDictionary<string, string>();
            int totalWinCount = int.Parse(loadedData["TotalWinCount"]);
            int totalLossCount = int.Parse(loadedData["TotalLossCount"]);
            int normalModeWins = int.Parse(loadedData["NormalModeWins"]);
            int hardModeWins = int.Parse(loadedData["HardModeWins"]);
            int livesLost = int.Parse(loadedData["LivesLost"]);
            bool normalModeWon = loadedData["NormalModeWon"] == "True";
            bool hardModeWon = loadedData["HardModeWon"] == "True";
            Xalkomak.totalGamesWon = totalWinCount;
            Xalkomak.totalGamesLost = totalLossCount;
            Xalkomak.gamesWonOnNormalMode = normalModeWins;
            Xalkomak.gamesWonOnHardMode = hardModeWins;
            Xalkomak.livesLost = livesLost;
            Xalkomak.gameCompletedOnNormalMode = normalModeWon;
            Xalkomak.gameCompletedOnHardMode = hardModeWon;
            GD.Print(Xalkomak.gameCompletedOnNormalMode);
            GD.Print(loadedData);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        //if (Input.IsActionJustPressed("attemptSave"))
        //{
        //    SaveGameData();
        //}
    }

    private void OnPlayButtonPressed()
    {
        if (Xalkomak.totalGamesWon >= 1)
        {
            DifficultySelectionMenu menu = difficultyMenu.Instantiate() as DifficultySelectionMenu;
            AddChild(menu);
            menu.DifficultySelected += GetDifficulty;
            menu.TreeExiting += () => 
            {
                menu.DifficultySelected -= GetDifficulty;
            };
            ProcessMode = ProcessModeEnum.Disabled;
            menu.popUpAnim.AnimationFinished += (StringName animName) =>
            {
                if(animName.Equals("PopIn"))
                {
                    ProcessMode = ProcessModeEnum.Always;
                }
                if (animName.Equals("PopOut"))
                {
                    if (!selectedDifficulty.Equals(""))
                    {
                        Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Hard ? 1 : 3;
                        Xalkomak.playerCanControl = true;
                        Xalkomak.documentsCollected = 0;
                        for (int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
                        {
                            Xalkomak.isDocumentCollected[i] = false;
                        }
                        levelLoader.SwitchScene(gamePath);
                    }
                    menu.QueueFree();
                }
            };
        }
        else
        {
            Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Hard ? 1 : 3;
            Xalkomak.playerCanControl = true;
            Xalkomak.documentsCollected = 0;
            for (int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
            {
                Xalkomak.isDocumentCollected[i] = false;
            }
            levelLoader.SwitchScene(gamePath);
        }
    }

    private void OnOptionsButtonPressed()
    {
        OptionsMenu opMenu = optionsMenu.Instantiate<OptionsMenu>();
        AddChild(opMenu);
        ProcessMode = ProcessModeEnum.Disabled;
        opMenu.TreeExiting += () => 
        {
            opMenu.SettingsValueChanged -= OnOptionMenuSettingChanged;
            //GD.Print(opMenu.IsConnected(OptionsMenu.SignalName.SettingsValueChanged, Ma));
        };
        opMenu.SettingsValueChanged += OnOptionMenuSettingChanged;
        opMenu.animPlayer.AnimationFinished += (StringName animName) => 
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.Always;
            }
            if (animName.Equals("PopOut"))
                opMenu.QueueFree();
        };
    }

    private void OnExtrasButtonPressed()
    {

    }

    private void OnCreditsButtonPressed()
    {

    }

    private void OnQuitButtonPressed()
    {
        ConfirmQuit quitBox = confirmQuit.Instantiate() as ConfirmQuit;
        AddChild(quitBox);
        quitBox.SetQuitMessage("[center]Confirm to exit the game[/center]");
        quitBox.QuitConfirmed += (bool confirmedQuit) =>
        {
            confirmingQuit = confirmedQuit;
        };
        ProcessMode = ProcessModeEnum.Disabled;
        quitBox.popAnim.AnimationFinished += (StringName animName) =>
        {
            if (animName.Equals("PopIn"))
            {
                ProcessMode = ProcessModeEnum.Always;
            }
            if (animName.Equals("PopOut"))
            {      
                if (confirmingQuit)
                    GetTree().Quit();
                quitBox.QueueFree();
            }
        };
    }

    private void GetDifficulty(StringName difficulty)
    {
        selectedDifficulty = difficulty;
        switch (selectedDifficulty)
        {
            case "Normal":
                Xalkomak.difficulty = Xalkomak.Difficulty.Normal;
                GD.Print("Started a game on " + Xalkomak.difficulty + " difficulty.");
                break;
            case "Hard":
                Xalkomak.difficulty = Xalkomak.Difficulty.Hard;
                GD.Print("Started a game on " + Xalkomak.difficulty + " difficulty.");
                break;
            default:
                selectedDifficulty = "";
                GD.Print("No difficulty selected/Backed out.");
                break;
        }
    }

    private void OnOptionMenuSettingChanged(string settingName, Variant settingValue)
    {
        switch (settingName)
        {
            case "BGMSlider":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.musicAudioLevel = (float)settingValue;
                userPrefs.SavePrefs();
                break;
            case "SFXSlider":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.soundAudioLevel = (float)settingValue;
                userPrefs.SavePrefs();
                break;
            case "ResOption":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.resolutionIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "SizeOption":
                //GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.screenSizeIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "VSyncEnabled":
                GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.vSyncEnabled = (bool)settingValue;
                userPrefs.SavePrefs();
                break;
            case "FPSOption":
                GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
                userPrefs.gameFps = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            case "FPS Index":
                userPrefs.fpsIndex = (int)settingValue;
                userPrefs.SavePrefs();
                break;
            default:
                GD.PrintErr("Setting node", settingName, " is unavailable or not defined.");
                break;
        }
    }
}
