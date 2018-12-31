using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class MainMenu : MonoBehaviour {

		// string gameVersion = "1";

		[SerializeField]
		private GameObject mainPanel;
		[SerializeField]
		private GameObject optionsPanel;
		[SerializeField]
		private GameObject loadoutEditPanel;
		[SerializeField]
		private GameObject loadoutSelectPanel;
		[SerializeField]
		private GameObject modal;
		public delegate void ModalAction(string text);
		private Text modalTitle;
		private InputField modalInput;
		private Button modalButton;
		private Text modalButtonText;


		void Awake() {
			// PhotonNetwork.AutomaticallySyncScene = true;

			modalInput = modal.GetComponentInChildren<InputField>();
			modalTitle = modal.GetComponentInChildren<Text>();
			modalButton = modal.GetComponentInChildren<Button>();
			modalButtonText = modalButton.gameObject.GetComponentInChildren<Text>();
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

		public void SetActivePanel(string activePanel)
		{
			mainPanel.SetActive(activePanel.Equals(mainPanel.name));
			optionsPanel.SetActive(activePanel.Equals(optionsPanel.name));
			loadoutEditPanel.SetActive(activePanel.Equals(loadoutEditPanel.name));
			loadoutSelectPanel.SetActive(activePanel.Equals(loadoutSelectPanel.name));
		}

		public void OpenModalWith(string title, string buttonName, bool textBox, ModalAction action) {
			modal.transform.parent.gameObject.SetActive(true);
			modalTitle.text = title;
			modalInput.gameObject.SetActive(textBox);
			modalButtonText.text = buttonName;
			modalButton.onClick.AddListener(() => action(modalInput.text));
			modalButton.onClick.AddListener(() => CloseModal());
		}

		public void CloseModal() {
			modal.transform.parent.gameObject.SetActive(false);
		}

		public void LoadScene(string sceneName) {
			SceneManager.LoadScene(sceneName);
		}

		public void ExitGame() {
			Application.Quit();
		}
	}
}
