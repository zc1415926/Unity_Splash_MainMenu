using UnityEngine;
using System.Collections;

public class Scene : MonoBehaviour {

	private GameObject obj;
	public GameObject Cube;
	public GameObject Sphere;
	
	void Start () {
	
		string what = PlayerPrefs.GetString ("what");

		if ("cube" == what) {
		
			obj = (GameObject)Instantiate(Cube);
		}
		else if("sphere" == what){

			obj = (GameObject)Instantiate(Sphere);
		}

		Debug.Log (what);
		PlayerPrefs.DeleteKey ("what");
		if (!PlayerPrefs.HasKey ("what")) {
		
			Debug.Log("there is no \"what\"");
		}
	}
	
	void OnGUI(){

		if (GUILayout.Button ("Back")) {
		
			Application.LoadLevel("Menu");
		}
	}
}