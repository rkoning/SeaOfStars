using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public abstract class Weapon : MonoBehaviour {

		public new string name;
		public int damage;
		public float coolDown;
		public float magazineSize;
		public float reloadTime;
		public float ammoConsumptionTap;
		public float ammoConsumptionHold;
		public float ammoConsumptionRelease;

		protected float nextShot;
		protected float shotsRemaining;
		protected float reloadEnd;
		protected bool reloading;

		protected bool isFiring;
		public Transform[] firingPoints;
		public int playerNumber;
		[SerializeField]
		private GameObject WeaponGeometry;
		[HideInInspector]
		public WeaponIndicator indicator;

		protected virtual void Start() {
				shotsRemaining = magazineSize;
				if (indicator)
					indicator.Setup(name, magazineSize);
		}

		protected virtual void Update() {
			if (reloading) {
				if (Time.fixedTime > reloadEnd) {
					shotsRemaining = magazineSize;
					reloading = false;
					if (indicator) {
						indicator.SetAmmoText((int) shotsRemaining, magazineSize);
						indicator.SetReloadAmmount(1f);
					}
				} else {
					indicator.SetReloadAmmount(1f - (reloadEnd - Time.fixedTime) / reloadTime);
				}
			}
		}

		public virtual bool OnTap() {
			if (!reloading) {
				if (Time.fixedTime > nextShot) {
					nextShot = Time.fixedTime + coolDown;
					shotsRemaining -= ammoConsumptionTap;
					if (indicator) {
						indicator.SetAmmoText((int) shotsRemaining, magazineSize);
						indicator.SetReloadAmmount(shotsRemaining / magazineSize);
					}
					reloading = CheckReload();
					return true;
				}
			}
			return false;
		}

		public virtual bool OnHold() {
			if (!reloading) {
				shotsRemaining -= ammoConsumptionHold;
				if (indicator) {
					indicator.SetAmmoText((int) shotsRemaining, magazineSize);
					indicator.SetReloadAmmount(shotsRemaining / magazineSize);
				}
				reloading = CheckReload();
				if (reloading) {
					OnRelease();
					return false;
				}
				return true;
			}
			return false;
		}

		public virtual bool OnRelease() {
			return true;
		}

		private bool CheckReload() {
			if (shotsRemaining <= 0) {
				reloadEnd = Time.fixedTime + reloadTime;
				return true;
			}
			return false;
		}

		public virtual void Equip() {
			for(int i = 0; i < firingPoints.Length; i++) {
				GameObject.Instantiate(WeaponGeometry, firingPoints[i].position, firingPoints[i].rotation, firingPoints[i]);
			}
		}

		public virtual void Unequip() {
			for(int i = 0; i < firingPoints.Length; i++) {
				if (firingPoints[i].childCount > 0)
					Destroy(firingPoints[i].GetChild(0).gameObject);
			}

			Destroy(gameObject);
		}
	}
}
