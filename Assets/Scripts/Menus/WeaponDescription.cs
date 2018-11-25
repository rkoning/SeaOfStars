using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.RyanKoning.SeaOfStars {

	public enum WeaponType {
		Laser,
		Missile,
		Ballistic
	}

	[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
	public class WeaponDescription : ScriptableObject {

		public new string name;
		public string prefabPath;

		public WeaponType type;
		public bool disjointed;

		public int damage;
		public float range;
		public float cooldown;
		public float magazineSize;
		public int reloadTime;
	}
}
