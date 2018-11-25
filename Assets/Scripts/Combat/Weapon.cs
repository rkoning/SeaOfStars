using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public abstract class Weapon : MonoBehaviour {
		public int damage;
		public float coolDown;
		private float nextShot;
		public Transform[] firingPoints;

		public virtual bool OnTap() {
			if (Time.fixedTime > nextShot) {
				nextShot = Time.fixedTime + coolDown;
				return true;
			}
			return false;
		}
	}
}
