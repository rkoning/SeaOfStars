using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.Cegorach.SeaOfStars {
	public class Laser : Weapon {

		public float duration;
		public float range;
		public LayerMask mask;
		public Transform[] firingPoints;

		private float durationEnd;
		private bool isFiring;
		private LineRenderer[] beams;

		void Start() {
			Debug.Log(photonView.IsMine);
			beams = new LineRenderer[firingPoints.Length];
			for (int i = 0; i < firingPoints.Length; i++) {
				beams[i] = firingPoints[i].GetComponent<LineRenderer>();
				beams[i].enabled = false;
			}
		}

		void Update() {
			if ((photonView.IsMine || PhotonNetwork.IsConnected == false) && Time.fixedTime > durationEnd) {
					isFiring = false;
			}
			if (isFiring) {
				for(int i = 0; i < firingPoints.Length; i++) {
					FireBeam(firingPoints[i], beams[i]);
				}
			} else {
				DisableBeams();
			}
		}

		public override bool Fire() {
			if (base.Fire()) {
				isFiring = true;
				durationEnd = Time.fixedTime + duration;
				return true;
			}
			return false;
		}

		public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
    		// We own this player: send the others our data
				stream.SendNext(isFiring);
			} else {
				// Network player: read the stream
				isFiring = (bool)stream.ReceiveNext();
			}
		}

		protected void FireBeam(Transform origin, LineRenderer beam) {
			beam.enabled = true;
			RaycastHit hit;
			if (Physics.Raycast(origin.position, origin.forward, out hit, range, mask)) {
				beam.SetPositions( new Vector3[] {origin.position, hit.point} );
				var h = hit.collider.GetComponent<Health>();
				if (h != null) {
					h.TakeDamage(damage, photonView.Owner.ActorNumber);
				}
			} else {
				beam.SetPositions( new Vector3[] {origin.position, origin.position + origin.forward * range} );
			}
		}

		protected void DisableBeams() {
			foreach(var beam in beams) {
				beam.enabled = false;
			}
		}
	}
}
