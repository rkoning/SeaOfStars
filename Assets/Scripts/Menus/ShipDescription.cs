using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {

	[CreateAssetMenu(fileName = "NewShip", menuName = "Ship")]
	public class ShipDescription : ScriptableObject {
		public new string name;
		public string prefabPath;
		public int primaryHardpoints;
		public int secondaryHardpoints;
		public WeaponDescription primary;
		public WeaponDescription secondary;
	}
}
