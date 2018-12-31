using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class ParticleSystemWeapon : Weapon {

		public ParticleSystem[] projectors;
		private ParticleSystem prefab;

		protected override void Start() {
			base.Start();
			prefab = GetComponentInChildren<ParticleSystem>();
			projectors = new ParticleSystem[firingPoints.Length];
			for (int i = 0; i < firingPoints.Length; i++) {
				var _gun = (GameObject) Instantiate(prefab.gameObject, firingPoints[i].position, firingPoints[i].rotation, firingPoints[i]);
				projectors[i] = _gun.GetComponent<ParticleSystem>();
				_gun.GetComponent<ParticleDamage>().damage = this.damage;
				_gun.GetComponent<ParticleDamage>().playerNumber = this.playerNumber;
			}
			Destroy(prefab.gameObject);
		}

		public override bool OnTap() {
			isFiring = base.OnTap();
			return isFiring;
		}

		public override bool OnHold() {
			if (base.OnHold()) {
				for (int i = 0; i < projectors.Length; i++) {
					projectors[i].Play();
				}
				return true;
			}
			return false;
		}

		public override bool OnRelease() {
			if (base.OnRelease()) {
				for (int i = 0; i < projectors.Length; i++) {
					projectors[i].Stop();
				}
				return true;
			}
			return false;
		}
		// void Update() {
		// 	if (isFiring) {
		// 		if (Time.fixedTime < nextShot)
		// 			isFiring = false;
		// 		for (int i = 0; i < projectors.Length; i++) {
		// 			projectors[i].Play();
		// 		}
		// 	} else {
		// 		for (int i = 0; i < projectors.Length; i++) {
		// 			projectors[i].Stop();
		// 		}
		// 	}
		// }
	}
}
