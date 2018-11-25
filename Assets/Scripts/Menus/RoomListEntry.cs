using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class RoomListEntry : MonoBehaviour {
		public Text RoomNameText;
		public Text RoomPlayersText;
		public Button JoinRoomButton;

		private string roomName;

		public void Start()
		{
				JoinRoomButton.onClick.AddListener(() =>
				{
						PhotonNetwork.JoinRoom(roomName);
				});
		}

		public void Initialize(string name, byte currentPlayers, byte maxPlayers)
		{
				roomName = name;

				RoomNameText.text = name;
				RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
		}
	}
}
