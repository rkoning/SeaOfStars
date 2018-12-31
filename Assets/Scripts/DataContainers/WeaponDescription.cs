using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.RyanKoning.SeaOfStars {

	public enum WeaponType {
		Laser,
		Missile,
		Ballistic,
		Plasma,
		Utility
	}

	[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
	public class WeaponDescription : ScriptableObject {

		public new string name;
		public string prefabPath;
		public GameObject weaponProp;

		public WeaponType type;

		public int damage;
		public float range;
		public float cooldown;
		public float magazineSize;
		public int reloadTime;
	}
}
