using Godot;
using System;
using Godot.Collections;

public partial class MainMenu : Control
{
    [Export] public LevelLoader levelLoader;
    [Export] public PackedScene difficultyMenu;
    [Export] public PackedScene confirmQuit;

    private string selectedDifficulty;
    private bool confirmingQuit;

    public const string userGameDataPath = "user://save.dat";
    const string gamePath = "res://Scenes/Game.tscn";

    public override void _Ready()
    {
        base._Ready();
        //LoadGameData();
        levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
        levelLoader.Visible = true;
        GetTree().Paused = false;
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
        if (Input.IsActionJustPressed("attemptSave"))
        {
            SaveGameData();
        }
    }

    private void OnPlayButtonPressed()
    {
        if (Xalkomak.totalGamesWon >= 1)
        {
            DifficultySelectionMenu menu = difficultyMenu.Instantiate() as DifficultySelectionMenu;
            AddChild(menu);
            menu.DifficultySelected += GetDifficulty;
            ProcessMode = ProcessModeEnum.Disabled;
            menu.popUpAnim.AnimationFinished += (StringName animName) =>
            {
                if(animName.Equals("PopIn"))
                {
                    ProcessMode = ProcessModeEnum.Always;
                }
                if (animName.Equals("PopOut") && (selectedDifficulty.Equals("Normal") || selectedDifficulty.Equals("Hard")))
                {
                    Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Hard ? 1 : 3;
                    Xalkomak.playerCanControl = true;
                    Xalkomak.documentsCollected = 0;
                    for (int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
                    {
                        Xalkomak.isDocumentCollected[i] = false;
                    }
                    menu.QueueFree();
                    levelLoader.SwitchScene(gamePath);
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
        quitBox.quitMessage.Text = "";
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
            if (animName.Equals("PopOut") && confirmingQuit)
            {                
                quitBox.QueueFree();
                GetTree().Quit();
            }
        };
    }

    private void GetDifficulty(StringName difficulty)
    {
        selectedDifficulty = difficulty;
        switch(selectedDifficulty)
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
                GD.Print("No difficulty selected/Backed out.");
                break;
        }
    }
}