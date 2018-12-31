using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class WeaponSystems : MonoBehaviourPunCallbacks, IPunObservable {

		private bool primaryInput;
		private bool primaryHeld;
		private float primaryHoldTime;
		private bool primaryTapped;

		private bool secondaryInput;
		private bool secondaryHeld;
		private float secondaryHoldTime;

		public float holdTime;

		public WeaponGroup primary;
		public WeaponGroup secondary;

		public PlayerController player;

		void Awake() {
			object _primary;
			var _si = GetComponentInChildren<ShipInfo>();
			if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_PRIMARY_WEAPON, out _primary)) {
				string _primaryName = (string) _primary;
				var _weaponObj = (GameObject) Instantiate(Resources.Load("PrimaryWeapons/" + _primaryName, typeof(GameObject)), transform.position, transform.rotation, transform);
				var _weapon = _weaponObj.GetComponent<Weapon>();
				primary.Add(_weapon);
				_weapon.playerNumber = photonView.Owner.ActorNumber;
				_weapon.firingPoints = _si.primaryGroupFirePoints;
				_weapon.Equip();
				if (photonView.IsMine)
					_weapon.indicator = GetComponent<PlayerHUD>().primaryIndicator.GetComponent<WeaponIndicator>();
			}

			object _secondary;
			if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_SECONDARY_WEAPON, out _secondary)) {
				string _secondaryName = (string) _secondary;
				object weapon = Resources.Load("SecondaryWeapons/" + _secondaryName);
				if (weapon != null) {
					var _weaponObj = (GameObject) Instantiate((GameObject) weapon, transform.position, transform.rotation, transform);
					var _weapon = _weaponObj.GetComponent<Weapon>();
					secondary.Add(_weapon);
					_weapon.playerNumber = photonView.Owner.ActorNumber;
					_weapon.firingPoints = _si.secondaryGroupFirePoints;
					_weapon.Equip();
					if (photonView.IsMine)
						_weapon.indicator = GetComponent<PlayerHUD>().secondaryIndicator.GetComponent<WeaponIndicator>();
				}
			}

			primary.timeToHold = holdTime;
			secondary.timeToHold = holdTime;
		}

		void Update() {
			if (!PhotonNetwork.IsConnected || photonView.IsMine) {
				primaryInput = player.PrimaryFireInput;
				secondaryInput = player.SecondaryFireInput;
			}

			primary.HandleInput(primaryInput);
			secondary.HandleInput(secondaryInput);
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
			if (stream.IsWriting) {
				// We own this player: send the others our data
				stream.SendNext(primaryInput);
				stream.SendNext(secondaryInput);
			} else {
				// Network player: read the stream
				primaryInput = (bool)stream.ReceiveNext();
				secondaryInput = (bool)stream.ReceiveNext();
			}
		}

	}

	[System.Serializable]
	public class WeaponGroup {
		private bool tapped;
		private bool held;
		public List<Weapon> group;
		private float holdTime;
		public float timeToHold;

		public void HandleInput(bool input) {
			if (input) {
				if (!tapped) {
					tapped = true;
					for(int i = 0; i < group.Count; i++) {
						group[i].OnTap();
					}
					holdTime = timeToHold + Time.fixedTime;
				} else if (holdTime < Time.fixedTime) {
					held = true;
					for (int i = 0; i < group.Count; i++) {
						group[i].OnHold();
					}
				}
			} else {
				if (held) {
					for(int i = 0; i < group.Count; i++) {
						group[i].OnRelease();
					}
				}
				tapped = false;
				held = false;
			}
		}

		public void Add(Weapon weapon) {
			group.Add(weapon);
		}
	}
}
