using UnityEngine;
using System.Collections;

public class Sender : MonoBehaviour {
	
	void Start () {
		PlayerPrefs.SetString ("message", "hello!");
	}

	void OnGUI() {

		if (GUILayout.Button ("send!", GUILayout.Height (50))) {
		
			Application.LoadLevel("Receiver");
		}
	}
}