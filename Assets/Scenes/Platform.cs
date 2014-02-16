using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	public GameObject Prefab01;
	public GameObject Prefab02;
	//public GameObject Sphere;
	private GameObject player;

	//public Avatar[] avatars = new Avatar[0];

	// Use this for initialization
	void Start () {
	
	
		string character = PlayerPrefs.GetString ("Character");

		switch(character)
		{
		case "Cube":
			player = (GameObject)Instantiate(Prefab01);
			player.transform.parent = transform;

			break;

		case "Sphere":
			player = (GameObject)Instantiate(Prefab02);
			player.transform.parent = transform;

			break;

		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
