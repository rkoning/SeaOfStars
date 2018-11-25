using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {

	public enum GameMode {
		Deathmatch,
		Race
	}

	public class SeaOfStarsGame {
		public const float PLAYER_RESPAWN_TIME = 3.0F;
		public const int PLAYER_MAX_LIVES = 3;

		public const string PLAYER_LIVES = "PlayerLives";
		public const string PLAYER_READY = "IsPlayerReady";
		public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
		public const string PLAYER_SCORE = "PlayerScore";
		public const string PLAYER_SELECTED_SHIP = "PlayerSelectedShip";
		public const string PLAYER_SELECTED_PRIMARY_WEAPON = "PlayerSelectedPrimaryWeapon";
		public const string PLAYER_SELECTED_SECONDARY_WEAPON = "PlayerSelectedSecondary";

		public const string ROOM_SELECTED_STAGE = "RoomSelectedStage";
		public const string ROOM_SELECTED_GAME_MODE = "RoomSelectedGameMode";

		public const string DEATHMATCH_SCORE_TO_WIN = "DeathmatchScoreToWin";


		public static Color GetColor(int colorChoice)
		{
				switch (colorChoice)
				{
						case 0: return Color.red;
						case 1: return Color.green;
						case 2: return Color.blue;
						case 3: return Color.yellow;
						case 4: return Color.cyan;
						case 5: return Color.grey;
						case 6: return Color.magenta;
						case 7: return Color.white;
				}

				return Color.black;
		}
	}
}
