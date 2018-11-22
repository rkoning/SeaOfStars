using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Cegorach.SeaOfStars {
	public class StageListEntry : MonoBehaviour {

		public string stageName;
		public GameMode[] gameModes;

		[SerializeField]
		private Text stageNameText;
		[SerializeField]
		private Button selectButton;
		[SerializeField]
		private Image selectedIndicator;

		[SerializeField]
		private Color selectedColor;
		private Color originalColor;

		private RoomBrowser browser;
		public RoomBrowser Browser { set { browser = value; } }

		void Start() {
			originalColor = selectedIndicator.color;
			stageNameText.text = stageName;
		}

		public void OnSelect() {
			foreach(var sle in FindObjectsOfType<StageListEntry>()) {
				sle.OnDeselect();
			}
			selectedIndicator.color = selectedColor;
			if (browser != null) {
				browser.SetStage(this);
			}
		}

		public void OnDeselect() {
			selectedIndicator.color = originalColor;
		}
	}
}
