using UnityEngine;
using System.Collections;

public class Receiver : MonoBehaviour {

	void OnGUI(){

		if (GUILayout.Button ("receive!", GUILayout.Height (50))) {
		
			string message = PlayerPrefs.GetString("message");
			Debug.Log(message);

			PlayerPrefs.DeleteKey("message");
		}
	}
}
