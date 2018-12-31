using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class LoadoutEditorPanel : MonoBehaviour {

		[SerializeField]
		private CustomizationMenu customizationMenu;

		// Destroy the example ship if it exists when the editor exits.
		void OnDisable() {
			if (customizationMenu.shipAnchor.transform.childCount > 0) {
				foreach(Transform t in customizationMenu.shipAnchor.transform) {
					Destroy(t.gameObject);
				}
			}
		}
	}
}
