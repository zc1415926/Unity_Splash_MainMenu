using UnityEngine;
using System.Collections;

public class GUIDebugWindow : MonoBehaviour {

	public StandardGUI stdGUI;
	public GUISkin mySkin;
	
	public bool flushPlayerPrefs = false; // enable to do PlayerPrefs.DeleteAll();
	
	
	public bool showDebugWindow = true; // Show Debug Window?
	private float prevRotationToggle = 1.0f;
	public float debugRotationToggle = 1.0f;
	public float debugFreezeTimeToggle = 1.0f;
	public float currentBGOpacity = 0.0f;
	public float prevBGMaxOpacity = 0.8f;
	
	private Rect debugWindowRect = new Rect (10f,10f,577f,280f);
	
	public string[] backgroundPaths = new string[5] {"GUI/Backgrounds/background01", "GUI/Backgrounds/background02", "GUI/Backgrounds/background03", "GUI/Backgrounds/background04", "GUI/Backgrounds/background05"};
	public int currentBackground= 0;
	
	private float ang;
	
	public int guiDepth = 10;
	
	
	void Awake() {
	
		if(flushPlayerPrefs) PlayerPrefs.DeleteAll();
		
		GameObject tempGO = GameObject.Find("GUI");
		stdGUI = (StandardGUI) tempGO.GetComponent("StandardGUI");
		mySkin = stdGUI.mySkin;	
	
			
	}
	
	
	
	void OnGUI () {
	
		// DEBUG WINDOW
		
		GUI.depth = guiDepth;
		GUI.skin = mySkin;
		
		if(showDebugWindow) {
			debugWindowRect = GUI.Window (666, debugWindowRect, DebugWindow, "DEBUG CONTROLS");
			
		}
		
		
	}
	
	
	void ChangeBackground() {
		
		currentBackground++;
		if(currentBackground >= backgroundPaths.Length) currentBackground = 0;
		stdGUI.backgroundImg = (Texture2D) Resources.Load(backgroundPaths[currentBackground], typeof(Texture2D));
		
		
	}
	
	
	
	
	public void DebugWindow (int windowID) {
		Vector2 pos = new Vector2(33f, 63f);
		
		if(GUI.Button(new Rect(530f, 24f, 19f, 19f), GUIContent.none, GUI.skin.GetStyle("xButton"))) showDebugWindow = false;
		
		debugRotationToggle = BlackishGUI.OnOffSwitchWithLabel(new Rect(pos.x, pos.y, 255f, 41f), debugRotationToggle, "Rotation", mySkin.GetStyle("onOffSlider"), mySkin.GetStyle("onOffSliderThumb"), mySkin.GetStyle("SliderLabel"));
		if(Screen.width < 640) GUI.enabled = false;
			if(stdGUI.scaleFactor < 1.5f) {
				if(GUI.Button(new Rect(pos.x + 260f, pos.y, 140f, 42f), "Retina on")) stdGUI.scaleFactor = 2.0f;	
			} else {
				if(GUI.Button(new Rect(pos.x + 260f, pos.y, 140f, 42f), "Retina off")) stdGUI.scaleFactor = 1.0f;
			}
		GUI.enabled = true;
		if(GUI.Button(new Rect(pos.x + 405f, pos.y, 110f, 42f), "Restart")) {
			Time.timeScale = 1.0f;
			stdGUI.currentFaderOpacity = 1.0f;
			for(int i = 0; i < stdGUI.matScale.Length; i++) stdGUI.matScale[i] = Vector3.one;
			for(int i = 0; i < stdGUI.guiOpacity.Length; i++) stdGUI.guiOpacity[i] = 0.0f;
			stdGUI.matPos[0] = new Vector3(-1.5f, 0.0f, 0.0f);
			for(int i = 1; i < stdGUI.matPos.Length; i++) stdGUI.matPos[i] = new Vector3(1.5f, 0.0f, 0.0f);
			
			StartCoroutine(stdGUI.Start());
			showDebugWindow = false;	
		}
	
		stdGUI.offsetAngle = BlackishGUI.HorizontalSliderWithLabel(new Rect(pos.x, pos.y+48f, 400f, 42f), 135.0f, stdGUI.offsetAngle, -180.0f, 180.0f, "Angle");
		stdGUI.backgroundMaxOpacity = BlackishGUI.HorizontalSliderWithLabel(new Rect(pos.x, pos.y+96f, 400f, 42f), 135.0f, stdGUI.backgroundMaxOpacity, "Background");
		if(stdGUI.backgroundMaxOpacity != prevBGMaxOpacity) {
			prevBGMaxOpacity = stdGUI.backgroundMaxOpacity;
			//stdGUI.currentBGOpacity = stdGUI.backgroundMaxOpacity;	
		}
	
		if(stdGUI.offsetAngle == 0.0f) GUI.enabled = false;
		if(GUI.Button(new Rect(pos.x + 405f, pos.y+48f, 110f, 42f), "Reset")) {
			stdGUI.offsetAngle = 0.0f;
		}
		GUI.enabled = true;
		if(GUI.Button(new Rect(pos.x + 405f, pos.y+96f, 110f, 42f), "Change")) {
			ChangeBackground();	
		}
		GUI.Label(new Rect(pos.x, pos.y + 144f, 515f, 42f), "Orientation: " + stdGUI.currentDeviceOrientation);
		
		GUI.DragWindow();
		
	}
	
	
		
	void Update () {
	
		if(Input.GetKeyUp("tab")) {
			showDebugWindow = !showDebugWindow;
		}
		
		if(Input.touchCount > 2) showDebugWindow = true;
		
		
		if(prevRotationToggle != debugRotationToggle) {
			prevRotationToggle = debugRotationToggle;
			if(debugRotationToggle > 0.5f) {
				stdGUI.allowOrientationLandscapeLeft = true;
				stdGUI.allowOrientationLandscapeRight = true;
				stdGUI.allowOrientationPortrait = true;
				stdGUI.allowOrientationPortraitUpsideDown = true;
			} else {
				stdGUI.allowOrientationLandscapeLeft = false;
				stdGUI.allowOrientationLandscapeRight = false;
				stdGUI.allowOrientationPortrait = false;
				stdGUI.allowOrientationPortraitUpsideDown = false;			
			}	
		}
	
	
	
	}
	
}
