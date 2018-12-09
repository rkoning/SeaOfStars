using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class Laser : Weapon {

		public float duration;
		public float range;
		public LayerMask mask;

		private float durationEnd;
		private LineRenderer[] beams;
		public Material beamMaterial;
		public float beamThickness;

		void Start() {
			beams = new LineRenderer[firingPoints.Length];
			for (int i = 0; i < firingPoints.Length; i++) {
				beams[i] = firingPoints[i].gameObject.AddComponent<LineRenderer>();
				beams[i].material = beamMaterial;
				beams[i].widthMultiplier = beamThickness;
				beams[i].enabled = false;
			}
		}

		void Update() {
			if (Time.fixedTime > durationEnd) {
					isFiring = false;
			}
			if (isFiring) {
				for(int i = 0; i < firingPoints.Length; i++) {
					FireBeam(firingPoints[i], beams[i]);
				}
			} else {
				DisableBeams();
			}
		}

		public override bool OnTap() {
			if (base.OnTap()) {
				isFiring = true;
				durationEnd = Time.fixedTime + duration;
				return true;
			}
			return false;
		}

		protected void FireBeam(Transform origin, LineRenderer beam) {
			beam.enabled = true;
			RaycastHit hit;
			if (Physics.Raycast(origin.position, origin.forward, out hit, range, mask)) {
				beam.SetPositions( new Vector3[] {origin.position, hit.point} );
				var h = hit.collider.GetComponent<Health>();
				if (h != null) {
					h.TakeDamage(damage, playerNumber);
				}
			} else {
				beam.SetPositions( new Vector3[] {origin.position, origin.position + origin.forward * range} );
			}
		}

		protected void DisableBeams() {
			foreach(var beam in beams) {
				beam.enabled = false;
			}
		}
	}
}
