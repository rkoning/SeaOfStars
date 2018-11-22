using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

namespace Com.Cegorach.SeaOfStars {
	public class PlayerListEntry : MonoBehaviour {

		[Header("UI References")]
		public Text PlayerNameText;
		public Image PlayerColorImage;
		public Button PlayerReadyButton;
		public Image PlayerReadyImage;

		private int ownerId;
		private bool isPlayerReady;

		public void OnEnable()
		{
				PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
		}

		public void Start()
		{
				Debug.Log("Started");
				if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
				{
						PlayerReadyButton.gameObject.SetActive(false);
				}
				else
				{
						Hashtable initialProps = new Hashtable() {{SeaOfStarsGame.PLAYER_READY, isPlayerReady}, {SeaOfStarsGame.PLAYER_LIVES, SeaOfStarsGame.PLAYER_MAX_LIVES}};
						PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
						PhotonNetwork.LocalPlayer.SetScore(0);

						PlayerReadyButton.onClick.AddListener(() =>
						{
								isPlayerReady = !isPlayerReady;
								SetPlayerReady(isPlayerReady);

								Hashtable props = new Hashtable() {{SeaOfStarsGame.PLAYER_READY, isPlayerReady}};
								PhotonNetwork.LocalPlayer.SetCustomProperties(props);

								if (PhotonNetwork.IsMasterClient)
								{
										FindObjectOfType<RoomBrowser>().LocalPlayerPropertiesUpdated();
								}
						});
				}
		}

		public void OnDisable()	{
			PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
		}

		public void Initialize(int playerId, string playerName) {
			ownerId = playerId;
			PlayerNameText.text = playerName;
		}

		private void OnPlayerNumberingChanged()
		{
				foreach (Player p in PhotonNetwork.PlayerList)
				{
						if (p.ActorNumber == ownerId)
						{
								PlayerColorImage.color = SeaOfStarsGame.GetColor(p.GetPlayerNumber());
						}
				}
		}

		public void SetPlayerReady(bool playerReady)
		{
				PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
				PlayerReadyImage.enabled = playerReady;
		}
	}
}
