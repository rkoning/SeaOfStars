using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Com.Cegorach.SeaOfStars {
	public class DeathmatchGameLogic : GameLogic {

		private int scoreToWin;
		private int pointsPerKill = 1;

		void Start() {
			if (PhotonNetwork.IsConnected) {
				object scoreToWin;
				PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeaOfStarsGame.DEATHMATCH_SCORE_TO_WIN, out scoreToWin);
				this.scoreToWin = (int) scoreToWin;
			}

			spawnPoints = new Transform[transform.childCount];
			for(int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).tag == "Deathmatch SpawnPoint") {
					spawnPoints[i] = transform.GetChild(i);
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			SpawnPlayers();
		}

		public override void SpawnPlayers() {
			if (playerPrefab != null) {
				if (PlayerController.LocalPlayerInstance == null) {
					var spawnPoint = GetSpawnPoint(0);
					PhotonNetwork.Instantiate(this.playerPrefab.name, (Vector3)spawnPoint[0], (Quaternion)spawnPoint[1], 0);
				}
			}
		}

		//
		// Returns a random spawn point. Ignoring actorNumber.
		//
		public override object[] GetSpawnPoint(int actorNumber) {
			var _point = spawnPoints[Random.Range(0,spawnPoints.Length - 1)];
			return new object[] {_point.position, _point.rotation};
		}

		//
		// Increments the killing players score by pointsPerKill (default 1)
		//
		public override void OnPlayerDeath(int playerID, int enemyID) {
			if (players.ContainsKey(enemyID)) {
				Score(enemyID, pointsPerKill);
			}
		}

		public override void Score(int playerID, int points) {
			players[playerID].AddScore(points);
			Announcer.Instance.UpdateScoreBoard(players);
			if (players[playerID].score >= scoreToWin) {
				Announcer.Instance.ShowNotice(players[playerID].name + " has won the match!");
				Announcer.Instance.ShowScoreBoard();
				SettleMatch();
				StartCoroutine(WaitThenEndGame(endGameWait));
			}
		}
	}
}
