using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class WeaponSystems : MonoBehaviourPunCallbacks, IPunObservable {

		private bool primaryTapped;
		private bool primaryHeld;
		private float primaryHoldTime;

		private bool secondaryTapped;
		private bool secondaryHeld;
		private float secondaryHoldTime;

		public float holdTime;

		public List<Weapon> primaryGroup = new List<Weapon>();
		public List<Weapon> secondaryGroup = new List<Weapon>();

		public PlayerController player;

		void Start() {
			if (PhotonNetwork.IsConnected) {

			} else {
			}
		}

		void Update() {
			if (!PhotonNetwork.IsConnected || photonView.IsMine) {
				primaryTapped = player.PrimaryFireInput;
				secondaryTapped = player.SecondaryFireInput;
			}

			if (primaryTapped) {
				for(int i = 0; i < primaryGroup.Count; i++) {
					primaryGroup[i].OnTap();
				}
			}

			if (secondaryTapped) {
				for(int i = 0; i < secondaryGroup.Count; i++) {
					secondaryGroup[i].OnTap();
				}
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				// We own this player: send the others our data
				stream.SendNext(primaryTapped);
				stream.SendNext(secondaryTapped);
			} else {
				// Network player: read the stream
				primaryTapped = (bool)stream.ReceiveNext();
				secondaryTapped = (bool)stream.ReceiveNext();
			}
		}

	}
}
