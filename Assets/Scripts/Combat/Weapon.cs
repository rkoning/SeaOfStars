using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.Cegorach.SeaOfStars {
	public abstract class Weapon : MonoBehaviourPunCallbacks, IPunObservable {
		public int damage;
		public float coolDown;
		private float nextShot;

		public virtual bool Fire() {
			if (Time.fixedTime > nextShot) {
				nextShot = Time.fixedTime + coolDown;
				return true;
			}
			return false;
		}

		public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
	}
}
