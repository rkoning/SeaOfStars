using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public abstract class Health : MonoBehaviourPunCallbacks, IPunObservable {

		public int maxHealth;
		protected int currentHealth;
		public int CurrentHealth { get { return currentHealth; } }

		public Renderer recolorSurface;
		public GameObject shipBody;
		private Material defaultMaterial;
		public Material hitMaterial;

		private bool flashing;
		public float flashDuration = 0.2f;

		[SerializeField]
		protected ParticleSystem deathParticles;

		protected virtual void Start() {
			currentHealth = maxHealth;
			defaultMaterial = recolorSurface.material;
		}

		public virtual void TakeDamage(int damage, int enemyID) {
			currentHealth -= damage;
			if (currentHealth <= 0) {
				Die(enemyID);
				return;
			}

			if (!flashing) {
				flashing = true;
				StartCoroutine(DamageFlash());
			}
		}

		public abstract void Die(int enemyID);

		protected virtual IEnumerator DamageFlash() {
			recolorSurface.material = hitMaterial;
			yield return new WaitForSeconds(flashDuration);
			recolorSurface.material = defaultMaterial;
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				stream.SendNext(currentHealth);
			} else {
				currentHealth = (int)stream.ReceiveNext();
			}
		}
	}
}
