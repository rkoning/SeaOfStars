using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.RyanKoning.SeaOfStars {
	public class StageListEntry : MonoBehaviour {

		public Stage stage;

		[SerializeField]
		private Text stageNameText;
		[SerializeField]
		private Button selectButton;
		[SerializeField]
		private Image selectedIndicator;

		[SerializeField]
		private Color selectedColor;
		[SerializeField]
		private Color originalColor;

		private RoomBrowser browser;
		public RoomBrowser Browser { set { browser = value; } }

		void Start() {
			// originalColor = selectedIndicator.color;
			stageNameText.text = stage.name;
			selectButton.onClick.AddListener(delegate { browser.SetStage(this); } );
		}

		public void OnSelect() {
			foreach(var sle in FindObjectsOfType<StageListEntry>()) {
				sle.OnDeselect();
				Debug.Log(sle);
			}
			selectedIndicator.color = selectedColor;
		}

		public void OnDeselect() {
			selectedIndicator.color = originalColor;
		}
	}
}
