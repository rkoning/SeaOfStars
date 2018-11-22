using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;

namespace Com.Cegorach.SeaOfStars {
	public class PlayerHealth : Health {

		public Slider healthBar;

		[SerializeField]
		private GameObject shipBody;
		private Rigidbody rb;
		private bool dead = false;
		private int lastHit = -1;

		protected override void Start() {
			base.Start();
			rb = GetComponent<Rigidbody>();
		}

		void Update() {
			if (currentHealth <= 0 && !dead) {
				Die(lastHit);
			}
		}

		public override void Die(int enemyID) {
			if (!dead) {
				dead = true;
				deathParticles.Play();
				rb.velocity = Vector3.zero;
				rb.isKinematic = true;
				shipBody.SetActive(false);
				GetComponent<Collider>().enabled = false;
				StartCoroutine(Respawn(GameLogic.Instance.respawnTime));
				PhotonView view = PhotonView.Get(this);
				if (PhotonNetwork.IsConnected) {
					view.RPC("OnPlayerDeath", RpcTarget.Others, photonView.Owner.ActorNumber, enemyID);
				}
			}
		}

		[PunRPC]
		public void OnPlayerDeath(int playerID, int enemyID) {
			GameLogic.Instance.OnPlayerDeath(playerID, enemyID);
			Die(enemyID);
			Announcer.Instance.ShowNotice(playerID + " was killed by " + enemyID);
		}

		public override void TakeDamage(int damage, int enemyID) {
			lastHit = enemyID;
			base.TakeDamage(damage, enemyID);
			if (healthBar != null) {
				healthBar.value = (float) currentHealth / maxHealth;
			}
		}

		IEnumerator Respawn(float respawnTime) {
			yield return new WaitForSeconds(respawnTime);
			// Get a spawnPoint and set our position / rotation
			var spawnPoint = GameLogic.Instance.GetSpawnPoint(photonView.Owner.ActorNumber);
			transform.position = (Vector3)spawnPoint[0];
			transform.rotation = (Quaternion)spawnPoint[1];
			// Reset attributes
			rb.isKinematic = false;
			shipBody.SetActive(true);
			currentHealth = maxHealth;
			if (healthBar != null) {
				healthBar.value = 1;
			}
			GetComponent<Collider>().enabled = true;
			// Now, you have my permission to die.
			dead = false;
		}

		void OnCollisionEnter(Collision c) {
			TakeDamage((int) Mathf.Sqrt(c.impulse.sqrMagnitude), -1);
		}
	}
}
