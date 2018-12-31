using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class PlayerListEntry : MonoBehaviourPunCallbacks, IPunObservable {

		[Header("UI References")]
		public Text PlayerNameText;
		public Image PlayerColorImage;
		public Button PlayerReadyButton;
		public Image PlayerReadyImage;
		public Text PlayerShipText;

		private int ownerId;
		private bool isPlayerReady;

		public void OnEnable() {
			PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
		}

		public void Start()	{
			if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId) {
				PlayerReadyButton.gameObject.SetActive(false);
			}	else {
				Hashtable initialProps = new Hashtable() {{SeaOfStarsGame.PLAYER_READY, isPlayerReady}, {SeaOfStarsGame.PLAYER_LIVES, SeaOfStarsGame.PLAYER_MAX_LIVES}};
				PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
				PhotonNetwork.LocalPlayer.SetScore(0);

				PlayerReadyButton.onClick.AddListener(() =>	{
					isPlayerReady = !isPlayerReady;
					SetPlayerReady(isPlayerReady);

					Hashtable props = new Hashtable() {
						{SeaOfStarsGame.PLAYER_READY, isPlayerReady}
					};
					PhotonNetwork.LocalPlayer.SetCustomProperties(props);

					if (PhotonNetwork.IsMasterClient)	{
						FindObjectOfType<RoomBrowser>().LocalPlayerPropertiesUpdated();
					}
				});
			}
			transform.SetParent(FindObjectOfType<RoomBrowser>().playerListContent.transform, false);
			FindObjectOfType<RoomBrowser>().FormatListEntry(gameObject, photonView.OwnerActorNr - 1);
			PlayerNameText.text = photonView.Owner.NickName;
		}

		public void OnSetLoadout(Loadout newLoadout) {
			Hashtable props = new Hashtable() {
				{SeaOfStarsGame.PLAYER_SELECTED_SHIP, newLoadout.ship},
				{SeaOfStarsGame.PLAYER_SELECTED_PRIMARY_WEAPON, newLoadout.primary},
				{SeaOfStarsGame.PLAYER_SELECTED_SECONDARY_WEAPON, newLoadout.secondary},
			};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
			photonView.RPC("OnShipTextChange", RpcTarget.AllBuffered, photonView.Owner.ActorNumber, newLoadout.ship);
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

		}
		public override void OnLeftRoom() {
			PhotonNetwork.Destroy(gameObject);
		}
		[PunRPC]
		public void OnShipTextChange(int sender, string shipName) {
			Debug.Log("Ship Name: " + shipName);
			Debug.Log(PlayerShipText.text);
			PlayerShipText.text = shipName;
			Debug.Log(PlayerShipText.text);
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
				if (photonView.IsMine) {
					Hashtable props = new Hashtable() {
						{SeaOfStarsGame.PLAYER_READY, playerReady},
					};
					PhotonNetwork.LocalPlayer.SetCustomProperties(props);
				}
		}
	}
}
