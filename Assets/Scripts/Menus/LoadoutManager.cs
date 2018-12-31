using System.Collections;
using System.Collections.Generic;

using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Com.RyanKoning.SeaOfStars {
	public class LoadoutManager {

		public Loadout[] GetAllLoadouts() {
			if (Directory.Exists(Application.persistentDataPath + "/Loadouts")) {
				var files = Directory.GetFiles(Application.persistentDataPath + "/Loadouts");
				var fileCount = files.Length;
				var loadouts = new Loadout[fileCount];
				for (int i = 0; i < fileCount; i++) {
					loadouts[i] = LoadLoadout(files[i]);
				}
				return loadouts;
			} else {
				Directory.CreateDirectory(Application.persistentDataPath + "/Loadouts");
				Debug.Log("Loadouts Directory Created!");
				return null;
			}
		}

		public void ChangeLoadoutName(Loadout loadout, string newName) {
			File.Delete(loadout.Path);
			loadout.name = newName;
			SaveLoadout(loadout);
		}

		public void SaveLoadout(Loadout loadout) {
			BinaryFormatter bf = new BinaryFormatter();
			string path = Application.persistentDataPath + "/Loadouts/" + loadout.name + ".dat";
			FileStream file = File.Create(path);
			loadout.Path = path;
			bf.Serialize(file, loadout);
			file.Close();
		}

		public void DeleteLoadout(Loadout loadout) {
			File.Delete(loadout.Path);
		}

		private Loadout LoadLoadout(string filepath) {
			if (File.Exists(filepath)) {
				FileStream file = File.Open(filepath, FileMode.OpenOrCreate);
				BinaryFormatter bf = new BinaryFormatter();
				Loadout loadout = (Loadout) bf.Deserialize(file);
				file.Close();
				return loadout;
			} else {
				return null;
			}
		}
	}
}
