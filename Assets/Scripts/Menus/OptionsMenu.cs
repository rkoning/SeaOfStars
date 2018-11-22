using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Com.Cegorach.SeaOfStars {
	public class OptionsMenu : MonoBehaviour {

		const string playerNamePrefKey = "PlayerName";
		[SerializeField]
		private InputField nameInput;

		void Start() {
			string defaultName = string.Empty;
			if (nameInput != null && PlayerPrefs.HasKey(playerNamePrefKey)) {
				defaultName = PlayerPrefs.GetString(playerNamePrefKey);
				nameInput.text = defaultName;
			}

			PhotonNetwork.NickName = defaultName;
		}

		public void SetPlayerName(string value) {
			if (string.IsNullOrEmpty(value))
			{
					Debug.LogError("Player Name is null or empty");
					return;
			}
			PhotonNetwork.NickName = value;


			PlayerPrefs.SetString(playerNamePrefKey,value);
		}
	}
}
