using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public abstract class Weapon : MonoBehaviour {
		public int damage;
		public float coolDown;
		protected float nextShot;
		protected bool isFiring;
		public Transform[] firingPoints;
		public int playerNumber;

		public virtual bool OnTap() {
			if (Time.fixedTime > nextShot) {
				nextShot = Time.fixedTime + coolDown;
				return true;
			}
			return false;
		}

		public virtual void OnHold() {

		}

		public virtual void OnRelease() {

		}
	}
}
