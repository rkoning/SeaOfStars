using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class Ship : MonoBehaviourPunCallbacks {

		private PlayerController p;
		private Rigidbody rb;

  	[Header("Physics Forces")]
		public float yawTorque;
		public float rollTorque;
		public float pitchTorque;
		public float baseThrustForce;
		public float brakeForce;
		public float thrustForce;

		// [Header("Weapons")]
		// public Weapon[] primaryGroup;

		// private Vector3 networkedPosition;
		// private Quaternion networkedRotation;
		// private Vector3 networkedSpeed;

		void Awake() {
			object _selectedShip;
			if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_SHIP, out _selectedShip)) {
				string _shipName = (string) _selectedShip;
				var _hull = (GameObject) Instantiate(Resources.Load(_shipName, typeof(GameObject)), transform.position, transform.rotation, transform);
				var _si = _hull.GetComponent<ShipInfo>();

				GetComponent<Health>().recolorSurface = _si.recolorSurface;
				GetComponent<Health>().shipBody = _hull;
				object _primary;
				if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_PRIMARY_WEAPON, out _primary)) {
					string _primaryName = (string) _primary;
					var _weaponObj = (GameObject) Instantiate(Resources.Load("Weapons/" + _primaryName, typeof(GameObject)), transform.position, transform.rotation, transform);
					var _weapon = _weaponObj.GetComponent<Weapon>();
					GetComponent<WeaponSystems>().primaryGroup.Add(_weapon);
					_weapon.firingPoints = _si.primaryGroupFirePoints;
				}

				object _secondary;
				if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_PRIMARY_WEAPON, out _secondary)) {
					string _secondaryName = (string) _secondary;
					var _weaponObj = (GameObject) Instantiate(Resources.Load("Weapons/" + _secondaryName, typeof(GameObject)), transform.position, transform.rotation, transform);
					var _weapon = _weaponObj.GetComponent<Weapon>();
					GetComponent<WeaponSystems>().secondaryGroup.Add(_weapon);
					_weapon.firingPoints = _si.secondaryGroupFirePoints;

				}
			}
		}

		void Start() {
			p = GetComponent<PlayerController>();
			rb = GetComponent<Rigidbody>();
		}

		void FixedUpdate() {
			if (photonView.IsMine == true || PhotonNetwork.IsConnected == false) {

				if (p.BrakeInput)
					rb.AddForce((baseThrustForce - brakeForce) * transform.forward);
				else
					rb.AddForce((baseThrustForce + thrustForce * p.ThrottleInput) * transform.forward);

				rb.AddTorque(
					(yawTorque * p.YawInput * transform.up) +
					(rollTorque * p.RollInput * transform.forward) +
					(pitchTorque * p.PitchInput * transform.right)
				);

				// to be handled by weapon systems
				// if (p.PrimaryFireInput) {
				// 	for (int i = 0; i < primaryGroup.Length; i++) {
				// 		primaryGroup[i].Fire();
				// 	}
				// }
			}
		}
	}
}
