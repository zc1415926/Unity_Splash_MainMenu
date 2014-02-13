using UnityEngine;
using System.Collections;

public class QAPlatform : MonoBehaviour {


	//public FlashingController flashingController;

	//public HighlightableObject highlightableObj;



	public void AnswerIsRight(){

		((FlashingController)gameObject.GetComponent("FlashingController")).enabled = false;
		((HighlightableObject)gameObject.GetComponent("HighlightableObject")).enabled = false;
		//flashingController.enabled = false;
		//highlightableObj.enabled = false;

		iTween.MoveBy(gameObject, iTween.Hash("y", 5, 
		                                      "easeType", "easeInOutCirc", 
		                                     // "loopType", "none", 
		                                      "time", 1.0f,
		                                      "delay", 0.2f));

		//iTween.ColorTo(gameObject, new Color(1.0f, 1.0f, 1.0f, 0.0f),1.0f);

		//iTween.ColorTo(gameObject,new Color(1f,1f,1f, 0f),2f);
		iTween.ColorTo(gameObject, iTween.Hash("a", 0.0f, "oncomplete", "DestroyMyself", "time", 1, "delay", 1));

		//iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));

	}

	void AnswerIsWrong(){

	}

	void DestroyMyself(){
		Destroy(gameObject);
	}
}
