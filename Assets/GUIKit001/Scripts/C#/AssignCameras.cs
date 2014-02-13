using UnityEngine;
using System.Collections;

public class AssignCameras : MonoBehaviour {

	public Camera[] cams = new Camera[0];

	void Start () {
		if(StandardGUI.instance) StandardGUI.instance.SetupCameras(cams);
		else Debug.LogWarning("C# StandarGUI not found!");
	}
	
}
