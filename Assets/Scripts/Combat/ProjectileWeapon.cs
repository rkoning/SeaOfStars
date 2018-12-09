using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class ProjectileWeapon : Weapon {

		public GameObject projectilePrefab;
		private GameObject[] projectiles;
		[SerializeField]
		private int maxProjectiles;
		private int currentProjectile = 0;

		void Start() {
			projectiles = new GameObject[maxProjectiles];

			for (int i = 0; i < maxProjectiles; i++) {
				projectiles[i] = GameObject.Instantiate(projectilePrefab);
				var p = projectiles[i].GetComponent<Projectile>();
				p.damage = damage;
				p.playerNumber = playerNumber;
				projectiles[i].SetActive(false);
			}
		}

		public override bool OnTap() {
			if (base.OnTap()) {
				Fire();
				return true;
			}
			return false;
		}

		private void Fire() {
			for(int i = 0; i < firingPoints.Length; i++) {
				FireProjectileFrom(firingPoints[i]);
			}
		}

		private void FireProjectileFrom(Transform point) {
			projectiles[currentProjectile].SetActive(true);
			projectiles[currentProjectile].transform.position = point.position;
			projectiles[currentProjectile].transform.rotation = point.rotation;
			projectiles[currentProjectile].GetComponent<Projectile>().Launch(point.forward);
			currentProjectile++;
			if (currentProjectile > maxProjectiles - 1)
				currentProjectile = 0;
		}
	}
}
