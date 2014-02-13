using UnityEngine;
using System.Collections;

public class LoadNextLevel : MonoBehaviour {


	private bool done = false;
	private Color col;
	
	
	void Start () {
		if(!StandardGUI.instance) Destroy(this);
	}
	
	void OnGUI () {
		if(!done) {
			GUI.matrix = StandardGUI.instance.guiMatrix;
			GUI.skin = StandardGUI.instance.mySkin;
			
			//Hide this gui if menu is up
			col = GUI.color;
			col.a = 1f - StandardGUI.instance.currentBGOpacity;
			GUI.color = col;
			
			if(col.a > 0f) {
				if(GUI.Button(StandardGUI.instance.ScaleRectBottomRight(new Rect(-210, -52, 200, 42)), "Next Level")) {
					done = true;
					StartCoroutine(InitiateLoading());
				}
			}
		}
		
	}
	
	IEnumerator InitiateLoading() {
		LevelStatus.Complete(0); //Mark Level 1 (id 0) as complete
		LevelStatus.Unlock(1); //Unlock Level 2 (id 1)
		transform.parent = GameObject.FindWithTag("GUI").transform; //to keep this script alive through the level-change parent it to the GUI 
		yield return StartCoroutine(StandardGUI.instance.LoadLevel("Level2")); //wait for loading to complete
		Destroy(gameObject); //now destroy this manually
	}
}
