using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Cegorach.SeaOfStars {
	public class Announcer : MonoBehaviour {
		public static Announcer Instance;

		[SerializeField]
		private Text noticeText;

		[SerializeField]
		private GameObject scoreBoard;
		[SerializeField]
		private float scoreBoardRowHeight;
		[SerializeField]
		private GameObject playerRowPrefab;
		private GameObject[] rows;

		private Text[] names;
		private Text[] scores;

		[SerializeField]
		private Color startColor;
		[SerializeField]
		private Color endColor;

		void Awake() {
			Instance = this;
		}

		void Start() {
			rows = new GameObject[GameLogic.Instance.players.Count];
			int i = 0;
			foreach(KeyValuePair<int, PlayerInfo> kvp in GameLogic.Instance.players) {
				rows[i] = Instantiate(playerRowPrefab, new Vector3(0, -scoreBoardRowHeight * i - 5, 0), Quaternion.identity);
				rows[i].transform.SetParent(scoreBoard.transform, false);
				rows[i].transform.GetChild(0).GetComponentsInChildren<Text>()[0].text = kvp.Value.name;
				rows[i].transform.GetChild(0).GetComponentsInChildren<Text>()[1].text = kvp.Value.ToString();
				i++;
			}
			scoreBoard.SetActive(false);
		}

		public void ShowNotice(string notice) {
			noticeText.gameObject.SetActive(true);
			noticeText.text = notice;
			StartCoroutine(FadeOut(3f));
		}

		IEnumerator FadeOut(float duration) {
			for (float now = 0f; now < duration; now += Time.deltaTime) {
				float normalizedTime = now / duration;
				noticeText.color = Color.Lerp(startColor, endColor, normalizedTime);
				yield return null;
			}
			noticeText.color = endColor;
			noticeText.gameObject.SetActive(false);
		}

		public void UpdateScoreBoard(Dictionary<int, PlayerInfo> scores) {
			int i = 0;
			foreach(KeyValuePair<int, PlayerInfo> kvp in scores) {
				rows[i].transform.GetChild(0).GetComponentsInChildren<Text>()[0].text = kvp.Value.name;
				rows[i].transform.GetChild(0).GetComponentsInChildren<Text>()[1].text = kvp.Value.score.ToString();
				i++;
			}
			// remove any rows that belong to players not in the game anymore
			while(i < rows.Length) {
				Destroy(scoreBoard.transform.GetChild(i).gameObject);
				i++;
			}
		}

		public void ShowScoreBoard() {
			scoreBoard.SetActive(true);
		}

		public void HideScoreBoard() {
			scoreBoard.SetActive(false);
		}

		public void ToggleScoreBoard() {
			if (scoreBoard.activeInHierarchy) {
				scoreBoard.SetActive(false);
			} else {
				scoreBoard.SetActive(true);
			}
		}
	}
}
