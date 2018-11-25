using UnityEngine;
using Photon.Pun;

namespace Com.RyanKoning.SeaOfStars {
	public class PlayerController : MonoBehaviourPunCallbacks {

		public static GameObject LocalPlayerInstance;

		private bool yawRight;
		private bool yawLeft;
		private int yawInput;
		public int YawInput { get { return yawInput; } }

		private bool brakeInput;
		public bool BrakeInput { get { return brakeInput; } }

		private float rollInput;
		public float RollInput { get { return rollInput; } }

		private float pitchInput;
		public float PitchInput { get { return pitchInput; } }

		private float throttleInput;
		public float ThrottleInput { get { return throttleInput; } }

		private bool primaryFireInput;
		public bool PrimaryFireInput { get { return primaryFireInput; } }

		private bool secondaryFireInput;
		public bool SecondaryFireInput { get { return secondaryFireInput; } }

		private bool menuInput;
		public bool MenuInput { get { return menuInput; } }

		private bool secondaryMenuInput;
		public bool SecondaryMenuInput { get { return secondaryMenuInput; } }

		private CameraController cam;

		private string name;
		public string Name { get { return name; } }

		private bool isLocal;
		public bool IsLocal {get { return isLocal; } }

		private int actorNumber;
		public int ActorNumber {get { return actorNumber; } }

		void Awake() {
			if (!photonView.IsMine && PhotonNetwork.IsConnected == true) {
				return;
			}

			if (PhotonNetwork.IsConnected) {
				name = photonView.Owner.NickName;
				actorNumber = photonView.Owner.ActorNumber;
			} else {
				name = "Player 1";
				actorNumber = 0;				
			}

			isLocal = true;
 			GetComponentInChildren<Camera>().enabled = true;
			PlayerController.LocalPlayerInstance = this.gameObject;
			cam = GetComponentInChildren<CameraController>();
			cam.Target = this.transform;
			cam.transform.SetParent(null);
		}

		void Update() {
			if (!photonView.IsMine && PhotonNetwork.IsConnected == true) {
				return;
			}
			ProcessInputs();
		}

		void ProcessInputs() {
			yawRight = Input.GetButton("YawRight");
			yawLeft = Input.GetButton("YawLeft");

			if (yawRight && yawLeft) {
				brakeInput = true;
			} else {
				brakeInput = false;
				yawInput = (yawRight ? 1 : (yawLeft ? -1 : 0));
			}
			rollInput = Input.GetAxis("Roll");
			pitchInput = Input.GetAxis("Pitch");
			throttleInput = Input.GetAxis("Throttle");

			cam.Boosting = throttleInput == 1;

			primaryFireInput = Input.GetButton("PrimaryFire");
			secondaryFireInput = Input.GetButton("SecondaryFire");

			menuInput = Input.GetButton("Menu");
			secondaryMenuInput = Input.GetButton("SecondaryMenu");
			// Debug.Log("Yaw: " + yawInput + " Roll: " + rollInput + " Pitch: " + pitchInput + " Throttle: " + throttleInput + " Brake: " + brakeInput);
		}
	}
}
