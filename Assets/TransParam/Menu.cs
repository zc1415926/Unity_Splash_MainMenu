using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	void OnGUI(){

		if (GUILayout.Button ("Cube")) {
		
			PlayerPrefs.SetString("what", "cube");
			Application.LoadLevel("Scene");
		}
		else if(GUILayout.Button("Sphere"))	{

			PlayerPrefs.SetString("what", "sphere");
			Application.LoadLevel("Scene");
		}
	}
}