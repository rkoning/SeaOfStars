using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	[CreateAssetMenu(fileName = "NewStage", menuName = "Stage")]
	public class Stage : ScriptableObject {

			public new string name;
			public string scenePath;
			public GameMode[] gameModes;

	}
}
