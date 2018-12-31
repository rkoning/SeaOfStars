using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.RyanKoning.SeaOfStars {
	public class WeaponIndicator : MonoBehaviour {

		public Text nameText;
		public Text ammoText;
		public Image reloadImage;

		public void Setup(string name, float maxAmmo) {
			nameText.text = name;
			ammoText.text = maxAmmo + "/" + maxAmmo;
			reloadImage.fillAmount = 1f;
		}

		public void SetReloadAmmount(float amount) {
			reloadImage.fillAmount = amount;
		}

		public void SetAmmoText(float currentAmmo, float maxAmmo) {
			ammoText.text = currentAmmo + "/" + maxAmmo;
		}
	}
}
