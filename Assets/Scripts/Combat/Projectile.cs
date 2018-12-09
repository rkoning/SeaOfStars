using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class Projectile : MonoBehaviour {

		public int damage;
		public int playerNumber;
		[SerializeField]
		private float speed;
		private Rigidbody rb;

		void Awake() {
			rb = GetComponent<Rigidbody>();
		}

		public void Launch(Vector3 direction) {
			rb.velocity = Vector3.zero;
			rb.AddForce(speed * direction);
		}

		void OnCollisionEnter(Collision other) {
			var _hp = other.collider.GetComponent<Health>();
			if (_hp && other.collider.GetComponent<PlayerController>().ActorNumber != playerNumber) {
				Debug.Log(other.collider.GetComponent<PlayerController>().ActorNumber + " " + playerNumber);
				_hp.TakeDamage(damage, playerNumber);
			}
			gameObject.SetActive(false);
		}
	}
}
