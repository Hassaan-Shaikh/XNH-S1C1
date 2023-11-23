using Godot;
using System;
using Godot.Collections;

public partial class DeathScreen : Control
{
    [Export] LevelLoader levelLoader;
    [Export] Label livesRemaining;
    [Export] Label gameOver;
    [Export] Button retryButton;
    [Export] Button quitButton;
    [Export] AnimationPlayer deathAnim;

    int counter = 0;

    public const string userGameDataPath = "user://save.dat";
    const string gamePath = "res://Scenes/Game.tscn";
    const string mainMenuPath = "res://Scenes/MainMenu.tscn";

    public override void _Ready()
    {
        base._Ready();
        //LoadGameData();
        levelLoader.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        livesRemaining.Visible = Xalkomak.livesRemaining > 0;
        gameOver.Visible = Xalkomak.livesRemaining == 0;
        retryButton.Visible = Xalkomak.livesRemaining == 0;
        quitButton.Visible = Xalkomak.livesRemaining == 0;

        if (Xalkomak.livesRemaining > 0)
        {
            Xalkomak.livesLost++;
            deathAnim.Play("LivesRemaining");
        }
        else
        {
            deathAnim.Play("GameOver");
            Xalkomak.totalGamesLost += 1;
            Xalkomak.livesRemaining = Xalkomak.difficulty == Xalkomak.Difficulty.Normal ? 3 : 1;
            Xalkomak.playerCanControl = true;
            Xalkomak.documentsCollected = 0;
            for (int i = 0; i < Xalkomak.isDocumentCollected.Length; i++)
            {
                Xalkomak.isDocumentCollected[i] = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        livesRemaining.Text = "Lives Remaining: " + Xalkomak.livesRemaining;
    }

    public void UpdateLifeCounter()
    {
        Xalkomak.livesRemaining -= 1;
        Xalkomak.playerCanControl = true;
    }

    private void OnDeathAnimAnimationFinished(string animName)
    {
        if(animName == "LivesRemaining")
        {
            levelLoader.SwitchScene(gamePath);
        }
    }

    public void OnRetryButtonPressed()
    {        
        levelLoader.SwitchScene(gamePath);
    }

    public void OnQuitButtonPressed()
    {
        //GetTree().Quit();
        levelLoader.SwitchScene(mainMenuPath);
    }

    public void ReleaseMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
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
}
