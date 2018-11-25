using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.RyanKoning.SeaOfStars {
	public class BasicPlayerSpawner : MonoBehaviourPunCallbacks {

		public GameObject playerPrefab;
		private Transform[] spawnPoints;

		public float respawnTime;
		public static BasicPlayerSpawner Instance;

		void Awake() {
			Instance = this;

			object _gameMode;
			Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, out _gameMode));
			Debug.Log((string) _gameMode);
			// fill spawnPoints array and disable markers
			spawnPoints = new Transform[transform.childCount];
			for(int i = 0; i < transform.childCount; i++) {
				spawnPoints[i] = transform.GetChild(i);
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		void Start() {
			if (playerPrefab != null) {
				if (PlayerController.LocalPlayerInstance == null) {
					Transform spawnPoint = GetRandomSpawnPoint();
					PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
				} else {

				}
			}
		}

		public Transform GetRandomSpawnPoint() {
			return spawnPoints[Random.Range(0,spawnPoints.Length - 1)];
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
