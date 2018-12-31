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
		private float ignoreCollisionsUntil = 0.25f;

		void Awake() {
			rb = GetComponent<Rigidbody>();
		}

		void OnCollisionEnter(Collision other) {
			var _hp = other.collider.GetComponent<Health>();
			if (_hp && other.collider.GetComponent<PlayerController>().ActorNumber != playerNumber) {
				Debug.Log(other.collider.GetComponent<PlayerController>().ActorNumber + " " + playerNumber);
				_hp.TakeDamage(damage, playerNumber);
			}
			gameObject.SetActive(false);
		}

		public void Launch(Vector3 direction) {
			StartCoroutine(WaitThenCollide());
			rb.velocity = Vector3.zero;
			rb.AddForce(speed * direction);
		}

		private IEnumerator WaitThenCollide() {
			GetComponent<Collider>().enabled = false;
			yield return new WaitForSeconds(ignoreCollisionsUntil);
			GetComponent<Collider>().enabled = true;
		}

	}
}
