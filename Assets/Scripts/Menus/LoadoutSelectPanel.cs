using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class LoadoutSelectPanel : MonoBehaviour {

		[SerializeField]
		private CustomizationMenu c;

		void Start() {
			c.PopulateLoadoutSelect();
		}

		void OnEnable() {
			Debug.Log("OnEnable");
			c.PopulateLoadoutSelect();
		}
	}
}
