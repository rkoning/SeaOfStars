using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class PlayerHUD : MonoBehaviourPunCallbacks {

		private PlayerController player;
		private PlayerHealth health;
		public GameObject healthBarPrefab;
		private GameObject healthBar;
		public GameObject weaponIndicatorPrefab;
		[HideInInspector]
		public GameObject primaryIndicator;
		[HideInInspector]
		public GameObject secondaryIndicator;

		private GameObject menu;
		public float menuInputDelay = 0.4f;
		private float nextMenuInput;

		public Vector3 screenOffset = new Vector3(0, 30, 0);

		void Awake() {
			health = GetComponent<PlayerHealth>();
			player = GetComponent<PlayerController>();
			var _canvas = FindObjectOfType<Canvas>();
			healthBar = Instantiate(healthBarPrefab);

			menu = _canvas.transform.GetChild(0).gameObject;

			healthBar.transform.SetParent(_canvas.transform);

			health.healthBar = healthBar.GetComponent<Slider>();

			if (player.photonView.IsMine || !PhotonNetwork.IsConnected) {
				healthBar.GetComponentInChildren<Text>().enabled = false;
				primaryIndicator = GameObject.Instantiate(weaponIndicatorPrefab);
				primaryIndicator.transform.SetParent(_canvas.transform, false);
				secondaryIndicator = GameObject.Instantiate(weaponIndicatorPrefab);
				secondaryIndicator.transform.SetParent(_canvas.transform, false);
				secondaryIndicator.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 30);
			} else {
				Destroy(healthBar);
				// healthBar.GetComponentInChildren<Text>().text = photonView.Owner.NickName;
			}
		}

		void LateUpdate() {
			if (player.photonView.IsMine || !PhotonNetwork.IsConnected) {
				if (Time.fixedTime > nextMenuInput) {
					if (player.MenuInput) {
						nextMenuInput = Time.fixedTime + menuInputDelay;
						ToggleMenu();
					} else if(player.SecondaryMenuInput) {
						nextMenuInput = Time.fixedTime + menuInputDelay;
						Announcer.Instance.ToggleScoreBoard();
					}
				}
			} else {
				// healthBar.transform.position = Camera.main.WorldToScreenPoint (player.transform.position) + screenOffset;
			}
		}

		void ToggleMenu() {
			if (menu.activeInHierarchy) {
				menu.SetActive(false);
			} else {
				menu.SetActive(true);
			}
		}

		void OnDestroy() {
			Destroy(healthBar);
		}
	}
}
