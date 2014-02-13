using UnityEngine;
using System.Collections;

public class HitQuestion : MonoBehaviour {

	/*void OnControllerColliderHit(ControllerColliderHit hit) {
	
		if(hit.gameObject.tag == "QuestionPlatform"){

			Debug.Log("!!Hit!!");
		}
	}*/

	public GUISkin guiSkin;

	private bool isShowFunctionTip = false;
	private Rect rectFunctionTip;
	public int rectFunctionTipWidth = 100;
	public int rectFunctionTipHeight = 100;
	public string rectFunctionTipText = "Press F for Fun";

	private bool isPressedFKey = false;

	private Rect rectQuestionPage;
	public int rectQuestionPageWidth = 200;
	public int rectQuestionPageHeight = 200;
	//public string rectQuestionPageText = "*******************\n\nYou are in Fun World!\n\nDon't worry!\n\nPress \"Esc\" Key To Exit!";

	public MouseLook mouseLookX;
	public MouseLook mouseLookY;
	public CharacterMotor characterMotor;

	private Collider collider;

	void OnStart(){

		//custome style 中的style要用GetStyle方法才行
		//guiSkin.GetStyle("title").normal.background = guiSkin.button.normal.background;
	}

	void OnTriggerEnter(Collider col){
		collider = col;

		//这一句为在OnAwake和OnStart里Screen.widtht Screen.height都读不出数据
		rectFunctionTip = new Rect((Screen.width - rectFunctionTipWidth) / 2,
		                           (Screen.height - rectFunctionTipHeight) / 2,
		                           rectFunctionTipWidth,
		                           rectFunctionTipHeight);


		if("QuestionPlatform" == col.gameObject.tag){
			isShowFunctionTip = true;

			rectQuestionPage = new Rect((Screen.width - rectQuestionPageWidth) / 2,
			                            (Screen.height - rectQuestionPageHeight) / 2,
			                            rectQuestionPageWidth,
			                            rectQuestionPageHeight);
		}
	}
	void OnTriggerExit(Collider col) {

		isShowFunctionTip = false;
		//isPressedFKey = false;
	}

	void Update(){

		if(isShowFunctionTip && Input.GetKeyDown(KeyCode.F)){

			isPressedFKey = true;
		}

		if(isPressedFKey){

			mouseLookX.enabled = false;
			mouseLookY.enabled = false;
			characterMotor.canControl = false;
		}

		/*if(isPressedFKey && Input.GetKey(KeyCode.Escape)){

			mouseLookX.enabled = true;
			mouseLookY.enabled = true;
			characterMotor.canControl = true;

			isShowFunctionTip = true;
			isPressedFKey = false;
		}*/
	}

	//OnGUI其实是在不停的按其中的代码刷新屏幕，像OpenGL
	void OnGUI(){

		if(isShowFunctionTip){

			//Debug.Log("rectFunctionTip" + rectFunctionTip);
			GUI.Label(rectFunctionTip, rectFunctionTipText, guiSkin.GetStyle("SmallLabel"));
		}
		if(isPressedFKey){

			isShowFunctionTip = false;
			//GUI.Label(rectQuestionPage, rectQuestionPageText, guiSkin.box);

			GUILayout.BeginArea(rectQuestionPage, guiSkin.window);


				GUILayout.BeginVertical();
				//GUILayout.Space(5);
				//GUILayout.FlexibleSpace();

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("请回答以下问题", guiSkin.GetStyle("Title"));
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("",guiSkin.GetStyle("xButton"))){
						CloseWindow();
					}
					GUILayout.EndHorizontal();

				GUILayout.FlexibleSpace();

					GUILayout.BeginVertical();
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("中国封建王朝最后一个皇帝是谁？", guiSkin.GetStyle("TextQuestion"));
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();

					GUILayout.FlexibleSpace();

						GUILayout.BeginHorizontal();
						if(GUILayout.Button("李世民", guiSkin.button)){

			
						}
						GUILayout.FlexibleSpace();//GUILayout.Space(10);
						if(GUILayout.Button("爱新觉罗·溥仪", guiSkin.button)){

							((QAPlatform)(collider.gameObject.GetComponent("QAPlatform"))).AnswerIsRight();
							/*mouseLookX.enabled = true;
							mouseLookY.enabled = true;
							characterMotor.canControl = true;
							
							isShowFunctionTip = true;
							isPressedFKey = false;*/

				collider.gameObject.GetComponent("");
							CloseWindow();
						}
						//GUILayout.Space(10);
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("嬴政", guiSkin.button)){

						}
						


						GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.EndArea();


		}

	}

	void CloseWindow(){
		mouseLookX.enabled = true;
		mouseLookY.enabled = true;
		characterMotor.canControl = true;
		
		isShowFunctionTip = true;
		isPressedFKey = false;
	}
}
