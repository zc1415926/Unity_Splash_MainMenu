using UnityEngine;
using System.Collections;

public class ZC_PreNext : MonoBehaviour {

	public SwipeControl swipeCtrl;

	void OnGUI(){

		GUI.matrix = swipeCtrl.matrix;

		if(swipeCtrl.currentValue == 0){
			GUI.enabled = false;
		}

		if(GUI.Button(new Rect(-100, 95, 80, 30), "Previous")){
			swipeCtrl.currentValue--;
		}

		GUI.enabled = true;

		if(swipeCtrl.currentValue == swipeCtrl.maxValue){
			GUI.enabled = false;
		}

		if(GUI.Button(new Rect(20, 95, 80, 30), "Next")){
			swipeCtrl.currentValue++;
		}

		GUI.enabled = true;	
	}
}
