using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class CameraController : MonoBehaviour {

		private Transform target;
		public Transform Target { set { target = value; } }

		public Vector3 offset;

		public float smoothTime;

		private bool boosting;
		public bool Boosting { set { boosting = value; } }

		private Vector3 velocity;
		private Camera cam;

		public float minFOV;
		public float maxFOV;
		private float currentFOV;
		public float FOVChangeTime;

		[SerializeField]
		private ParticleSystem speedLines;

		public float rotationSpeed;

		void Start() {
			currentFOV = minFOV;
			cam = GetComponent<Camera>();
		}

		void FixedUpdate() {
			if (target) {
				transform.position = Vector3.SmoothDamp(
					transform.position,
					target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x,
					ref velocity,
					smoothTime
				);

				transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.fixedDeltaTime * rotationSpeed);

				if (boosting) {
					speedLines.Play();
					currentFOV = Mathf.Lerp(currentFOV, maxFOV, Time.fixedDeltaTime / FOVChangeTime);
					cam.fieldOfView = currentFOV;
				} else {
					speedLines.Stop();
					currentFOV = Mathf.Lerp(currentFOV, minFOV, Time.fixedDeltaTime / FOVChangeTime);
					cam.fieldOfView = currentFOV;
				}
			}
		}
	}
}
