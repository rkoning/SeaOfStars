using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

namespace Com.Cegorach.SeaOfStars {
	public class RaceGameLogic : GameLogic, IOnEventCallback {

		private Transform raceGatesParent;
		private RaceGate[] raceGates;

		private int numLaps;

		private readonly byte setPlayerSpawnEvent = 0;

		void Start() {
			SetupGates();

			spawnPoints = new Transform[transform.childCount];
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).tag == "Race SpawnPoint") {
					spawnPoints[i] = transform.GetChild(i);
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}
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

		public override object[] GetSpawnPoint(int playerID) {
			Debug.Log(playerID);
			return new object[] {players[playerID].spawnPosition, players[playerID].spawnRotation};
		}

		public override void OnPlayerDeath(int playerID, int enemyID) {

		}

		public override void Score(int playerID, int points) {
		}

		public void OnSetSpawnPoint(Dictionary<byte, object> parameters)	{
			object[] _data = (object[])parameters[(byte) 245];
			Vector3 _spawnPosition = (Vector3)_data[0];
			Quaternion _spawnRotation = (Quaternion)_data[1];
			int _id = (int) parameters[(byte) 254];
			players[_id].SetSpawnPoint(_spawnPosition, _spawnRotation);
		}

		public void OnEvent(EventData photonEvent) {
			if (photonEvent.Code == setPlayerSpawnEvent) {
				OnSetSpawnPoint(photonEvent.Parameters);
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
