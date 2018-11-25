using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class MainMenu : MonoBehaviour {

		// string gameVersion = "1";

		[SerializeField]
		private GameObject mainPanel;
		[SerializeField]
		private GameObject optionsPanel;


		void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
		}

		public void ToggleOptionsMenu() {
			if (optionsPanel.activeInHierarchy) {
				optionsPanel.SetActive(false);
				mainPanel.SetActive(true);
			} else {
				optionsPanel.SetActive(true);
				mainPanel.SetActive(false);
			}
		}

		public void LoadScene(string sceneName) {
			SceneManager.LoadScene(sceneName);
		}

		public void ExitGame() {
			Application.Quit();
		}
	}
}
