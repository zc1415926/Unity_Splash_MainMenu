using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	public GameObject Cube;
	public GameObject Sphere;
	private GameObject player;

	// Use this for initialization
	void Start () {
	
	
		string character = PlayerPrefs.GetString ("Character");

		switch(character)
		{
		case "Cube":
			player = (GameObject)Instantiate(Cube);
			player.transform.parent = transform;

			break;

		case "Sphere":
			player = (GameObject)Instantiate(Sphere);
			player.transform.parent = transform;
			break;

		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
