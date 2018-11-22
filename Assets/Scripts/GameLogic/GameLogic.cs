using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.Cegorach.SeaOfStars {

	public class PlayerInfo {
		public string name;
		public int id;
		public int score;
		public Vector3 spawnPosition;
		public Quaternion spawnRotation;

		public PlayerInfo(string name, int id, int score, Transform spawnPoint) {
			this.name = name;
			this.id = id;
			this.score = score;
			if (spawnPoint != null){
				this.spawnPosition = spawnPoint.position;
				this.spawnRotation = spawnPoint.rotation;
			}
		}

		public void AddScore(int points) {
			this.score += points;
		}

		public void SetSpawnPoint(Transform spawnPoint) {
			this.spawnPosition = spawnPoint.position;
			this.spawnRotation = spawnPoint.rotation;
		}

		public void SetSpawnPoint(Vector3 spawnPosition, Quaternion spawnRotation) {
			Debug.Log(id + " " + spawnPosition + " " + spawnRotation);
			this.spawnPosition = spawnPosition;
			this.spawnRotation = spawnRotation;
		}
	}

	public abstract class GameLogic : MonoBehaviourPunCallbacks {

		public GameObject playerPrefab;
		protected Transform[] spawnPoints;

		public float respawnTime;
		public static GameLogic Instance;
		// public Dictionary<string, int> playerScores;
		public Dictionary<int, PlayerInfo> players;

		public float endGameWait = 5f;

		public string postGameScenePath = "Scenes/PostgameLobby";

		protected virtual void Awake() {
			// Initialize player scores Dictionary to 0 for all players in the room
			players = new Dictionary<int, PlayerInfo>();
			if (PhotonNetwork.IsConnected) {
				foreach (Player p in PhotonNetwork.PlayerList) {
					players[p.ActorNumber] = new PlayerInfo(p.NickName, p.ActorNumber, 0, null);
				}
			} else {
				players[0] = new PlayerInfo("Player 1", 0, 0, null);
			}
			Instance = this;
		}

		//
		// Custom logic to spawn the players at the start of the game.
		//
		public abstract void SpawnPlayers();

		//
		// Called by player health when a player dies.
		//
		public abstract void OnPlayerDeath(int playerID, int enemyID);

		//
		// Takes a players actorNumber as an arguement,
		// Returns the transform of a spawn point, this will be different for each game mode
		//
		public abstract object[] GetSpawnPoint(int actorNumber);

		//
		// Called when a player scores.
		//
		public abstract void Score(int playerID, int points);

		//
		// Takes a duration in seconds as an arguement and loads the post-game scene after that duration.
		//
		protected IEnumerator WaitThenEndGame(float duration) {
			yield return new WaitForSeconds(duration);
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.LoadLevel(postGameScenePath);
			} else {
				SceneManager.LoadScene(postGameScenePath);
			}
		}

		//
		// Writes scores to photon player properties to be used in post-game scene
		//
		protected void SettleMatch() {
			if (PhotonNetwork.IsMasterClient) {
				foreach (Player p in PhotonNetwork.PlayerList) {
					if (players.ContainsKey(p.ActorNumber)) {
						ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable {
							{ SeaOfStarsGame.PLAYER_SCORE, players[p.ActorNumber].score }
						};
						p.SetCustomProperties(props);
					}
				}
			}
		}
	}
}
