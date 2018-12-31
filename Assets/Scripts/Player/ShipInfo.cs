using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class ShipInfo : MonoBehaviour {

		public new string name;
		public Transform[] primaryGroupFirePoints;
		public WeaponType[] primaryAcceptedTypes;
		public Transform[] secondaryGroupFirePoints;
		public WeaponType[] secondaryAcceptedTypes;
		public Renderer recolorSurface;

		public float yawTorque;
		public float rollTorque;
		public float pitchTorque;
		public float baseThrustForce;
		public float brakeForce;
		public float thrustForce;

		public int health;

	}
}
