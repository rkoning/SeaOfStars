using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.Cegorach.SeaOfStars {

	public struct PlayerScore {
		public string name;
		public int score;
	}

	public class PostGameLobby : MonoBehaviourPunCallbacks {

		[SerializeField]
		private GameObject scoreBoard;
		[SerializeField]
		private GameObject playerRowPrefab;

		void Start() {
			PlayerScore[] scores = new PlayerScore[PhotonNetwork.PlayerList.Length];

			int i = 0;
			foreach (Player p in PhotonNetwork.PlayerList) {
				object score;
				if (p.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SCORE, out score)) {
				scores[i].score = (int) score;
				}
				scores[i].name = p.NickName;
				i++;
			}

			Array.Sort<PlayerScore>(scores, (x,y) => x.score.CompareTo(y.score));

			for (i = 0; i < scores.Length; i++) {
				var pr = Instantiate(playerRowPrefab, new Vector3(5, -60 * i, 0), Quaternion.identity);
				pr.transform.GetChild(0).GetComponentsInChildren<Text>()[0].text = scores[i].name;
				pr.transform.GetChild(0).GetComponentsInChildren<Text>()[1].text = scores[i].score.ToString();
				pr.transform.SetParent(scoreBoard.transform, false);
			}
		}

		public void LeaveRoom() {
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.LeaveRoom();
				PhotonNetwork.LoadLevel("Scenes/MainMenu");
			} else {
				SceneManager.LoadScene("Scenes/MainMenu");
			}
		}
	}
}
