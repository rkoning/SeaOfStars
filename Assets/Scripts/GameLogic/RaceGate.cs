using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class RaceGate : MonoBehaviour {

		public RaceGate nextGate;
		public Transform spawnPoint;
		[HideInInspector]
		public bool isLastGate;
		public RaceGameLogic race;

		//
		// Checks if the colliding object is the local player, if so set the next spawn point to this transform and
		// update the current gate for this player. Triggers the SetSpawnPoint RPC in RaceGameLogic
		//
		void OnTriggerEnter(Collider other) {
			var _player = other.GetComponent<PlayerController>();
			if (_player != null && _player.IsLocal) {
				if (isLastGate)
					race.Score(_player.ActorNumber, 1);
				nextGate.SetActive(true);
				SetActive(false);
				race.SetSpawnPoint(transform, _player.ActorNumber);
			}
		}

		//
		// Sets the state of the renderer and collider of the gate
		//
		public void SetActive(bool active) {
			GetComponent<Renderer>().enabled = active;
			GetComponent<Collider>().enabled = active;
		}
	}
}
