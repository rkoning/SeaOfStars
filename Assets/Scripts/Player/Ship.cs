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
		private ShipInfo info;

		void Awake() {
			if (PhotonNetwork.IsConnected) {
				object _selectedShip;
				if (photonView.Owner.CustomProperties.TryGetValue(SeaOfStarsGame.PLAYER_SELECTED_SHIP, out _selectedShip)) {
					string _shipName = (string) _selectedShip;
					var _hull = (GameObject) Instantiate(Resources.Load("Ships/" + _shipName, typeof(GameObject)), transform.position, transform.rotation, transform);
					info = _hull.GetComponent<ShipInfo>();
					GetComponent<Health>().recolorSurface = info.recolorSurface;
					GetComponent<Health>().shipBody = _hull;
				}
			} else {
				string _shipName = "Duo";
				var _hull = (GameObject) Instantiate(Resources.Load("Ships/" + _shipName, typeof(GameObject)), transform.position, transform.rotation, transform);
				info = _hull.GetComponent<ShipInfo>();
				GetComponent<Health>().recolorSurface = info.recolorSurface;
				GetComponent<Health>().shipBody = _hull;
			}

			yawTorque = info.yawTorque;
			rollTorque = info.rollTorque;
			pitchTorque = info.pitchTorque;
			baseThrustForce = info.baseThrustForce;
			brakeForce = info.brakeForce;
			thrustForce = info.thrustForce;
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
			}
		}
	}
}
