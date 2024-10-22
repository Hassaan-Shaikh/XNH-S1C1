using Godot;
using System;
using Godot.Collections;

public partial class MainMenu : Control
{
	[Export] public LevelLoader levelLoader;
	[Export] public PackedScene difficultyMenu, confirmQuit, optionsMenu;

	UserPrefs userPrefs;

	private Button extrasButton;

	private string selectedDifficulty;
	private bool confirmingQuit;

	const string gamePath = "res://Scenes/Game.tscn";

	public override void _Ready()
	{
		base._Ready();

		Dictionary<string, Variant> audioSettings = UserConfig.GetAudioSettings();
		Dictionary<string, Variant> videoSettings = UserConfig.GetVideoSettings();
		Dictionary<string, Variant> controlSettings = UserConfig.GetControlSettings();

		selectedDifficulty = "";
		if (FileAccess.FileExists(Xalkomak.userGameDataPath))
		{
			LoadGameData();
		}
		else
		{
			SaveGameData();
		}
		/*userPrefs = UserPrefs.LoadOrCreate();
		userPrefs.SavePrefs();
		Xalkomak.currentResIndex = userPrefs.resolutionIndex;
		Xalkomak.currentScreenIndex = userPrefs.screenSizeIndex;
		Xalkomak.gameFrameRate = userPrefs.gameFps;
		Xalkomak.fpsIndex = userPrefs.fpsIndex;
		Xalkomak.camSens = userPrefs.sensitivityLevel;*/
		levelLoader = GetTree().GetNodesInGroup("LevelLoader")[0] as LevelLoader;
		levelLoader.Visible = true;
		GetTree().Paused = false;
		extrasButton = GetNode<Button>("MarginContainer/VBoxContainer/ExtrasButton");
		extrasButton.Disabled = Xalkomak.totalGamesWon == 0;
		switch ((int)videoSettings["screen_index"])
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
		DisplayServer.WindowSetSize(OptionsMenu.GetValues()[(int)videoSettings["resolution_index"]]);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb((float)audioSettings["sound_volume"]));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), Mathf.LinearToDb((float)audioSettings["music_volume"]));
		Engine.MaxFps = (int)videoSettings["frames_per_second"];
		Xalkomak.camSens = (float)controlSettings["mouse_sensitivity"];
	}

	public void SaveGameData()
	{
		var file = FileAccess.OpenEncryptedWithPass(Xalkomak.userGameDataPath, FileAccess.ModeFlags.Write, "xalkomak");
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
		var file = FileAccess.OpenEncryptedWithPass(Xalkomak.userGameDataPath, FileAccess.ModeFlags.Read, "xalkomak");
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
			case "FPSOption":
				GD.Print("Incoming setting change from\nName: ", settingName, "\nNew Value: ", settingValue, "\n");
				userPrefs.gameFps = (int)settingValue;
				userPrefs.SavePrefs();
				break;
			case "FPS Index":
				userPrefs.fpsIndex = (int)settingValue;
				userPrefs.SavePrefs();
				break;
			case "SenSlider":
				userPrefs.sensitivityLevel = (float)settingValue;
				userPrefs.SavePrefs();
				break;
			default:
				GD.PrintErr("Setting node ", settingName, " is unavailable or not defined.");
				break;
		}
	}
}
