using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.RyanKoning.SeaOfStars {
	public class CustomizationMenu : MonoBehaviour {

		[SerializeField]
		private SlidePanel[] optionPanels;
		public Transform shipAnchor;
		[SerializeField]
		private string[] ships;
		[SerializeField]
		private WeaponDescription[] primaryWeapons;
		private Weapon primary;
		[SerializeField]
		private WeaponDescription[] secondaryWeapons;
		private Weapon secondary;
		private ShipInfo currentShip;
		private Loadout currentLoadout;
		[SerializeField]
		private GameObject itemPrefab;

		private LoadoutManager loadoutManager;

		[SerializeField]
		private Transform loadoutSelectContent;
		[SerializeField]
		private Text loadoutTitle;

		[SerializeField]
		private Sprite warningSprite;

		void Start() {
			loadoutManager = new LoadoutManager();
		}

		public void CreateNewLoadout() {
			GetComponent<MainMenu>().OpenModalWith("Create New Loadout", "Create", true, CreateNewLoadout);
		}

		public void CreateNewLoadout(string loadoutName) {
				InitializeShipList(optionPanels[0].content);
				currentLoadout = new Loadout(loadoutName, null, null, null);
				loadoutManager.SaveLoadout(currentLoadout);
				EditLoadout(currentLoadout);
		}

		public void RenameLoadout() {
			GetComponent<MainMenu>().OpenModalWith("Rename Loadout", "Rename", true, RenameLoadout);
		}

		public void RenameLoadout(string loadoutName) {
			loadoutManager.ChangeLoadoutName(currentLoadout, loadoutName);
			this.loadoutTitle.text = loadoutName;
		}

		public void DeleteLoadout() {
			GetComponent<MainMenu>().OpenModalWith("Are you sure you want to delete: " + currentLoadout.name + "?", "Confirm", false, DeleteCurrentLoadout);
		}

		public void DeleteCurrentLoadout(string empty) {
			loadoutManager.DeleteLoadout(currentLoadout);
			GetComponent<MainMenu>().SetActivePanel("Loadout Select Panel");
		}

		public void SaveLoadout() {
			loadoutManager.SaveLoadout(currentLoadout);
		}

		public void EditLoadout(Loadout loadout) {
			currentLoadout = loadout;
			ResetEditMenu();
		}

		public void ResetEditMenu() {
			loadoutTitle.text = currentLoadout.name;
			Destroy(currentShip);
			InitializeShipList(optionPanels[0].content);
			if (currentLoadout.ship != null) {
				ChangeShip(currentLoadout.ship, false);
				ChangeWeapon("PrimaryWeapons", currentLoadout.primary);
				if (currentShip.secondaryAcceptedTypes.Length < 1) {
					currentLoadout.secondary = "";
				} else {
					ChangeWeapon("SecondaryWeapons", currentLoadout.secondary);
				}
			}
		}

		public void TogglePanel(int index) {
			if (optionPanels[index].open)
				StartCoroutine(optionPanels[index].Close());
			else
				StartCoroutine(optionPanels[index].Open());

			for (int i = index - 1; i > 0; i--) {
				StartCoroutine(optionPanels[i].Close());
			}
			for (int i = index + 1; i < optionPanels.Length; i++) {
				StartCoroutine(optionPanels[i].Open());
			}
		}

		public void ChangeShip(string shipName, bool clearWeapons) {
			if (shipName == null)
				return;
			if (currentShip != null) {
				Destroy(currentShip.gameObject);
			}

			// create the new ship and refresh the weapon lists
			currentShip = GameObject.Instantiate((GameObject) Resources.Load("Ships/" + shipName), shipAnchor.position, shipAnchor.rotation, shipAnchor).GetComponent<ShipInfo>();
			InitializeWeaponList("PrimaryWeapons", currentShip.primaryAcceptedTypes, primaryWeapons, optionPanels[1].content);
			InitializeWeaponList("SecondaryWeapons", currentShip.secondaryAcceptedTypes, secondaryWeapons, optionPanels[2].content);

			// remove primary and secondary weapons, and clear them from the loadout
			if (primary != null) {
				primary.Unequip();
				primary = null;
				if (clearWeapons) {
					currentLoadout.primary = null;
				}
			}
			if (secondary != null) {
				secondary.Unequip();
				secondary = null;
				if (clearWeapons) {
				  currentLoadout.secondary = null;
			  }
			}
			currentLoadout.ship = shipName;
		}

		public void ChangeWeapon(string prefix, string weaponName) {
			if (weaponName == null)
				return;
			Debug.Log(prefix + "/" + weaponName);
			var _weaponObj = GameObject.Instantiate((GameObject) Resources.Load(prefix + "/" + weaponName), shipAnchor.position, shipAnchor.rotation, shipAnchor);
			var _weapon = _weaponObj.GetComponent<Weapon>();
			if (prefix == "PrimaryWeapons") {
				_weapon.firingPoints = currentShip.primaryGroupFirePoints;
				if (primary) {
					primary.Unequip();
				}
				primary = _weapon;
				currentLoadout.primary = weaponName;
			} else {
				_weapon.firingPoints = currentShip.secondaryGroupFirePoints;
				if (secondary)
					secondary.Unequip();
				secondary = _weapon;
				currentLoadout.secondary = weaponName;
			}
			_weapon.Equip();
			// _weapon.enabled = false;
		}

		private void InitializeShipList(Transform listContent) {
			for (int i = 0; i < ships.Length; i++) {
				string shipName = ships[i];
				var btn = CreateButton(listContent, new Vector2(0, -110 * i), shipName);
				btn.onClick.AddListener(delegate { ChangeShip(shipName, true); } );
			}
		}

		private void InitializeWeaponList(string prefix, WeaponType[] allowedTypes, WeaponDescription[] weaponList, Transform listContent) {
			for (int i = 0; i < weaponList.Length; i++) {
				var weaponDesc = weaponList[i];
				var btn = CreateButton(listContent, new Vector2(0, -110 * i), weaponDesc.name);
				btn.onClick.AddListener(delegate { ChangeWeapon(prefix, weaponDesc.prefabPath); } );
				if (Array.IndexOf(allowedTypes, weaponDesc.type) == -1) {
					btn.interactable = false;
				}
			}
		}

		private Button CreateButton(Transform parent, Vector2 offset, string title, bool warning = false) {
			var item = GameObject.Instantiate(itemPrefab, offset, Quaternion.identity, null);
			item.transform.SetParent(parent, false);
			item.GetComponentInChildren<Text>().text = title;
			if (warning) {
				item.transform.GetChild(1).GetComponent<Image>().sprite = warningSprite;
			}
			return item.transform.GetComponentInChildren<Button>();
		}

		public void PopulateLoadoutSelect() {
			foreach (Transform t in loadoutSelectContent) {
				Destroy(t.gameObject);
			}
			Loadout[] allLoadouts = loadoutManager.GetAllLoadouts();
			if (allLoadouts != null) {
				for(int i = 0; i < allLoadouts.Length; i++) {
					var _thisLoadout = allLoadouts[i];
					var _loadoutBtn = CreateButton(loadoutSelectContent, new Vector2(0, i * -110), _thisLoadout.name, !_thisLoadout.IsComplete());
					_loadoutBtn.onClick.AddListener(delegate { EditLoadout(_thisLoadout); } );
					_loadoutBtn.onClick.AddListener(delegate { GetComponent<MainMenu>().SetActivePanel("Loadout Editor Panel"); } );
				}
			}
		}
	}

	[System.Serializable]
	public class SlidePanel {

		private float slideTime = 0.1f;
		private Vector2 openPosition = new Vector2(420, 0);
		public RectTransform panel;
		public RectTransform content;
		private float startTime;
		public bool open = false;

		public IEnumerator Toggle() {
			Debug.Log("Toggle");
			Vector2 point = open ? Vector2.zero : openPosition;
			Vector2 origin = open ? openPosition : Vector2.zero;
			float step = slideTime * Time.fixedDeltaTime;
			float t = 0;
			while (t < slideTime + step) {
				t += step;
				panel.anchoredPosition = Vector2.Lerp(origin, point, t);
				yield return new WaitForFixedUpdate();
			}
			open = !open;
		}

		public IEnumerator Open() {
			if (open)
				yield break;
			Vector2 point = openPosition;
			Vector2 origin = Vector2.zero;
			float t = 0;
			while (t < 1) {
				t += Time.fixedDeltaTime / slideTime;
				panel.anchoredPosition = Vector2.Lerp(origin, point, t);
				yield return new WaitForFixedUpdate();
			}
			open = true;
		}

		public IEnumerator Close() {
			if (!open)
				yield break;
			Vector2 point = Vector2.zero;
			Vector2 origin = openPosition;
			float t = 0;
			while (t < 1) {
				t += Time.fixedDeltaTime / slideTime;
				panel.anchoredPosition = Vector2.Lerp(origin, point, t);
				yield return new WaitForFixedUpdate();
			}
			open = false;
		}
	}
}
