using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

namespace Com.RyanKoning.SeaOfStars {
	public class RaceGameLogic : GameLogic, IOnEventCallback {

		private Transform raceGatesParent;
		private RaceGate[] raceGates;

		private int numLaps = 1;

		private int numPlayersFinished = 0;
		private int numPlayers;

		private readonly byte setPlayerSpawnEvent = 0;
		private readonly byte playerScoredEvent = 1;

		void Start() {
			SetupGates();

			spawnPoints = new Transform[transform.childCount];
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).tag == "Race SpawnPoint") {
					spawnPoints[i] = transform.GetChild(i);
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}

			numPlayers = PhotonNetwork.IsConnected ? PhotonNetwork.PlayerList.Length : 1;
			SpawnPlayers();
		}

		private void SetupGates() {
			raceGatesParent = GameObject.Find("RaceGates").transform;
			raceGates = new RaceGate[raceGatesParent.childCount];
			raceGates[0] = raceGatesParent.GetChild(0).GetComponentInChildren<RaceGate>();
			raceGates[0].race = this;
			for(int i = 1; i < raceGatesParent.childCount; i++) {
				raceGates[i] = raceGatesParent.GetChild(i).GetComponentInChildren<RaceGate>();
				raceGates[i - 1].nextGate = raceGates[i];
				raceGates[i].race = this;
				raceGates[i].SetActive(false);
			}
			raceGates[0].SetActive(true);
			raceGates[raceGates.Length - 1].nextGate = raceGates[0];

			raceGates[0].isLastGate = true;
		}

		public override void SpawnPlayers() {
			if (playerPrefab != null) {
				if (PlayerController.LocalPlayerInstance == null) {
					int _actorNumber = PhotonNetwork.IsConnected == true ? PhotonNetwork.LocalPlayer.ActorNumber - 1 : 0;
					Transform spawnPoint = spawnPoints[_actorNumber];
					SetSpawnPoint(spawnPoint, _actorNumber);
					PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
				}
			}
		}

		public void SetSpawnPoint(Transform newSpawnPoint, int playerID) {
			if (PhotonNetwork.IsConnected == true) {
				object[] _content = new object[] { newSpawnPoint.position, newSpawnPoint.rotation, playerID };
				RaiseEventOptions _raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
				SendOptions _sendOptions = new SendOptions { Reliability = true };
				PhotonNetwork.RaiseEvent(setPlayerSpawnEvent, _content, _raiseEventOptions, _sendOptions);
			} else {
				players[playerID].SetSpawnPoint(newSpawnPoint);
			}
		}

		//
		// Returns the players last spawnPosition and rotation
		//
		public override object[] GetSpawnPoint(int playerID) {
			return new object[] {players[playerID].spawnPosition, players[playerID].spawnRotation};
		}

		//
		// OnPlayerDeath is ignored in Race mode
		//
		public override void OnPlayerDeath(int playerID, int enemyID) {
			return;
		}

		//
		// Score is called whenever a player finishes a lap, once the score for a player is over numLaps, they have finished the race.
		//
		public override void Score(int playerID, int points) {
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.RaiseEvent(
					playerScoredEvent,
					new object[] {points, playerID},
					new RaiseEventOptions { Receivers = ReceiverGroup.All },
					new SendOptions { Reliability = true }
				);
			} else {
				IncrementPlayerScore(playerID, points);
			}
		}

		public void OnPlayerScored(Dictionary<byte, object> parameters) {
			object[] _data = (object[])parameters[(byte) 245];
			int _points = (int)_data[0];
			int _playerID = (int) _data[1];
			IncrementPlayerScore(_playerID, _points);
		}

		//
		// Adds to a players score and shows a notice, if the players score is greater than the number of laps
		// that must be completed, they have finished the race.
		//
		private void IncrementPlayerScore(int playerID, int points) {
			players[playerID].AddScore(points);
			if (players[playerID].score > numLaps) {
				PlayerFinished(playerID);
				Announcer.Instance.ShowNotice(players[playerID].name + " Finished!");
			} else {
				Announcer.Instance.ShowNotice("Lap " + players[playerID].score + "/" + numLaps);
			}
		}

		private void PlayerFinished(int playerID) {
			numPlayersFinished += 1;
			Debug.Log("Player Finished: " + playerID);
			Debug.Log(numPlayersFinished);
			if (numPlayersFinished >= numPlayers - 1) {
				Announcer.Instance.ShowScoreBoard();
				SettleMatch();
				StartCoroutine(WaitThenEndGame(endGameWait));
			}
		}

		public void OnSetSpawnPoint(Dictionary<byte, object> parameters) {
			object[] _data = (object[])parameters[(byte) 245];
			Vector3 _spawnPosition = (Vector3)_data[0];
			Quaternion _spawnRotation = (Quaternion)_data[1];
			int _id = (int) parameters[(byte) 254];
			players[_id].SetSpawnPoint(_spawnPosition, _spawnRotation);
		}

		public void OnEvent(EventData photonEvent) {
			if (photonEvent.Code == setPlayerSpawnEvent) {
				OnSetSpawnPoint(photonEvent.Parameters);
			} else if (photonEvent.Code == playerScoredEvent) {
				OnPlayerScored(photonEvent.Parameters);
			}

			// OnSetSpawnPointEvent(photonEvent.Code, photonEvent.content, photonEvent.raiseEventOptions, photonEvent.sendOptions);
		}

		public void OnEnable() {
			if (PhotonNetwork.IsConnected == true)
		  	PhotonNetwork.AddCallbackTarget(this);
		}

		public void OnDisable() {
			if (PhotonNetwork.IsConnected == true)
		  	PhotonNetwork.RemoveCallbackTarget(this);
		}
	}
}
