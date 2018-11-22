using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.Cegorach.SeaOfStars {
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

		[Header("Weapons")]
		public Weapon[] primaryGroup;

		private Vector3 networkedPosition;
		private Quaternion networkedRotation;
		private Vector3 networkedSpeed;

		private float lastNetworkDataReceivedTime;

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

				if (p.PrimaryFireInput) {
					for (int i = 0; i < primaryGroup.Length; i++) {
						primaryGroup[i].Fire();
					}
				}
			}
		}
	}
}
