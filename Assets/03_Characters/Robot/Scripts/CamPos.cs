using UnityEngine;
using System.Collections;

public class CamPos : MonoBehaviour {


	// Use this for initialization
	void Start () {
		((ThirdPersonCamera)GameObject.Find ("Main Camera").GetComponent("ThirdPersonCamera")).standardPos = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
