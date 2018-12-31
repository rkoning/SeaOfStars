using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.RyanKoning.SeaOfStars {
	public class RoomBrowser : MonoBehaviourPunCallbacks {

    [Header("Create Room Panel")]
		[SerializeField]
		private GameObject hostPanel;
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players.")]
		[SerializeField]
		private byte maxPlayersPerRoom = 4;
		[SerializeField]
		private InputField RoomNameInputField;

    [Header("Room List Panel")]
		[SerializeField]
		private GameObject browserPanel;
		[SerializeField]
		private GameObject roomListContent;
		[SerializeField]
		private GameObject roomListEntryPrefab;
		[SerializeField]
		private Text loggedInText;

    [Header("Loading")]
		[SerializeField]
		private GameObject progressText;

		[Header("In Lobby Panel")]
		[SerializeField]
		private GameObject roomLobbyPanel;
		[SerializeField]
		public GameObject playerListContent;
		[SerializeField]
		private GameObject playerListEntryPrefab;
		[SerializeField]
		private GameObject startGameButton;
		[SerializeField]
		private Text roomName;

		[Header("Stage Selection")]
		[SerializeField]
		private GameObject stageListEntryPrefab;
		[SerializeField]
		private GameObject hostOptions;
		[SerializeField]
		private GameObject stageListContent;
		[SerializeField]
		private Text subtitleText;
		[SerializeField]
		private Stage[] stages;

		private string selectedStage;
		private string selectedGameMode;

		[Header("Game Mode Selection")]
		[SerializeField]
		private Dropdown gameModeDropdown;

		[Header("Loadout Selection")]
		[SerializeField]
		private Transform loadoutListContent;
		[SerializeField]
		private GameObject loadoutListEntryPrefab;

	  private Dictionary<string, RoomInfo> cachedRoomList;
	  private Dictionary<string, GameObject> roomListEntries;
		private Dictionary<int, GameObject> playerListEntries;

		private PlayerListEntry localListEntry;

		void Awake() {
			PhotonNetwork.AutomaticallySyncScene = true;
	    cachedRoomList = new Dictionary<string, RoomInfo>();
	    roomListEntries = new Dictionary<string, GameObject>();
		}

		void Start() {
			SetActivePanel(progressText.name);
			if (!PhotonNetwork.IsConnected) {
				PhotonNetwork.ConnectUsingSettings();
			} else {
				SetActivePanel(browserPanel.name);
			}

			string _name = PlayerPrefs.GetString("PlayerName");
			PhotonNetwork.NickName = _name;
			loggedInText.text = "Connected as: " + _name;
		}

		// Photon Callbacks
		public override void OnConnectedToMaster() {
		   Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
			 if (!PhotonNetwork.InLobby) {
				 PhotonNetwork.JoinLobby();
			 }
			 SetActivePanel(browserPanel.name);
		}

		public override void OnDisconnected(DisconnectCause cause) {
			SetActivePanel(browserPanel.name);
	    Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
		}

		// called when we join a room
		// finds the stage name if we are not the host, then creates list entries for each player (including us) that is in the room
		public override void OnJoinedRoom() {
			SetActivePanel(roomLobbyPanel.name);
			InitializeLoadoutList();
			hostOptions.SetActive(PhotonNetwork.IsMasterClient);
			if (PhotonNetwork.IsMasterClient) {
				InitializeStageList();
			} else {
				object _stageName;
				object _gameMode;
				if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_STAGE, out _stageName)
						&& PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, out _gameMode)) {
					subtitleText.text = (string) _gameMode + " On " + (string) _stageName;
				}
			}

			roomName.text = PhotonNetwork.CurrentRoom.Name;
			if (playerListEntries == null) {
				playerListEntries = new Dictionary<int, GameObject>();
			}

			// if (PhotonNetwork.IsMasterClient) {
			// foreach(Player p in PhotonNetwork.PlayerList) {
			// 	GameObject entry = PhotonNetwork.Instantiate(playerListEntryPrefab.name, Vector3.zero, Quaternion.identity, 0);
			// 	entry.transform.SetParent(playerListContent.transform);
			// 	FormatListEntry(entry, p.ActorNumber);
			// 	entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);
			//
			// 	object isPlayerReady;
			// 	if (p.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_READY, out isPlayerReady)) {
			// 		entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
			// 	}
			// }
			// }

			GameObject entry = PhotonNetwork.Instantiate(playerListEntryPrefab.name, Vector3.zero, Quaternion.identity, 0);
			entry.transform.SetParent(playerListContent.transform);
			FormatListEntry(entry, PhotonNetwork.LocalPlayer.ActorNumber);
			entry.GetComponent<PlayerListEntry>().Initialize(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName);
			localListEntry = entry.GetComponent<PlayerListEntry>();

			startGameButton.gameObject.SetActive(CheckPlayersReady());
			ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable {
				{SeaOfStarsGame.PLAYER_LOADED_LEVEL, false}
			};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
		}

		public override void OnLeftRoom() {
			SetActivePanel(browserPanel.name);
			playerListEntries.Clear();
			playerListEntries = null;
		}

		// Called when a remote player enters the room
		public override void OnPlayerEnteredRoom(Player newPlayer) {
			// PlayerListEntry newPlayerEntry = null;
			// foreach (PhotonView v in FindObjectsOfType<PhotonView>()) {
			// 	if (v.OwnerActorNr == newPlayer.ActorNumber) {
			// 		newPlayerEntry = v.gameObject.GetComponent<PlayerListEntry>();
			// 	}
			// 	Debug.Log(v.OwnerActorNr);
			// }
			// Debug.Log(newPlayer.ActorNumber);
			// playerListEntries.Add(newPlayer.ActorNumber, newPlayerEntry.gameObject);
			startGameButton.gameObject.SetActive(CheckPlayersReady());
		}

		// called when a remote player leaves the room
		public override void OnPlayerLeftRoom(Player otherPlayer) {
			playerListEntries.Remove(otherPlayer.ActorNumber);
			startGameButton.gameObject.SetActive(CheckPlayersReady());
			Debug.Log(PhotonNetwork.PlayerList.Length);
		}

		public override void OnMasterClientSwitched(Player newMasterClient) {
			if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)	{
				startGameButton.gameObject.SetActive(CheckPlayersReady());
			}
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
			if (playerListEntries == null) {
				playerListEntries = new Dictionary<int, GameObject>();
			}

			GameObject entry;
			if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry)) {
				object isPlayerReady;
				if (changedProps.TryGetValue(SeaOfStarsGame.PLAYER_READY, out isPlayerReady))	{
					entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
				}
			}
			startGameButton.gameObject.SetActive(CheckPlayersReady());
		}

		public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps) {
			object _stageName;
			object _gameMode;
			if (changedProps.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, out _gameMode)) {
				selectedGameMode = (string) _gameMode;
			}
			if (changedProps.TryGetValue(SeaOfStarsGame.ROOM_SELECTED_STAGE, out _stageName)) {
				selectedStage = (string) _stageName;
			}
			subtitleText.text = selectedGameMode + " On " + selectedStage;
		}

	  public override void OnRoomListUpdate(List<RoomInfo> roomList) {
      ClearRoomList();
      UpdateCachedRoomList(roomList);
      UpdateRoomList();
	  }

		// UI Callbacks
		public void OnHostButtonClicked() {
			SetActivePanel(hostPanel.name);
		}

		public void OnBackButtonClicked() {
			if (hostPanel.activeInHierarchy) {
				SetActivePanel(browserPanel.name);
			} else {
				SceneManager.LoadScene(0);
			}
		}

		public void OnLeaveGameButtonClicked()
		{
			PhotonNetwork.Destroy(localListEntry.gameObject);
			PhotonNetwork.LeaveRoom();
		}

		public void OnStartGameButtonClicked()
		{
				PhotonNetwork.CurrentRoom.IsOpen = false;
				PhotonNetwork.CurrentRoom.IsVisible = false;

				PhotonNetwork.LoadLevel("Scenes/" + selectedStage);
		}

		public void OnCreateRoomButtonClicked() {
			SetActivePanel("Progress Label");

			string roomName = RoomNameInputField.text;
			roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

			RoomOptions options = new RoomOptions {MaxPlayers = maxPlayersPerRoom};

			PhotonNetwork.CreateRoom(roomName, options, null);
		}

		// Public Methods
    public void LocalPlayerPropertiesUpdated() {
      startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

		public void SetStage(StageListEntry sle) {
			selectedStage = sle.stage.name;
			selectedGameMode = sle.stage.gameModes[0].ToString();
			ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable {
				{SeaOfStarsGame.ROOM_SELECTED_STAGE, selectedStage},
				{SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, selectedGameMode},
				{SeaOfStarsGame.DEATHMATCH_SCORE_TO_WIN, 3}
			};
			PhotonNetwork.CurrentRoom.SetCustomProperties(props);
			sle.OnSelect();
			PopulateGameModes(sle.stage.gameModes);
		}

		public void SetGameMode(string gameMode) {
			ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable {
				{SeaOfStarsGame.ROOM_SELECTED_GAME_MODE, gameMode},
				{SeaOfStarsGame.DEATHMATCH_SCORE_TO_WIN, 3}
			};
			PhotonNetwork.CurrentRoom.SetCustomProperties(props);
		}

		// Private Methods
		private void PopulateGameModes(GameMode[] gameModes) {
			List<string> gameModeList = new List<string>();
			foreach(GameMode gm in gameModes) {
				gameModeList.Add(gm.ToString());
			}
			gameModeDropdown.ClearOptions();
			gameModeDropdown.AddOptions(gameModeList);

			gameModeDropdown.onValueChanged.AddListener( delegate { SetGameMode(gameModeDropdown.options[gameModeDropdown.value].text); } );
		}

		private void InitializeStageList() {
			for (int i = 0; i < stages.Length; i++) {
				var _stageListEntry = Instantiate(stageListEntryPrefab, i * new Vector3(0, 120 * i, 0), Quaternion.identity, null);
				_stageListEntry.transform.SetParent(stageListContent.transform);
				FormatListEntry(_stageListEntry, i);
				_stageListEntry.GetComponent<StageListEntry>().stage = stages[i];
				_stageListEntry.GetComponent<StageListEntry>().Browser = this;
			}
			SetStage(FindObjectOfType<StageListEntry>());
		}

		private void InitializeLoadoutList() {
			LoadoutManager lm = new LoadoutManager();
			Loadout[] loadouts = lm.GetAllLoadouts();
			for (int i = 0; i < loadouts.Length; i++) {
				var _loadoutEntry = Instantiate(loadoutListEntryPrefab, new Vector3(0, -110 * i, 0), Quaternion.identity, null);
				_loadoutEntry.transform.SetParent(loadoutListContent, false);
				var _thisLoadout = loadouts[i];
				_loadoutEntry.GetComponentInChildren<Button>().onClick.AddListener(delegate { SetLoadout(_thisLoadout); } );
				_loadoutEntry.GetComponentInChildren<Button>().interactable = _thisLoadout.IsComplete();
				_loadoutEntry.GetComponentInChildren<Text>().text = _thisLoadout.name;
			}
		}

		public void SetLoadout(Loadout selected) {
			localListEntry.GetComponent<PlayerListEntry>().OnSetLoadout(selected);
		}

		private void ClearRoomList() {
			foreach (GameObject entry in roomListEntries.Values)
			{
					Destroy(entry.gameObject);
			}
			roomListEntries.Clear();
		}

		private void UpdateRoomList() {
			int i = 0;
			foreach (RoomInfo info in cachedRoomList.Values)
			{
					GameObject entry = Instantiate(roomListEntryPrefab);
					entry.transform.SetParent(roomListContent.transform);
					FormatListEntry(entry, i);
					entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

					roomListEntries.Add(info.Name, entry);
					i++;
			}
		}

		private void UpdateCachedRoomList(List<RoomInfo> roomList) {
			foreach (RoomInfo info in roomList) {
					// Remove room from cached room list if it got closed, became invisible or was marked as removed
					if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {
							if (cachedRoomList.ContainsKey(info.Name)) {
									cachedRoomList.Remove(info.Name);
							}

							continue;
					}

					// Update cached room info
					if (cachedRoomList.ContainsKey(info.Name)) {
							cachedRoomList[info.Name] = info;
					}
					// Add new room info to cache
					else {
							cachedRoomList.Add(info.Name, info);
					}
			}
		}

		private void SetActivePanel(string activePanel)
		{
			hostPanel.SetActive(activePanel.Equals(hostPanel.name));
			browserPanel.SetActive(activePanel.Equals(browserPanel.name));
			progressText.SetActive(activePanel.Equals(progressText.name));
			roomLobbyPanel.SetActive(activePanel.Equals(roomLobbyPanel.name));
		}

		private bool CheckPlayersReady()
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return false;
			}

			foreach (Player p in PhotonNetwork.PlayerList)
			{
				object isPlayerReady;
				if (p.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_READY, out isPlayerReady))
				{
					if (!(bool) isPlayerReady)
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public void FormatListEntry(GameObject entry, int index) {
			var rt = entry.GetComponent<RectTransform>();
			rt.offsetMin = new Vector2(0,0);
			rt.offsetMax = new Vector2(0,100);
			rt.anchoredPosition = new Vector3(0,index * -120,0);
			entry.transform.localScale = Vector3.one;
		}
	}
}
