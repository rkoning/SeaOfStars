using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class WeaponSystems : MonoBehaviourPunCallbacks, IPunObservable {

		private bool primaryInput;
		private bool primaryHeld;
		private float primaryHoldTime;
		private bool primaryTapped;

		private bool secondaryInput;
		private bool secondaryHeld;
		private float secondaryHoldTime;

		public float holdTime;

		public List<Weapon> primaryGroup = new List<Weapon>();
		public List<Weapon> secondaryGroup = new List<Weapon>();

		public PlayerController player;

		void Update() {
			if (!PhotonNetwork.IsConnected || photonView.IsMine) {
				primaryInput = player.PrimaryFireInput;
				secondaryInput = player.SecondaryFireInput;
			}

			if (primaryInput) {
				if (!primaryTapped) {
					Debug.Log("Tap");
					primaryTapped = true;
					for(int i = 0; i < primaryGroup.Count; i++) {
						primaryGroup[i].OnTap();
					}
					primaryHoldTime = holdTime + Time.fixedTime;
				} else if (primaryHoldTime < Time.fixedTime) {
					Debug.Log("Hold");
					primaryHeld = true;
					for (int i = 0; i < primaryGroup.Count; i++) {
						primaryGroup[i].OnHold();
					}
				}
			} else {
				if (primaryHeld) {
					Debug.Log("Release");
					for(int i = 0; i < primaryGroup.Count; i++) {
						primaryGroup[i].OnRelease();
					}
				}
				primaryTapped = false;
				primaryHeld = false;

			}

			if (secondaryInput) {
				for(int i = 0; i < secondaryGroup.Count; i++) {
					secondaryGroup[i].OnTap();
				}
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				// We own this player: send the others our data
				stream.SendNext(primaryInput);
				stream.SendNext(secondaryInput);
			} else {
				// Network player: read the stream
				primaryInput = (bool)stream.ReceiveNext();
				secondaryInput = (bool)stream.ReceiveNext();
			}
		}

	}
}
