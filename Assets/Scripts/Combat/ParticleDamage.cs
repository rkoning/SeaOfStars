using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class ParticleDamage : MonoBehaviour {

		private ParticleSystem particles;
		[HideInInspector]
		public int damage;
		[HideInInspector]
		public int playerNumber;

		void Start() {
			particles = GetComponent<ParticleSystem>();
		}

		void OnParticleCollision(GameObject other) {
			var _hp = other.GetComponent<Health>();
			if (_hp != null && other.GetComponent<PlayerController>().ActorNumber != playerNumber) {
				_hp.TakeDamage(damage, playerNumber);
			}
		}
	}
}
