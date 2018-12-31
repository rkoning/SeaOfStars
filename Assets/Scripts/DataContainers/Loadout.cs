using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	[System.Serializable]
	public class Loadout {
		public string name;
		public string ship;
		public string primary;
		public string secondary;
		private string path;
		public string Path {
			get { return path; }
			set { path = value;}
		}

		public Loadout(string name, string ship, string primary, string secondary) {
			this.name = name;
			this.ship = ship;
			this.primary = primary;
			this.secondary = secondary;
		}

		public bool IsComplete() {
			return name != null && ship != null && primary != null && secondary != null;
		}
	}
}
