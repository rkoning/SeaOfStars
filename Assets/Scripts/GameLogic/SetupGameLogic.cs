using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityEngine.SceneManagement;

namespace Com.Cegorach.SeaOfStars {
	public class SetupGameLogic : MonoBehaviourPunCallbacks {

		public GameObject playerPrefab;
		public float respawnTime;

		void Awake () {
			object gameMode;
			GameLogic logic = null;
			if (PhotonNetwork.IsConnected){
				if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, out gameMode)) {
						if ((string) gameMode == GameMode.Deathmatch.ToString()) {
							logic = gameObject.AddComponent<DeathmatchGameLogic>();
						} else if ((string) gameMode == GameMode.Race.ToString()) {
							logic = gameObject.AddComponent<RaceGameLogic>();
						}

						logic.playerPrefab = playerPrefab;
						logic.respawnTime = respawnTime;
				} else {
					Debug.Log("No Game Mode selected");
					SceneManager.LoadScene(0);
				}
			} else {
				logic = gameObject.AddComponent<RaceGameLogic>();
				logic.playerPrefab = playerPrefab;
				logic.respawnTime = respawnTime;
			}
		}

		public override void OnLeftRoom() {
			SceneManager.LoadScene(0);
		}

		public void LeaveRoom() {
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.LeaveRoom();
			} else {
				SceneManager.LoadScene(0);
			}
		}
	}
}
