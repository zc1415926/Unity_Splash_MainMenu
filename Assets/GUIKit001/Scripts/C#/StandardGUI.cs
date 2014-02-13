///////////////////////////////////////////////
////          StandardGUI.cs v1.4          ////
////  copyright (c) 2010 by Markus Hofer   ////
////          for GameAssets.net           ////
///////////////////////////////////////////////
//zc1415926
/*
	    建立一个角色名称数组characterNames，顺序与场景名称数组对应。把场景名称数组各项都填成相同的一个名字
	即主场景的名字。再在角色名称数组里输入可选择的角色名称，在选择完点击Play按钮时，读取
	选择的序号对应数组里的名称，使用SetString方法传递到主场景中，在主场景中判定加载哪个角色。
	    另外，要在从主场景退出到菜单时，删除主场景，就要把场景中的所有GameObject放在同一个父
	级GameObject里，并将此父级GameObject的Tag设成“Level”。
        PlayerPrefs的数据存在注册表里，详见API DOC，LevelStatus就是存在PlayerPrefs里，此代码
    每次运行时读注册表，Inspector面板中的数据只有第一次运行的数据会写入注册表，以后就只读注册
    表了，所以再改Inspector面板中没没用作用了
	*/

using UnityEngine;
using System.Collections;

public class StandardGUI : MonoBehaviour {
	
	private static StandardGUI s_Instance = null;
	
	public string version = "100"; //Version of the game - 100 equals 1.0.0 - can be used internally to detect and react upon updates.
	private int launchCount = 0; //How often the game was launched
	
	public GUISkin mySkin; //global GUISkin
	
	public bool testRetinaDisplay = false; // enable this to use retina display //TODO: Make private?
	private string resourcePath100Percent = "GUI/100percent/"; //normal image resources will be loaded from this location
	private string resourcePath200Percent = "GUI/200percent/"; //image resources for retina display will be loaded from this location
	
	//Orientation and Camera Behaviour
	
	public bool allowOrientationLandscapeLeft = true;
	public bool allowOrientationLandscapeRight = true;
	public bool allowOrientationPortrait = true;
	public bool allowOrientationPortraitUpsideDown = true;
	
	public bool autoRotateKeyboard = false; //On the iPad, the black rotating frame is caused by the (out of view) keyboard rotating to the current orientation. So if you don't want the black frames and don't use the keyboard, set this to false!
	
	public bool rotateCamerasWithMatrixAngle = true; // Should Camera.main.transform.eulerAngles.z be set to matrixAngle? Makes the Camera rotate with the GUI...
	public bool alignCoordinateSystemToScreenOrientation = false; // When the screen orientation changes, change the default as well, so that the coordinate system rotates with the screen. (this disables rotation of the camera)
	public Camera[] cams = new Camera[0]; // Array of cameras that should be rotated - leave empty to use Camera.main only
	public bool adjustCameraFOV = true; // should the FOV be adjusted?
	private float[] camFOV = new float[0]; //will be filled with default FOV of all cameras in Awake
	
	private float LandscapeLeftAngleOffset = 0.0f; //Angle from current orientation to LandscapeLeft (will be filled depending on standard orientation)
	private float PortraitUpsideDownAngleOffset = 270.0f;
	private float LandscapeRightAngleOffset = 180.0f;
	private float PortraitAngleOffset = 90.0f;
	public DeviceOrientation currentDeviceOrientation; //Current Orientation - Don't change this, it's being set automatically! This is only public because the DebugMenu reads it...
	
	//Fade in / Fade out
	public Texture2D faderImg; //An image that will be used for "fade to black" - leave empty to use color
	public Color faderColor = Color.black; //Fade-to-Black color
	public float currentFaderOpacity = 1.0f; //No need to change this - it's used by the script to track the current opacity of the fader - you can make this private for the final build, it's only visible so the DebugWindow can access it
	
	//Background Image
	public Texture2D backgroundImg; //leave empty to fill background with one color (specify below!) - backgroundImg has to be square!
	public Color backgroundColor = new Color(0.15f, 0.15f, 0.15f); //if no backgroundImg is given this color is used to fill the screen
	public float backgroundSizeFactor = 0.18f; // should be 0.0 for guis that won't rotate. A value of 0.2 makes the background image 20% larger, so you don't see the edge when rotating. set this to about 0.18 if your gui needs to rotate
	public float backgroundMaxOpacity = 1.0f; // max opacity that the background will reach (and usually stay at while the menu is on)
	public float currentBGOpacity = 0.0f; //No need to change this - it's used by the script to track the current opacity of the background - always ranges from 0.0 to 1.0, multiplied by backgroundMaxOpacity
	
	//Freeze Time when the menu comes up?
	public bool freezeTime = true; // Set Time.timeScale to 0.0 when the menu comes up?
	public float freezeTimeTransitionDuration = 1.0f; // How many seconds should the transition take (time will slow down and stop instead of stopping instantly) - set to 0.0 to freeze/unfreeze instantly
	private bool freezeTimeBusy = false; // Tracks if freezing/unfreezing is in progress
	
	//MENU ASSETS
	private Texture2D mainMenuTitleImg;
	public string mainMenuTitleImgName = "TitleMainMenu";
	public string startButtonName = "Start Game";
	public string settingsButtonName = "Settings";
	public string infoButtonName = "Credits";
	
	//NewsBox
	public News news; //Script that handles downloading news - leave empty to hide the news box
	public string playerPrefsOnlineToggleName = "Toggle2"; //Enter name of the PlayerPrefs value that determines if the game should stay offline or online. Leave empty to ignore (Default Example: "Toggle2") - This will determine if news should be downloaded or not!
	
	//LEVEL SELECTION
	private Texture2D levelSelectionTitleImg;
	public string levelSelectionTitleImgName = "TitleLevelSelection";
	private Texture2D bracketImg; //The brackets seen on the side of the Level Selection and Info/Credits/Instructions screens (not visible on iPhone in Portrait and PortraitUpsideDown)
	public string bracketImgName = "Bracket";
	public bool skipLevelSelection = false; //Skip level selection and go straight to the game
	private Texture2D[] levelImg = new Texture2D[0]; //An image representing each level
	public string[] levelImgName = new string[4] {"Level001", "Level002", "Level003", "Level004"};
	private Texture2D levelImgOverlayLocked; //If a level is not available yet, overlay this image - leave empty to ignore
	public string levelImgOverlayLockedName = "LevelLocked";
	private Texture2D levelImgOverlayComplete; //If a level is complete, overlay this image - leave empty to ignore
	public string levelImgOverlayCompleteName = "LevelComplete";
	//zc1415926
	//public Texture
	//public Texture arrowPrevImg;
	//public string arrowPrevImgName = "ArrowLeft"; //Will only be loaded and shown on desktop/web. In addition you can provide xxx_hover if you'd like to add a mouseover state
	//public Texture arrowNextImg;
	//public string arrowNextImgName = "ArrowRight"; //Will only be loaded and shown on desktop/web. In addition you can provide xxx_hover if you'd like to add a mouseover state
	public int[] levelStatus = new int[0]; //status of all levels, 0 = available, 1 = complete, -1 = locked
	public string[] levelSceneNames = new string[0]; //sceneNames of all levels. levels will be loaded using Application.LoadLevelAdditiveAsync() if you have UNITY PRO and Application.LoadLevelAdditive if you don't have Unity PRO
	public bool loadLevelsAdditively = true; //load levels additively to keep the gui in the background and use the pause-button and the ingame menu. set to false if you just want to switch to the new level and do your own pause button and ingame menu, etc...
	public int selectedLevel = 0; //the currently selected level
	public GameObject currentlyLoadedLevelRoot; //the currently loaded level - At least the Camera should be in here, so it gets deleted when the new scene + camera is loaded
	public SwipeControl levelSelectionSwipeControl;

	//zc1415926
	public string[] characterNames = new string[0];

	//SETTINGS
	private Texture2D settingsTitleImg;
	public string settingsTitleImgName = "TitleSettings";
	//SETTINGS Menu
	public float musicLevel = 0.4f; //Default Music Level - Will be saved in PlayerPrefs as "MusicLevel"
	public float soundLevel = 0.8f; //Default Sound Level - Will be saved in PlayerPrefs as "SoundLevel"
	private float savedMusicLevel; //What's currently in PlayerPrefs - used to determine if it changed!
	private float savedSoundLevel; //What's currently in PlayerPrefs - used to determine if it changed!
	public string settingsToggle1Label = "Particles"; // Fill in Name of Toggle 1 - leave empty to hide toggle
	public string settingsToggle2Label = "Online"; // Fill in Name of Toggle 2 - leave empty to hide toggle
	public bool toggle1 = false; //Will be saved in PlayerPrefs as "Toggle1"
	public bool toggle2 = true; //Will be saved in PlayerPrefs as "Toggle2"
	public string toggle1TargetFunctionName = "ToggleParticles"; //When the toggle is changed a message of this name will be called on the target-gameObject specified below
	public GameObject toggle1Target; //Leave empty to use this GameObject
	public string toggle2TargetFunctionName = "DownloadNews"; //When the toggle is changed a message of this name will be called on the target-gameObject specified below
	public GameObject toggle2Target; //Leave empty to use this GameObject
	private bool savedToggle1; //What's currently in PlayerPrefs - used to determine if it changed!
	private bool savedToggle2; //What's currently in PlayerPrefs - used to determine if it changed!
	private float toggle1Val = 0.0f; //Needed to make the iOS-Style Toggle-slider work
	private float toggle2Val = 0.0f; //Needed to make the iOS-Style Toggle-slider work
	private float checkSettingsAtThisInterval = 0.3f; //how often should the script check if the settings were updated? should be less than the time it takes to close the settings menu, because it will only be done when the settings are up!
	private float lastTimeSettingsWereChecked = 0.0f; //when was the last time we checked if the settings are saved in the playerprefs - needed so it doesn't try to update playerprefs every frame when you drag a slider... 
	
	//INFO SCREEN
	private Texture2D infoTitleImg; //Title of Info/Credits/Instructions screen
	public string infoTitleImgName = "TitleCredits";
	private Texture2D[] infoImg = new Texture2D[0]; //Content images of the Info/Credits/Instructions screen
	public string[] infoImgName = new string[3] {"CreditsPage01", "CreditsPage02", "CreditsPage03"};
	public bool infoScrollsInsteadOfSwipe = true; //Should the credits/instructions/info screen scroll instead of using swipe?
	public SwipeControl infoSwipeControl;
	private float infoStartTimeOffset; //needed to make scrolling credits start with the first whenever you access it
	
	//InGame Pause Button
	public bool showInGamePauseButton = true; //Want to show the built-in pause button to show in the game?
	private Texture2D pauseButtonImg; //This is the Pause-Button
	public string pauseButtonImgName = "PauseButton";
	
	//Global Matrix Values
	public float matrixAngle; //This angle is shared by all gui elements!
	private float matrixAngleClamped; //matrixAngle, but translated into values only between 0 and 360
	public float offsetAngle = 0.0f; //Additional Angle value that will be added only to the GUI matrix - use this if you want the GUI at an angle, but keep the camera straight for each orientation!
	public Vector3 globalPos = Vector3.zero; //This Position is added to all matrices
	public Vector3 globalScale = Vector3.one; //This Scale is multiplied to all matrices
	
	//RETINA
	public float scaleFactor = 1.0f; // This scale effects the Rects of all gui elements - this is used to blow everything to 200% for Retina-Displays - it sort of works for all percentages, but there's a few drawbacks: Some elements in the GUISkin are made for fixed height and the fonts won't scale up, so it will look weird. //TODO: Make private!
	private float prevScaleFactor = 1.0f; // Used to remember the previous scaleFactor to check if it changed
	
	//MATRICES - Values and Matrices for all individual menus
	public Matrix4x4 guiMatrix = Matrix4x4.identity; //Basic GUI Matrix
	public Vector3[] matAngle = new Vector3[4];
	private Vector3 matpos; // don't worry about this, it's the current position used to calculate the current matrix. only up here so the var doesn't have to be created every frame again and again...
	private Quaternion quat = Quaternion.identity; //don't worry about this, it's the current quaternion used to calculate the current matrix...
	public Vector3[] matPos = new Vector3[4] {new Vector3(-1.5f, 0.0f, 0.0f), new Vector3(1.5f, 0.0f, 0.0f), new Vector3(1.5f, 0.0f, 0.0f), new Vector3(1.5f, 0.0f, 0.0f)}; //Starting position of the the GUI-screens. x at -1.5 means -150% of Screen.width to the left. These values will be animated to make the gui move!
	public Vector3[] matScale = new Vector3[4] {Vector3.one, Vector3.one, Vector3.one, Vector3.one}; //Not really used right now, but could be used to animate the scale of the gui
	public float[] guiOpacity = new float[4] {1.0f, 0.0f, 0.0f, 0.0f}; //Current opacity of each GUI section - used to animate opacity
	
	private bool orientationIsLandscape = true; //needed to check if the settings screen has to be made smaller for portrait orientation on small screens

	public int screenWidth; //Use this instead of Screen.width 
	public int screenHeight; //Use this instead of Screen.height 

	
	//Default position of Top and Bottom Bars
	private float topBarPos; // -65;
	private float bottomBarPos; //95;
	
	//CONTROL VALUES // You should never need to change these, but can make them visible for testing purposes
	private int savedScreenWidth;
	private int savedScreenHeight;
	private float aspectRatio;
	
	private bool mainMenuOn = false; //Is the main menu up?
	private bool pauseMenuOn = false; //Is the pause menu up?
	
	private bool barsBusy = false; //Are the bars busy? (= currently transitioning)
	private bool menuBusy = false; //is the menu busy? (= currently transitioning)
	private bool rotationBusy = false; //Is the screen currently rotating?
	private bool pauseButtonBusy = false; //Makes sure the pause-Button can't be pushed repeatetly
	
	private bool showPopupWindow = false; //Is the popupWindow Showing? Well, actually it's a modal dialog...
	private string popupString = ""; //The Question that the popup is asking
	private string popupMessageNo = ""; //Name of the function that should be called if user chooses No
	private string popupMessageYes = ""; //Name of the function that should be called if user chooses Yes
	
	
	//RECTS // These are needed out here so that the gui can scale for the iPhone4's retina display (and possibly for larger screens) - use scaleFactor to set the multiplication factor
	//If you change any of these, remember that the old values will be remembered by the Editor and you won't see any effect unless you change the values in the Inspector or set the var to private.
	//Main Menu Portrait
	private Rect mainMenuTitleRect = new Rect(-240, -160, 480, 92);
	private Rect mainMenuB1Rect = new Rect(-120, -52, 240, 40);
	private Rect mainMenuB2Rect = new Rect(-120, -5, 240, 40);
	private Rect mainMenuB3Rect = new Rect(-120, 42, 240, 40);
	private Rect mainMenuNewsRect = new Rect(-150, 105, 300, 77);
	//Main Menu Landscape
	private Rect mainMenuNewsLRect = new Rect(-230, 105, 460, 47);
	//Settings Portrait
	private Rect settingsTitleRect = new Rect(-160, -160, 480, 88);
	private Rect settingsSlider1Rect = new Rect(-140, -60, 280, 42);
	private Rect settingsSlider2Rect = new Rect(-140, -9, 280, 42);
	private Rect settingsToggle1Rect = new Rect(-140, 52, 135, 21);
	private Rect settingsToggle2Rect = new Rect(5, 52, 135, 21);
	private Rect settingsB1Rect = new Rect(-143, 112, 120, 40);
	private Rect settingsB2Rect = new Rect(23, 112, 120, 40);
	//Settings Landscape
	private Rect settingsTitleLRect = new Rect(-240, -160, 480, 88);
	private Rect settingsSlider1LRect = new Rect(-220, -60, 440, 42);
	private Rect settingsSlider2LRect = new Rect(-220, -9, 440, 42);
	private Rect settingsToggle1LRect = new Rect(-220, 42, 215, 41);
	private Rect settingsToggle2LRect = new Rect(5, 42, 215, 41);
	private Rect settingsB1LRect = new Rect(-223, 112, 120, 40);
	private Rect settingsB2LRect = new Rect(103, 112, 120, 40);
	//Credits Portrait
	private Rect infoTitleRect = new Rect(-160, -160, 480, 88);
	private Rect infoRect = new Rect(-160, -96, 320, 200);
	private Rect infoB1Rect = new Rect(-143, 112, 120, 40);
	//Credits Landscape
	private Rect infoTitleLRect = new Rect(-240, -160, 480, 88);
	private Rect infoB1LRect = new Rect(-223, 112, 120, 40);
	//LevelSelection Portrait
	private Rect levelSelectionImgRect = new Rect(-160, -72, 320, 145);
	private Rect levelSelectionTitleRect = new Rect(-160, -160, 480, 88);
	private Rect levelSelectionB1Rect = new Rect(-143, 112, 120, 40);
	private Rect levelSelectionB2Rect = new Rect(23, 112, 120, 40);
	//zc1415926
	private Rect levelPreviousBRect = new Rect(-203, -25, 50, 50);
	private Rect levelNextBRect = new Rect(153, -25, 50, 50);

	//LevelSelection Landscape
	private Rect levelSelectionTitleLRect = new Rect(-240, -160, 480, 88);
	private Rect levelSelectionB1LRect = new Rect(-223, 112, 120, 40);
	private Rect levelSelectionB2LRect = new Rect(103, 112, 120, 40);
	//zc1415926
	private Rect levelPreviousBLRect = new Rect(-203, -25, 50, 50);
	private Rect levelNextBLRect = new Rect(153, -25, 50, 50);

	//Brackets for Level Selection And Credits
	private Rect bracketLeftRect = new Rect(-240, -96, 80, 200);
	private Rect bracketRightRect = new Rect(240, -96, -80, 200);
	//Popup
	private Rect popupRect = new Rect(-160, -64, 320, 156);
	private Rect popupNoRect = new Rect(-125, 34, 120, 40);
	private Rect popupYesRect = new Rect(5, 34, 120, 40);
	//Pause Button
	//private var pauseButtonRect = new Rect(); //Removed this, Rect is now calculated on the fly
	
	private bool isUnityPro = false; //Unity PRO or not? Application.LoadLevelAdditiveAsync will be used instead of Application.LoadLevelAdditive! Will be asigned automatically in Awake.
	
	
	
	
	//SINGLETON MAGIC

    public static StandardGUI instance { 
        get {
            if (s_Instance == null) {
                s_Instance =  FindObjectOfType(typeof (StandardGUI)) as StandardGUI;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an StandardGUI object. You have to have exactly one StandardGUI in the scene.");
            }
            
            return s_Instance;
        }
    }	
	
	
	//CODE

	//POSITIONING AND SCALING HELP
	
	public Rect ScaleRect (Rect myRect) { return ScaleRect(myRect, scaleFactor);  }
	public Rect ScaleRect (Rect myRect, float factor) { //Helper function that scales a Rect
		myRect.x *= factor;
		myRect.y *= factor;
		myRect.width *= factor;
		myRect.height *= factor;	
		
		return myRect;
	}

	public Rect ScaleRectTopLeft(Rect myRect) {
		myRect.x = (-screenWidth *0.5f + myRect.x * scaleFactor);
		myRect.y = (-screenHeight * 0.5f + myRect.y * scaleFactor);
		myRect.width *= scaleFactor;
		myRect.height *= scaleFactor;
		return myRect;
	}

	public Rect ScaleRectTopRight(Rect myRect) {
		myRect.x = (screenWidth * 0.5f + myRect.x * scaleFactor);
		myRect.y = (-screenHeight * 0.5f + myRect.y * scaleFactor);
		myRect.width *= scaleFactor;
		myRect.height *= scaleFactor;
		return myRect;
	}
	
	public Rect ScaleRectBottomLeft(Rect myRect) {
		myRect.x = (-screenWidth * 0.5f + myRect.x * scaleFactor);
		myRect.y = (screenHeight * 0.5f + myRect.y * scaleFactor);
		myRect.width *= scaleFactor;
		myRect.height *= scaleFactor;
		return myRect;
	}
	
	public Rect ScaleRectBottomRight(Rect myRect) {
		myRect.x = (screenWidth * 0.5f + myRect.x * scaleFactor);
		myRect.y = (screenHeight * 0.5f + myRect.y * scaleFactor);
		myRect.width *= scaleFactor;
		myRect.height *= scaleFactor;
		return myRect;
	}		

	//POSITIONING HELP

	public Rect PositionRectTopLeft(Rect myRect) { //Same as ScaleRectTopLeft but without the scaling
		myRect.x = (-screenWidth * 0.5f + myRect.x);	
		myRect.y = (-screenHeight * 0.5f + myRect.y);
		return myRect;
	}

	public Rect PositionRectTopRight(Rect myRect) { //Same as ScaleRectTopRight but without the scaling
		myRect.x = (screenWidth * 0.5f + myRect.x);
		myRect.y = (-screenHeight * 0.5f + myRect.y);
		return myRect;
	}
	
	public Rect PositionRectBottomLeft(Rect myRect) { //Same as ScaleRectBottomLeft but without the scaling
		myRect.x = (-screenWidth * 0.5f + myRect.x);
		myRect.y = (screenHeight * 0.5f + myRect.y);
		return myRect;
	}
	
	public Rect PositionRectBottomRight(Rect myRect) { //Same as ScaleRectBottomRight but without the scaling
		myRect.x = (screenWidth * 0.5f + myRect.x);
		myRect.y = (screenHeight * 0.5f + myRect.y);
		return myRect;
	}	

	
	//AWAKE
	
	void Awake () {
		
		Debug.Log("StandardGUI Awake");	

		//Warnings
		if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
			if(alignCoordinateSystemToScreenOrientation) Debug.LogWarning("alignCoordinateSystemToScreenOrientation will work on the device, but might look like it's stuck in rotation in the Editor! (UnityRemote sends new Orientation, but the Editor can't actually set it and so it tries to rotate again and again and again...)", gameObject);	
			if(alignCoordinateSystemToScreenOrientation && !rotateCamerasWithMatrixAngle) Debug.LogWarning("You might want to enable rotateCamerasWithMatrixAngle since you have alignCoordinateSystemToScreenOrientation enabled!", gameObject);
			if(showInGamePauseButton && pauseButtonImgName == "") Debug.LogWarning("You're trying to show the ingame pause button, but haven't assigned an image for it! (pauseButtonImg)", gameObject);
			if(pauseButtonImgName != "" && !showInGamePauseButton) Debug.LogWarning("You have an image for the pause Button, but you're not using it! = waste of memory. You might want to either enable showInGamePauseButton, or remove the image from pauseButtonImg...", gameObject);
			if(adjustCameraFOV && ((allowOrientationLandscapeLeft && allowOrientationLandscapeRight && !allowOrientationPortrait && !allowOrientationPortraitUpsideDown) || (!allowOrientationLandscapeLeft && !allowOrientationLandscapeRight && allowOrientationPortrait && allowOrientationPortraitUpsideDown))) { adjustCameraFOV = false; Debug.LogWarning("adjustCameraFOV was disabled because you only allow portrait or landscape orientations!", gameObject); }
			if(levelImgName.Length != levelStatus.Length || levelImgName.Length != levelSceneNames.Length || levelStatus.Length != levelSceneNames.Length) Debug.LogWarning("levelImgNames, levelStatus and levelSceneNames have to have the same number of elements or the Level Selection could crash!", gameObject);
		}
		
		if(SystemInfo.supportsRenderTextures) isUnityPro = true; else isUnityPro = false;
		
	
		if(!faderImg) { //Create Fader Image from color if no image was given
			faderImg = new Texture2D(1, 1);
			Color[] fcols = faderImg.GetPixels();
			for(int g = 0; g < fcols.Length; ++g) {
				fcols[g] = faderColor;
			}
			faderImg.SetPixels(fcols);
			faderImg.Apply();			
		}
		
		
		if(!backgroundImg) { //if no background-texture is given, create one in the color given	
			backgroundImg = new Texture2D(1, 1);
			Color[] cols = backgroundImg.GetPixels();
			for(int i = 0; i < cols.Length; ++i) {
				cols[i] = backgroundColor;
			}
			backgroundImg.SetPixels(cols);
			backgroundImg.Apply();	
		} 
	
	
		//Setup relative device orientation angles depending on the default that was set in the Player Settings
		SetupDeviceOrientationAngles();
		
		//Remember Screen Resolution, etc.
		DetermineScreenDimensions();
		
		//Prepare Cameras
		SetupCameras();
		
		//Check supported rotations!
		#if UNITY_IPHONE || UNITY_ANDROID
		if(autoRotateKeyboard) {
			if(allowOrientationPortrait) iPhoneKeyboard.autorotateToPortrait = true; else iPhoneKeyboard.autorotateToPortrait = false;
			if(allowOrientationPortraitUpsideDown) iPhoneKeyboard.autorotateToPortraitUpsideDown = true; else iPhoneKeyboard.autorotateToPortraitUpsideDown = false;
			if(allowOrientationLandscapeLeft) iPhoneKeyboard.autorotateToLandscapeLeft = true; else iPhoneKeyboard.autorotateToLandscapeLeft = false;
			if(allowOrientationLandscapeRight) iPhoneKeyboard.autorotateToLandscapeRight = true; else iPhoneKeyboard.autorotateToLandscapeRight = false;		
		} else { //never rotate the keyboard = keep the black rotating frame from showing on the iPad
			iPhoneKeyboard.autorotateToPortrait = false;
			iPhoneKeyboard.autorotateToPortraitUpsideDown = false;
			iPhoneKeyboard.autorotateToLandscapeLeft = false;
			iPhoneKeyboard.autorotateToLandscapeRight = false;
		}
		#endif
		
		//Set PlayerPrefs if they don't exist or load if they do
		if(!PlayerPrefs.HasKey("MusicLevel")) { //If this one doesn't exist, none of the others should exist either, so set them from the values given in the inspector!
			PlayerPrefs.SetFloat("MusicLevel", musicLevel);
			PlayerPrefs.SetFloat("SoundLevel", soundLevel);	
			if(toggle1) { PlayerPrefs.SetInt("Toggle1", 1); toggle1Val = 1.0f; } else toggle1Val = 0.0f;
			if(toggle2) { PlayerPrefs.SetInt("Toggle2", 1); toggle2Val = 1.0f; } else toggle2Val = 0.0f;
		} else { //Values exist, so Load the saved Values into the right Variables!
			musicLevel = savedMusicLevel = PlayerPrefs.GetFloat("MusicLevel");
			soundLevel = savedSoundLevel = PlayerPrefs.GetFloat("SoundLevel");
			if(PlayerPrefs.HasKey("Toggle1")) { toggle1 = true; toggle1Val = 1.0f; savedToggle1 = true; } else { toggle1 = false; toggle1Val = 0.0f; savedToggle1 = false; }
			if(PlayerPrefs.HasKey("Toggle2")) { toggle2 = true; toggle2Val = 1.0f; savedToggle2 = true; } else { toggle2 = false; toggle2Val = 0.0f; savedToggle2 = false; }					
		}
		
		if(!toggle1Target) toggle1Target = gameObject;
		if(!toggle2Target) toggle2Target = gameObject;
		
		//Check if Retina-Display or huge size
		bool retinaDevice = false;
		#if UNITY_IPHONE || UNITY_ANDROID
		if(iPhoneSettings.generation == iPhoneGeneration.iPhone4 || Screen.width == 960 || Screen.width == 640 || Screen.width == 2048 || Screen.width == 1536) retinaDevice = true; //checking for screen-dimensions here, because they forgot about 4th Gen iPod Touch for Unity 3.0 it seems...
		#endif 
		if(retinaDevice || testRetinaDisplay) { 
			LoadResourcesAndScaleRects(resourcePath200Percent, 2.0f); //Replace GUISkin with higher resolution skin and replace images with higher res images if available!
		} else {
			LoadResourcesAndScaleRects(resourcePath100Percent, 1.0f);
		}
		
	
		//Detect if game was updated since last launch
		string lastVersion = "000";
		if(PlayerPrefs.HasKey("version")) {
			lastVersion = PlayerPrefs.GetString("version");
		} else { //key "version" doesn't exist in PlayerPrefs yet
			//First Launch ever, fill LevelStatusX in PlayerPrefs with the values set in the Inspector!
			//Debug.Log("!!!!!!!!");
			for(int s = 0; s < levelStatus.Length; s++) {

				//Debug.Log("!!!!!!!!"+levelStatus[s]);
				PlayerPrefs.SetInt("LevelStatus" + s, levelStatus[s]);	
			}			
		}
		if(lastVersion != version) {
			//Update happend since last time! remembering new version...
			PlayerPrefs.SetString("version", version);
			
			//This is where you could add stuff that should only happen once for the new version (like setting new default PlayerPrefs values, etc.)
			if(version == "101") { //But careful! If someone buys the game at version 1.2 (=120), this will never happen!
	
			}
			
		}	
	
	
		//Load Level Status information from PlayerPrefs
		LoadLevelStatuses();
	
	
		//Set up the SwipeControl that's used by the LevelSelection
		if(!skipLevelSelection) {
			if(levelSelectionSwipeControl.partWidth == 0) levelSelectionSwipeControl.partWidth = levelSelectionImgRect.width; //Only apply if not already set in the SwipeControl-Script's inspector 
			levelSelectionSwipeControl.SetMouseRect(new Rect(Screen.width * -0.5f, levelSelectionImgRect.y, Screen.width, levelSelectionImgRect.height));
			levelSelectionSwipeControl.CalculateEdgeRectsFromMouseRect(levelSelectionImgRect);
			levelSelectionSwipeControl.Setup();
			if(levelSelectionSwipeControl.maxValue == 0) levelSelectionSwipeControl.maxValue = levelStatus.Length - 1;
			//Find highest unlocked Level and set it to be the startValue
			int highestUnlockedLevel = 0;
			for(int h = 0; h < levelStatus.Length; h++) {
				if(levelStatus[h] >= 0) highestUnlockedLevel = h;
			}
			if(levelSelectionSwipeControl.startValue == 0) levelSelectionSwipeControl.startValue = highestUnlockedLevel;
		} else {
			levelSelectionSwipeControl.enabled = false;	
		}
		
		
		//Set up the SwipeControl that's used by the Info/Credits/Instructions screen
		if(infoSwipeControl.partWidth == 0) infoSwipeControl.partWidth = infoRect.width; //Only apply if not already set in the SwipeControl-Script's inspector 
		if(infoScrollsInsteadOfSwipe) { //Scroll
			infoSwipeControl.currentValue = -1;
			infoSwipeControl.maxValue = infoImg.Length - 1;
			infoSwipeControl.clickEdgeToSwitch = false;
			infoSwipeControl.allowInput = false;
		} else { //Swipe
			infoSwipeControl.maxValue = infoImg.Length - 1;
			infoSwipeControl.SetMouseRect(new Rect(Screen.width * -0.5f, infoRect.y, Screen.width, infoRect.height)); //assign the mouseRect - based on the image, but for the whole width of the screen
			infoSwipeControl.CalculateEdgeRectsFromMouseRect(infoRect);
			infoSwipeControl.allowInput = true;
		}
		infoSwipeControl.Setup();
	
	
		//LaunchCount (as the name suggests) logs how often the game has been launched
		launchCount = PlayerPrefs.GetInt("LaunchCount");
		if(Time.realtimeSinceStartup < 15.0f) {
			launchCount ++;
			PlayerPrefs.SetInt("LaunchCount", launchCount);
		}
	
			
	}
	
	
	
	public IEnumerator Start () {
	
		Debug.Log("Start");
		
		StartCoroutine(LoadLevelInBackground("MainMenuBG")); //MainMenuBackground
		ShowMenu();
		yield return StartCoroutine(Wait.WaitForSecRealtime(0.5f));
		StartCoroutine(FadeOut()); //Fades out the black image
		
	}



	public string GetCurrentResourcePath() {
		if(scaleFactor > 1.5f) return resourcePath200Percent;
		else return resourcePath100Percent;
	}

	
	
	public void SetupCameras(Camera[] camArr) {
	
		cams = camArr;
		
		//Remember default landscape FOVs for all Cams
		camFOV = new float[cams.Length];
		for(int j = 0; j < cams.Length; j++) {
			camFOV[j] = cams[j].fieldOfView;
			#if UNITY_IPHONE || UNITY_ANDROID
			if(Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) camFOV[j] *= 1.25f;
			#endif
		}	
		
		Debug.Log("New Cameras set up for this scene!");
		
	}
	public void SetupCameras() {
		//Fill cams array with Camera.main if empty (default)
		Camera[] cameraArr = new Camera[0];
		if(cams.Length < 1) {
			cameraArr = new Camera[1];
			cameraArr[0] = Camera.main;	
		}
		SetupCameras(cameraArr);	
	}
	
	
	public void LoadResourcesAndScaleRects(string basePath, float newScaleFactor) { //Fills all imageslots with new images from a given basePath in the Resources folder
	
		float factor = newScaleFactor / prevScaleFactor; Debug.Log("Scale GUI by factor " + factor);
		scaleFactor = newScaleFactor;
		prevScaleFactor = newScaleFactor;
				
		//Load new GUISkin
		Debug.Log("Looking for GUISkin " + basePath + "" + mySkin.name);
		mySkin = (GUISkin) Resources.Load(basePath + "" + mySkin.name, typeof(GUISkin));
		//mySkin = Resources.Load(resourcePath200Percent + mySkin.name + "_200percent", GUISkin);
	
		
		//Load Title Images
		mainMenuTitleImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + mainMenuTitleImgName, typeof(Texture2D));
		settingsTitleImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + settingsTitleImgName, typeof(Texture2D));
		infoTitleImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + infoTitleImgName, typeof(Texture2D));
		levelSelectionTitleImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelSelectionTitleImgName, typeof(Texture2D));
		bracketImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + bracketImgName, typeof(Texture2D));
		pauseButtonImg = (Texture2D) Resources.Load(basePath + "StandardGUI/" + pauseButtonImgName, typeof(Texture2D));
		
		//Load Credits Images
		infoImg = new Texture2D[infoImgName.Length];
		for(int i = 0; i < infoImg.Length; i++) {
			infoImg[i] = (Texture2D) Resources.Load(basePath + "StandardGUI/" + infoImgName[i], typeof(Texture2D));
		}
		
		//Load Level Images
		levelImg = new Texture2D[levelImgName.Length];
		for(int j = 0; j < levelImg.Length; j++) {
			levelImg[j] = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgName[j], typeof(Texture2D));
		}
		levelImgOverlayLocked = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgOverlayLockedName, typeof(Texture2D));	
		levelImgOverlayComplete = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgOverlayCompleteName, typeof(Texture2D));
		
		//Scale up Rects!
		mainMenuTitleRect = ScaleRect(mainMenuTitleRect, factor);
		mainMenuB1Rect = ScaleRect(mainMenuB1Rect, factor);
		mainMenuB2Rect = ScaleRect(mainMenuB2Rect, factor);
		mainMenuB3Rect = ScaleRect(mainMenuB3Rect, factor);	
		mainMenuNewsRect = ScaleRect(mainMenuNewsRect, factor);
		mainMenuNewsLRect = ScaleRect(mainMenuNewsLRect, factor);
	
		settingsTitleRect = ScaleRect(settingsTitleRect, factor);
		settingsSlider1Rect = ScaleRect(settingsSlider1Rect, factor);
		settingsSlider2Rect = ScaleRect(settingsSlider2Rect, factor);
		settingsToggle1Rect = ScaleRect(settingsToggle1Rect, factor);
		settingsToggle2Rect = ScaleRect(settingsToggle2Rect, factor);
		settingsB1Rect = ScaleRect(settingsB1Rect, factor);
		settingsB2Rect = ScaleRect(settingsB2Rect, factor);
		
		settingsTitleLRect = ScaleRect(settingsTitleLRect, factor);
		settingsSlider1LRect = ScaleRect(settingsSlider1LRect, factor);
		settingsSlider2LRect = ScaleRect(settingsSlider2LRect, factor);
		settingsToggle1LRect = ScaleRect(settingsToggle1LRect, factor);
		settingsToggle2LRect = ScaleRect(settingsToggle2LRect, factor);
		settingsB1LRect = ScaleRect(settingsB1LRect, factor);
		settingsB2LRect = ScaleRect(settingsB2LRect, factor);
	
		infoTitleRect = ScaleRect(infoTitleRect, factor);
		infoRect = ScaleRect(infoRect, factor);
		infoB1Rect = ScaleRect(infoB1Rect, factor);
		
		infoTitleLRect = ScaleRect(infoTitleLRect, factor);
		infoB1LRect = ScaleRect(infoB1LRect, factor);
	
		levelSelectionImgRect = ScaleRect(levelSelectionImgRect, factor);
		levelSelectionTitleRect = ScaleRect(levelSelectionTitleRect, factor);
		levelSelectionB1Rect = ScaleRect(levelSelectionB1Rect, factor);
		levelSelectionB2Rect = ScaleRect(levelSelectionB2Rect, factor);
		//zc1415926
		levelPreviousBRect = ScaleRect(levelPreviousBRect, factor);
		levelNextBRect = ScaleRect(levelNextBRect, factor);
		
		levelSelectionTitleLRect = ScaleRect(levelSelectionTitleLRect, factor);
		levelSelectionB1LRect = ScaleRect(levelSelectionB1LRect, factor);
		levelSelectionB2LRect = ScaleRect(levelSelectionB2LRect, factor);
		//zc1415926
		levelPreviousBLRect = ScaleRect(levelPreviousBLRect, factor);
		levelNextBLRect = ScaleRect(levelNextBLRect,factor);
		
		bracketLeftRect = ScaleRect(bracketLeftRect, factor);
		bracketRightRect = ScaleRect(bracketRightRect, factor);
		
		popupRect = ScaleRect(popupRect, factor);
		popupNoRect = ScaleRect(popupNoRect, factor);
		popupYesRect = ScaleRect(popupYesRect, factor);
		
	//	pauseButtonRect = ScaleRect(pauseButtonRect, factor);	
	
		
		//Setup SwipeControls
		//LevelSelection
		levelSelectionSwipeControl.partWidth = levelSelectionImgRect.width; 
		levelSelectionSwipeControl.SetMouseRect(new Rect(Screen.width * -0.5f, levelSelectionImgRect.y, Screen.width, levelSelectionImgRect.height));
		levelSelectionSwipeControl.CalculateEdgeRectsFromMouseRect(levelSelectionImgRect);
		levelSelectionSwipeControl.Setup();
		//Info/Credits/Instructions
		if(!infoScrollsInsteadOfSwipe) { //Swipe
			infoSwipeControl.partWidth = infoRect.width; 
			infoSwipeControl.maxValue = infoImg.Length - 1;
			infoSwipeControl.SetMouseRect(new Rect(Screen.width * -0.5f, infoRect.y, Screen.width, infoRect.height)); //assign the mouseRect - based on the image, but for the whole width of the screen
			infoSwipeControl.CalculateEdgeRectsFromMouseRect(infoRect);
			infoSwipeControl.Setup();
		}
		
		
		
	}
	

	void LoadLevelSelectionImages() {
		string basePath = GetCurrentResourcePath();
		
		//Load Level Images
		levelImg = new Texture2D[levelImgName.Length];
		for(int j = 0; j < levelImg.Length; j++) {
			levelImg[j] = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgName[j], typeof(Texture2D));
		}
		levelImgOverlayLocked = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgOverlayLockedName, typeof(Texture2D));	
		levelImgOverlayComplete = (Texture2D) Resources.Load(basePath + "StandardGUI/" + levelImgOverlayCompleteName, typeof(Texture2D));		
	}
	
	IEnumerator UnloadLevelSelectionImagesInSec (float sec) {
		yield return StartCoroutine(Wait.WaitForSecRealtime(sec));
		levelImg = new Texture2D[0];
	}	
	
	
	

	
	
	void OpenLink() {
		
		Application.OpenURL(news.newsTextTargetURL);
		
	}

	
	
	public void DetermineScreenDimensions() {
		if(Screen.width != savedScreenWidth || Screen.height != savedScreenHeight) {
			savedScreenWidth = Screen.width;
			savedScreenHeight = Screen.height;
			aspectRatio = Screen.width / Screen.height;

			screenWidth = Screen.width;
			screenHeight = Screen.height;
			
			StartCoroutine(UpdateInternalScreenDimensionsInSec(0f, currentDeviceOrientation));

		}
	}
	

	IEnumerator UpdateInternalScreenDimensionsInSec (float duration, DeviceOrientation orient) {

		float startTime = Time.realtimeSinceStartup;
		
		while(Time.realtimeSinceStartup - startTime < duration) { 
			yield return null;
		}

		#if UNITY_IPHONE || UNITY_ANDROID
		if((Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) && ((orient == DeviceOrientation.Portrait && allowOrientationPortrait) || (orient == DeviceOrientation.PortraitUpsideDown && allowOrientationPortraitUpsideDown))) {
			//Default Device Orientation is Landscape, but new assumed orientation is Portrait
			screenWidth = Screen.height;
			screenHeight = Screen.width;
		} else if((Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) && ((orient == DeviceOrientation.LandscapeLeft && allowOrientationLandscapeLeft) || (orient == DeviceOrientation.LandscapeRight && allowOrientationLandscapeRight))) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;	
		} else if((Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) && ((orient == DeviceOrientation.LandscapeLeft && allowOrientationLandscapeLeft) || (orient == DeviceOrientation.LandscapeRight && allowOrientationLandscapeRight))) {
			screenWidth = Screen.height;
			screenHeight = Screen.width;					
		} else if((Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) && ((orient == DeviceOrientation.Portrait && allowOrientationPortrait) || (orient == DeviceOrientation.PortraitUpsideDown && allowOrientationPortraitUpsideDown))) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;					
		}
		#endif		
		
	}

	
	public IEnumerator RotateScreenTo(float targetVal, float duration, DeviceOrientation orient) { //Softly Rotate the matrixAngle to a new orientation
	
		if((orient == DeviceOrientation.LandscapeLeft && allowOrientationLandscapeLeft) || (orient == DeviceOrientation.LandscapeRight && allowOrientationLandscapeRight) || (orient == DeviceOrientation.Portrait && allowOrientationPortrait) || (orient == DeviceOrientation.PortraitUpsideDown && allowOrientationPortraitUpsideDown)) {
		
			//Debug.Log("RotateScreenTo " + targetVal + ", gA: " + matrixAngle + ", gAC: " + matrixAngleClamped + " orient: " + orient);
	
			rotationBusy = true;
		
			if(adjustCameraFOV && rotateCamerasWithMatrixAngle) {
				for(int i = 0; i < cams.Length; i++) {
					float targetFOV = camFOV[i];
					if(orient == DeviceOrientation.Portrait || orient == DeviceOrientation.PortraitUpsideDown) targetFOV *= 0.8f;
					StartCoroutine(ChangeFOV(cams[i], targetFOV, duration)); //duration * 0.95
				}
			}
			
			
			StartCoroutine(UpdateInternalScreenDimensionsInSec(duration * 0.5f, orient)); //Change screenWidth and screenHeight in the middle of the rotation

			
			float currentAngle = matrixAngle;
			float startTime = Time.realtimeSinceStartup;
			float durationVal = 1f/duration;	
			
		
			while(Time.realtimeSinceStartup - startTime < duration) { 
				matrixAngle = Mathf.SmoothStep(currentAngle, targetVal, (Time.realtimeSinceStartup - startTime) * durationVal);
				yield return null;
			}
			
			matrixAngle = targetVal;	
	
			currentDeviceOrientation = orient;
			
			#if UNITY_IPHONE || UNITY_ANDROID
			if(alignCoordinateSystemToScreenOrientation) {
	
				ScreenOrientation newOrientation = ScreenOrientation.LandscapeLeft;
		
				if(orient == DeviceOrientation.LandscapeLeft) newOrientation = ScreenOrientation.LandscapeLeft;
				else if(orient == DeviceOrientation.Portrait) newOrientation = ScreenOrientation.Portrait;
				else if(orient == DeviceOrientation.LandscapeRight) newOrientation = ScreenOrientation.LandscapeRight;
				else if(orient == DeviceOrientation.PortraitUpsideDown) newOrientation = ScreenOrientation.PortraitUpsideDown;
				Debug.Log("new iPhoneSettings.screenOrientaiton: " + Screen.orientation);
				
				yield return new WaitForEndOfFrame (); //Wait until the end of the frame or it will give an ugly jitter
				
				matrixAngle = 0.0f;
				if(adjustCameraFOV) {
					for(int j = 0; j < cams.Length; j++) {
						var nextFOV = camFOV[j] * 0.8f;
						if(orient == DeviceOrientation.Portrait || orient == DeviceOrientation.PortraitUpsideDown) nextFOV = camFOV[j];
						cams[j].fieldOfView = nextFOV;
						Debug.Log("alignCoordinateSystem: new FOV: " + nextFOV);
					}
				}
				Screen.orientation = newOrientation;
				DetermineScreenDimensions();
				SetupDeviceOrientationAngles();
			
			}
			#endif
	
			rotationBusy = false;
		}
		
	}
	
	
	
	
	public IEnumerator ChangeFOV (Camera cam, float targetFOV, float duration) { //Softly fade FOV to a new setting
	
			float currentFOV = cam.fieldOfView;
	
			float startTime = Time.realtimeSinceStartup;
			float durationVal = 1f/duration;	
			
			Debug.Log(currentFOV + " >> " + targetFOV);
			
			while(Time.realtimeSinceStartup - startTime < duration) { 
				cam.fieldOfView = Mathf.SmoothStep(currentFOV, targetFOV, (Time.realtimeSinceStartup - startTime) * durationVal);
				yield return null;
			}
			
			cam.fieldOfView = targetFOV;
			
			Debug.Log("FOV: " + targetFOV);
		
	}
	
	
	public void FreezeTime() { StartCoroutine(FreezeTime(true)); }
	public IEnumerator FreezeTime(bool instantly) {
		if(freezeTime) {
			while(freezeTimeBusy) yield return null;
			freezeTimeBusy = true;
			float targetVal = 0.0f;
			if(Time.timeScale == 0.0f) targetVal = 1.0f;
			if(freezeTimeTransitionDuration > 0.0f || instantly) {
				float currentVal = Time.timeScale;
				float startTime = Time.realtimeSinceStartup;
				float durationVal = 1f/freezeTimeTransitionDuration;	
				while(Time.realtimeSinceStartup - startTime < freezeTimeTransitionDuration) { 
					Time.timeScale = Mathf.SmoothStep(currentVal, targetVal, (Time.realtimeSinceStartup - startTime) * durationVal);
					yield return null;
				}		
			}	
			Time.timeScale = targetVal;		
			freezeTimeBusy = false;	
		}	
	}
	
	
	public IEnumerator FadeIn() { yield return StartCoroutine(FadeInOut(1.0f, 1.0f)); }
	public IEnumerator  FadeOut() { yield return StartCoroutine(FadeInOut(0.0f, 1.0f)); }
	public IEnumerator  FadeInOut (float targetVal, float duration) {
		
		Debug.Log("Fade!");
		
		float currentOpacity = currentFaderOpacity;
		float startTime = Time.realtimeSinceStartup;
		float durationVal = 1f/duration;	
		
		while(Time.realtimeSinceStartup - startTime < duration) { 
			currentFaderOpacity = Mathf.SmoothStep(currentOpacity, targetVal, (Time.realtimeSinceStartup - startTime) * durationVal);
			yield return null;
		}
		
		currentFaderOpacity = targetVal;	
	
	}
	
	
	
	public IEnumerator  FadeBackground (float targetVal, float duration) {
		
		float currentOpacity = currentBGOpacity;
		float startTime = Time.realtimeSinceStartup;
		float durationVal = 1f/duration;	
		
		while(Time.realtimeSinceStartup - startTime < duration) { 
			currentBGOpacity = Mathf.SmoothStep(currentOpacity, targetVal, (Time.realtimeSinceStartup - startTime) * durationVal);
			yield return null;
		}
		
		currentBGOpacity = targetVal;		
		
	}
	
	
	public IEnumerator AnimateBars(float topTarget, float bottomTarget, float duration) { //Animate the top and bottom bars to these positions (relative to the center of the screen) in this much time
	
		while(barsBusy) yield return null;
		
		barsBusy = true;
		float currentTopBarPos = topBarPos;
		float currentBottomBarPos = bottomBarPos;
		float startTime = Time.realtimeSinceStartup;
		float durationVal = 1f/duration;
	
		while(Time.realtimeSinceStartup - startTime < duration) { 
			topBarPos = Mathf.SmoothStep(currentTopBarPos, topTarget, (Time.realtimeSinceStartup - startTime) * durationVal);
			bottomBarPos = Mathf.SmoothStep(currentBottomBarPos, bottomTarget, (Time.realtimeSinceStartup - startTime) * durationVal);
			yield return null;
		}
		
		topBarPos = topTarget;
		bottomBarPos = bottomTarget;
		
		barsBusy = false;
		
	}
	
	
	public IEnumerator AnimateMenu(int menuID, Vector3 targetPos, float targetOpacity, float duration) { //Move the menu with ID menuID to targetPos while changing opacity to targetOpacity in the amount of seconds given in duration
		
	//	Debug.Log("Animating Menu " + menuID);
	
		//AudioController.inst.PlaySound(2);
	
		menuBusy = true;
		
		Vector3 currentMenuPos = matPos[menuID];
		float currentOpacity = guiOpacity[menuID];
		float startTime = Time.realtimeSinceStartup;
		float durationVal = 1f/duration;
	
		while(Time.realtimeSinceStartup - startTime < duration + 0.1f) {
			matPos[menuID].x = Mathf.SmoothStep(currentMenuPos.x, targetPos.x, (Time.realtimeSinceStartup - startTime) * durationVal);
			matPos[menuID].y = Mathf.SmoothStep(currentMenuPos.y, targetPos.y, (Time.realtimeSinceStartup - startTime) * durationVal);
			matPos[menuID].z = Mathf.SmoothStep(currentMenuPos.z, targetPos.z, (Time.realtimeSinceStartup - startTime) * durationVal);	
			guiOpacity[menuID] = Mathf.SmoothStep(currentOpacity, targetOpacity, (Time.realtimeSinceStartup - startTime) * durationVal);
			yield return null;
		}
		
		matPos[menuID] = targetPos;
		guiOpacity[menuID] = targetOpacity;
		
		menuBusy = false;
		
	}
	
	
	void SetupDeviceOrientationAngles() {
		
		//Setup relative device orientation angles depending on the default set in the Player Settings (or the changed default if allignCoordinateSystemToScreen is enabled)
		#if UNITY_IPHONE || UNITY_ANDROID
		if(Screen.orientation == ScreenOrientation.LandscapeLeft) {
			currentDeviceOrientation = DeviceOrientation.LandscapeLeft;
			LandscapeLeftAngleOffset = 0.0f;
			PortraitUpsideDownAngleOffset = 270.0f;
			LandscapeRightAngleOffset = 180.0f;
			PortraitAngleOffset = 90.0f;
		} else if(Screen.orientation == ScreenOrientation.Portrait) {
			currentDeviceOrientation = DeviceOrientation.Portrait;
			LandscapeLeftAngleOffset = 270.0f;
			PortraitUpsideDownAngleOffset = 180.0f;
			LandscapeRightAngleOffset = 90.0f;
			PortraitAngleOffset = 0.0f;		
		} else if(Screen.orientation == ScreenOrientation.LandscapeRight) {
			currentDeviceOrientation = DeviceOrientation.LandscapeRight;
			LandscapeLeftAngleOffset = 180.0f;
			PortraitUpsideDownAngleOffset = 90.0f;
			LandscapeRightAngleOffset = 0.0f;
			PortraitAngleOffset = 270.0f;		
		} else if(Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			currentDeviceOrientation = DeviceOrientation.PortraitUpsideDown;
			LandscapeLeftAngleOffset = 90.0f;
			PortraitUpsideDownAngleOffset = 0.0f;
			LandscapeRightAngleOffset = 270.0f;
			PortraitAngleOffset = 180.0f;		
		}	
		#endif
		
	}
	
	
	public void ShowMenu () { StartCoroutine(ShowMenu(false)); }
	public IEnumerator ShowMenu (bool instantly) { //Default Animation and stuff that should happen when the menu comes up
		
		mainMenuOn = true;
		
		if(instantly) StartCoroutine(FreezeTime(true)); else StartCoroutine(FreezeTime(false));
		topBarPos = -Screen.height * 0.5f;
		bottomBarPos = Screen.height * 0.5f;
		
		yield return StartCoroutine(Wait.WaitForSecRealtime(0.5f));
		
		if(instantly) StartCoroutine(FadeBackground(1.0f, 0.0f)); else StartCoroutine(FadeBackground(1.0f, 0.5f));
		StartCoroutine(AnimateBars(-65, 95, 0.5f)); 
		StartCoroutine(AnimateMenu(0, new Vector3(0.0f, 0.0f, 0.0f), 1.0f, 0.5f));	
		
	}


	public void LoadLevelStatuses() {
		for(int l = 0; l < levelStatus.Length; l++) {
		
			
			//Debug.Log("!!!!!!!!"+levelStatus[l]);

		}
		//Load Level Status information from PlayerPrefs
		for(int l = 0; l < levelStatus.Length; l++) {
			levelStatus[l] = PlayerPrefs.GetInt("LevelStatus" + l);	

			//Debug.Log("!!!!!!!!"+("LevelStatus" + l));
			//Debug.Log("!!!!!!!!"+PlayerPrefs.GetInt("LevelStatus" + l));
		}
	}

	


	public IEnumerator LoadLevelAndCloseMenu(string level) {

		StartCoroutine(AnimateBars(-Screen.height * 1.0f, Screen.height * 1.0f, 0.8f)); 
		if(!loadLevelsAdditively) MusicController.inst.FadeOutMusic(0.9f);
		yield return StartCoroutine(FadeIn());
		SaveSettings();
		
		if(loadLevelsAdditively) { //Load Levels Additively
		
			cams = new Camera[0];

			Debug.Log("Destroying old Level");
			while(currentlyLoadedLevelRoot) {
				Destroy(currentlyLoadedLevelRoot);
				yield return null;
			}
			
			if(isUnityPro) { //if Unity Pro, use LoadLevelAdditiveAsync
				AsyncOperation async = Application.LoadLevelAdditiveAsync(level); //var async : AsyncOperation = Application.LoadLevelAdditiveAsync(level);
				Debug.Log("Loading... (Async)");
				while(!async.isDone) {
					Debug.Log("Loading... " + async.progress); 	//Async Progress doesn't work on the iPhone/iPad on the moment...
					yield return null;
				}
			} else { //Not Unity Pro, use LoadLevelAdditive
				Application.LoadLevelAdditive(level);	
				while(Application.isLoadingLevel) { 
					Debug.Log("Loading...");
					yield return null;
				}
			}
			
			Debug.Log("Looking for new Level");
			
			currentlyLoadedLevelRoot = GameObject.FindWithTag("Level");
			if(!currentlyLoadedLevelRoot) Debug.LogWarning("No Level found! The root GameObject of your Level has to be tagged with 'Level' or the GUI won't know what to delete when a new level is loaded!");
			StartCoroutine(CloseMenu());	
			StartCoroutine(FadeOut()); //yield return 
			
		} else { //Don't load levels additively
			PlayerPrefs.SetFloat("MatrixAngle", matrixAngle); //Put MatrixAngle into PlayerPrefs, so it can be read out in the new level!
			Time.timeScale = 1.0f;
			Application.LoadLevel(level);	
		}
		
	}


	public IEnumerator LoadLevelInBackground(string level) { //MainMenuBackground
		
		if(loadLevelsAdditively) { //Load Levels Additively
		
			cams = new Camera[0];
			
			Debug.Log("Destroying old Level");
			while(currentlyLoadedLevelRoot) {
				Destroy(currentlyLoadedLevelRoot);
				yield return null;
			}			
			
			if(isUnityPro) { //if Unity Pro, use LoadLevelAdditiveAsync
				AsyncOperation async = Application.LoadLevelAdditiveAsync(level); //var async : AsyncOperation = Application.LoadLevelAdditiveAsync(level);
				Debug.Log("Loading... (Async)");
				while(!async.isDone) {
					Debug.Log("Loading... " + async.progress); 	//Async Progress doesn't work on the iPhone/iPad at the moment (Unity bug)...
					yield return null;
				}
			} else { //Not Unity Pro, use LoadLevelAdditive
				Application.LoadLevelAdditive(level);	
				while(Application.isLoadingLevel) { 
					Debug.Log("Loading...");
					yield return null;
				}
			}
			
			Debug.Log("Looking for new Level");
			
			currentlyLoadedLevelRoot = GameObject.FindWithTag("Level");
			if(!currentlyLoadedLevelRoot) Debug.LogWarning("No Level found! The root GameObject of your Level has to be tagged with 'Level' or the GUI won't know what to delete when a new level is loaded!");
			
		} else { //Don't load levels additively
			PlayerPrefs.SetFloat("MatrixAngle", matrixAngle); //Put MatrixAngle into PlayerPrefs, so it can be read out in the new level!
			Time.timeScale = 1.0f;
			Application.LoadLevel(level);	
		}
		
	}
	
	
	public IEnumerator LoadLevel(string level) { //use this for loading levels outside of the menu

		if(!loadLevelsAdditively) MusicController.inst.FadeOutMusic(0.9f);
		yield return StartCoroutine(FadeIn());		

		yield return StartCoroutine(LoadLevelInBackground(level));

		yield return StartCoroutine(FadeOut());		

	}
	
	
	
	public IEnumerator CloseMenu () {
		FreezeTime();	
		StartCoroutine(FadeBackground(0, 0.5f));
		StartCoroutine(AnimateBars(-Screen.height * 1.0f, Screen.height * 1.0f, 0.8f)); 
		yield return StartCoroutine(Wait.WaitForSecRealtime(1.0f));
		mainMenuOn = false;
		pauseMenuOn = false;
		
		SaveSettings();
	}
	
	
	
	public IEnumerator ShowPauseMenu () {
		if(!pauseButtonBusy) {
			pauseButtonBusy = true;
			FreezeTime();
			StartCoroutine(FadeBackground(1.0f, 0.5f));
			StartCoroutine(AnimateBars(-80, 101, 0.5f)); 
			StartCoroutine(AnimateMenu(0, new Vector3(-1.5f, 0.0f, 0.0f), 0.0f, 0.5f));
			StartCoroutine(AnimateMenu(1, new Vector3(0.0f, 0.0f, 0.0f), 1.0f, 0.5f));  	
			pauseMenuOn = true;
			
			yield return StartCoroutine(Wait.WaitForSecRealtime(1.0f)); 
			pauseButtonBusy = false;
		}
	}
	
	
	public IEnumerator QuitToMainMenu() { //Called by the popup
		yield return StartCoroutine(FadeIn());
		LoadLevelStatuses(); //load updated levels statuses, in case it changed while you were in a level...
		yield return StartCoroutine(LoadLevelInBackground("MainMenuBG")); //MainMenuBackground
		StartCoroutine(FadeOut());
		StartCoroutine(AnimateBars(-65, 95, 0.5f)); 
		StartCoroutine(AnimateMenu(0, new Vector3(0.0f, 0.0f, 0.0f), 1.0f, 0.5f));
		StartCoroutine(AnimateMenu(1, new Vector3(1.5f, 0.0f, 0.0f), 0.0f, 0.5f));
		pauseMenuOn = false;  		
		mainMenuOn = true;		
	}
	
	
	public void SaveSettings () {
		if(musicLevel != savedMusicLevel) { 
			PlayerPrefs.SetFloat("MusicLevel", musicLevel); 
			savedMusicLevel = musicLevel; 
			// If something special has to happen when the toggle changes, insert it here:
			Debug.Log("MusicLevel updated!"); 
			MusicController.inst.Initialize(); //Re-initialize MusicController to let it know about the change
		}
		
		if(soundLevel != savedSoundLevel) { 
			PlayerPrefs.SetFloat("SoundLevel", soundLevel); 
			savedSoundLevel = soundLevel; 
			// If something special has to happen when the toggle changes, insert it here: 
			Debug.Log("SoundLevel updated!");
			AudioController.inst.Initialize(); //Re-initialize AudioController to let it know about the change
			AudioController.inst.PlaySound(0); //Play a short sound as feeback to the player - this happens in the frequency that is set in checkSettingsAtThisInterval for as long as the player keeps dragging the control
		}
	
		if(toggle1 != savedToggle1) { 
			if(toggle1) PlayerPrefs.SetInt("Toggle1", 1); 
			else PlayerPrefs.DeleteKey("Toggle1"); 
			savedToggle1 = toggle1; 
			if(playerPrefsOnlineToggleName == "Toggle1" && news) news.ToggleOnline(toggle1);
			if(toggle1TargetFunctionName != "") toggle1Target.SendMessage(toggle1TargetFunctionName, toggle1, SendMessageOptions.DontRequireReceiver);
			// If something special has to happen when the toggle changes, insert it here:
			Debug.Log("Toggle1 updated!"); 
		}
	
		if(toggle2 != savedToggle2) { 
			if(toggle2) PlayerPrefs.SetInt("Toggle2", 1); 
			else PlayerPrefs.DeleteKey("Toggle2"); 
			savedToggle2 = toggle2; 
			if(playerPrefsOnlineToggleName == "Toggle2" && news) news.ToggleOnline(toggle2);
			if(toggle2TargetFunctionName != "") toggle2Target.SendMessage(toggle2TargetFunctionName, toggle2, SendMessageOptions.DontRequireReceiver);
			// If something special has to happen when the toggle changes, insert it here: (In this example, the function DownloadNews(); is called to either go online and get the latest news, or display an offline message...)
			Debug.Log("Toggle2 updated!");
		}
		
		lastTimeSettingsWereChecked = Time.realtimeSinceStartup;
	}
	
	
	
	void OnGUI() {	
	
		//Assign GUISkin
		GUI.skin = mySkin;

		Color gCol = GUI.color;
		
		if(mainMenuOn || pauseMenuOn) {
		
			//MATRIX
			quat.eulerAngles = new Vector3(0.0f, 0.0f, matrixAngle + offsetAngle);
			matpos = new Vector3(Mathf.Round(Screen.width * (0.5f + globalPos.x)), Mathf.Round(Screen.height * (0.5f + globalPos.y)), globalPos.z); //set position to the center of the screen
			GUI.matrix = Matrix4x4.TRS(matpos, quat, globalScale);		
			guiMatrix = GUI.matrix;	
			
			
			
			
			//BACKGROUND IMG
			
			if(backgroundImg) {
		
				gCol = GUI.color;
				gCol.a = currentBGOpacity * backgroundMaxOpacity;
				GUI.color = gCol;
				
				if(Screen.width > Screen.height) {
					GUI.DrawTexture(new Rect((-0.5f - backgroundSizeFactor) * Screen.width, (-0.5f - backgroundSizeFactor) * Screen.width, Screen.width * (1f + backgroundSizeFactor * 2f), Screen.width * (1f + backgroundSizeFactor * 2f)), backgroundImg); 
				} else {
					GUI.DrawTexture(new Rect((-0.5f - backgroundSizeFactor) * Screen.height, (-0.5f - backgroundSizeFactor) * Screen.height, Screen.height * (1f + backgroundSizeFactor * 2f), Screen.height * (1f + backgroundSizeFactor * 2f)), backgroundImg); 
				}
				
				gCol.a = 1.0f;
				GUI.color = gCol;
			}
			
			
	
			
			//BG BARS
			//Top Bottom Bar Elements
			GUI.Box(new Rect(-Screen.width * 1.0f, -Screen.height * 1.0f, Screen.width * 2.0f, Screen.height*1.0f + topBarPos * scaleFactor), GUIContent.none);
			GUI.Box(new Rect(-Screen.width * 1.0f, bottomBarPos * scaleFactor, Screen.width * 2.0f, Screen.height*1.0f), GUIContent.none);	
		
			
			if(showPopupWindow) GUI.enabled = false; //Disable all other GUI if a popupWindow is up!
		
		
		
		
			
			//MAIN MENU - ID 0
			
			if(guiOpacity[0] > 0.0f && new Rect(-1f, -1f, 2f, 2f).Contains(matPos[0])) {
				
				gCol = GUI.color;
				gCol.a = guiOpacity[0];
				GUI.color = gCol;
				
				quat.eulerAngles = matAngle[0];
				matpos = new Vector3(Mathf.Round(Screen.width * matPos[0].x), Mathf.Round(Screen.height * matPos[0].y), matPos[0].z); //set position to the center of the screen
				GUI.matrix = guiMatrix * Matrix4x4.TRS(matpos, quat, Vector3.Scale(matScale[0], globalScale));	
		
				GUI.DrawTexture(mainMenuTitleRect, mainMenuTitleImg);
			
				GUI.backgroundColor = Color.yellow; //Colored Play-Button to highlight this default option to press!
				//GUI.backgroundColor = GUI.contentColor = new Color(1.0, 1.0, Mathf.Sin(Time.realtimeSinceStartup * 3)*1.5-0.5); //Enable to make the Start-Button Pulse
				#if UNITY_IPHONE || UNITY_ANDROID
					GUI.SetNextControlName("start");
				#endif
				if(GUI.Button(mainMenuB1Rect, startButtonName)) { //Start Button
					AudioController.inst.PlaySound(1);
					if(skipLevelSelection) { //Start game, skip level selection
						StartCoroutine(AnimateMenu(0, new Vector3(1f, 0.0f, 0f), 0.0f, 0.5f)); 
						StartCoroutine(CloseMenu());
					} else { //Go to Level Selection
						LoadLevelSelectionImages();
						StartCoroutine(AnimateBars(-101f, 101f, 0.5f));
						StartCoroutine(AnimateMenu(0, new Vector3(-1.5f, 0.0f, 0f), 0.0f, 0.5f));
						StartCoroutine(AnimateMenu(3, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));				
					}
				} 
				#if UNITY_IPHONE || UNITY_ANDROID			
					GUI.FocusControl("start");
				#endif
				GUI.backgroundColor = GUI.contentColor = Color.white;
				
				if(GUI.Button(mainMenuB2Rect, settingsButtonName)) { 
					AudioController.inst.PlaySound(1);
					StartCoroutine(AnimateBars(-80, 101, 0.5f)); 
					StartCoroutine(AnimateMenu(0, new Vector3(-1.5f, 0.0f, 0f), 0.0f, 0.5f));
					StartCoroutine(AnimateMenu(1, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));  
				}
				
				if(GUI.Button(mainMenuB3Rect, infoButtonName)) {
					AudioController.inst.PlaySound(1);
					StartCoroutine(AnimateBars(-101, 101, 0.5f));
					StartCoroutine(AnimateMenu(0, new Vector3(-1.5f, 0.0f, 0f), 0.0f, 0.5f));
					StartCoroutine(AnimateMenu(2, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));
					infoStartTimeOffset = Time.realtimeSinceStartup - 2.5f; //Makes it start on the first page
				}
				
				Rect mainMenuNewsTempRect;
				if(!orientationIsLandscape) mainMenuNewsTempRect = mainMenuNewsRect; //Portrait
				else mainMenuNewsTempRect = mainMenuNewsLRect; //Landscape
				
				//NEWS-Box
				if(news) {
					if(GUI.Button(mainMenuNewsTempRect, news.newsText, GUI.skin.GetStyle("NewsBox"))) {
						AudioController.inst.PlaySound(1);
						if(news.newsTextTargetURL != "") { //if there's a link, ask if player wants to visit:
							showPopupWindow = true; //this causes all gui to be disabled and for the popup to show - look further down at the end of OnGUI...
							popupString = "在浏览器中打开链接？";
							popupMessageYes = "OpenLink"; //This function will be called if the user chooses YES in the popup!
						}
					}
				}
				
			}
		
		
		
		
		
		
			//SETTINGS - ID 1
		
			if(guiOpacity[1] > 0.0f && new Rect(-1f, -1f, 2f, 2f).Contains(matPos[1])) {
			
				gCol = GUI.color;
				gCol.a = guiOpacity[1];
				GUI.color = gCol;
			
				quat.eulerAngles = matAngle[1];
				matpos = new Vector3(Mathf.Round(Screen.width * matPos[1].x), Mathf.Round(Screen.height * matPos[1].y), matPos[1].z); //set position to the center of the screen
				GUI.matrix = guiMatrix * Matrix4x4.TRS(matpos, quat, Vector3.Scale(matScale[1], globalScale));	
			
				Rect settingsTitleTempRect;
				Rect settingsSlider1TempRect;
				Rect settingsSlider2TempRect;
				Rect settingsB1TempRect;
				Rect settingsB2TempRect;
			
				if(!orientationIsLandscape) { //For Portrait
					settingsTitleTempRect = settingsTitleRect;
					settingsSlider1TempRect = settingsSlider1Rect;
					settingsSlider2TempRect = settingsSlider2Rect;
					settingsB1TempRect = settingsB1Rect;
					settingsB2TempRect = settingsB2Rect;
				} else { //For Landscape
					settingsTitleTempRect = settingsTitleLRect;
					settingsSlider1TempRect = settingsSlider1LRect;
					settingsSlider2TempRect = settingsSlider2LRect;
					settingsB1TempRect = settingsB1LRect;
					settingsB2TempRect = settingsB2LRect;			
				}
			
				
				GUI.DrawTexture(settingsTitleTempRect, settingsTitleImg);
			
				musicLevel = GUI.HorizontalSlider(settingsSlider1TempRect, musicLevel, 0.0f, 1.0f, mySkin.GetStyle("musicSlider"), mySkin.GetStyle("musicSliderThumb"));
				soundLevel = GUI.HorizontalSlider(settingsSlider2TempRect, soundLevel, 0.0f, 1.0f, mySkin.GetStyle("musicSlider"), mySkin.GetStyle("soundSliderThumb"));
	
				if(!orientationIsLandscape) { //Toggles are different for portrait and landscape
					if(settingsToggle1Label != "") {
						toggle1 = GUI.Toggle(settingsToggle1Rect, toggle1, settingsToggle1Label);
					}
					if(settingsToggle2Label != "") {
						toggle2 = GUI.Toggle(settingsToggle2Rect, toggle2, settingsToggle2Label);
					}
				} else { //Landscape
					if(settingsToggle1Label != "") {
						toggle1Val = BlackishGUI.OnOffSwitchWithLabel(settingsToggle1LRect, toggle1Val, settingsToggle1Label, mySkin.GetStyle("onOffSlider"), mySkin.GetStyle("onOffSliderThumb"), mySkin.GetStyle("SliderLabel"), scaleFactor);
						if(toggle1Val > 0.5f) toggle1 = true; else toggle1 = false;
					}
					if(settingsToggle2Label != "") {
						 toggle2Val = BlackishGUI.OnOffSwitchWithLabel(settingsToggle2LRect, toggle2Val, settingsToggle2Label, mySkin.GetStyle("onOffSlider"), mySkin.GetStyle("onOffSliderThumb"), mySkin.GetStyle("SliderLabel"), scaleFactor);
						if(toggle2Val > 0.5f) toggle2 = true; else toggle2 = false;	
					}
				}
					
				if(Time.realtimeSinceStartup - checkSettingsAtThisInterval > lastTimeSettingsWereChecked) {
					SaveSettings(); //Update all changed settings values in playerprefs
				}
	
					
				if(pauseMenuOn) {
					if(GUI.Button(settingsB1TempRect, "退出")) {
						AudioController.inst.PlaySound(1);	
						showPopupWindow = true; //this cases all gui to be disabled and for the popup to show - look further down at the end of OnGUI...
						popupString = "确定要退出？";
						popupMessageYes = "QuitToMainMenu"; //This function will be called if the user chooses YES in the popup!
					}		
					if(GUI.Button(settingsB2TempRect, "继续")) {	
						AudioController.inst.PlaySound(1);
						StartCoroutine(AnimateMenu(1, new Vector3(1.5f, 0.0f, 0f), 0.0f, 0.5f));  		
						StartCoroutine(CloseMenu());
					}
				} else {
					if(GUI.Button(settingsB1TempRect, "后退")) {
						AudioController.inst.PlaySound(1);
						StartCoroutine(AnimateBars(-65, 95, 0.5f)); 
						StartCoroutine(AnimateMenu(0, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));
						StartCoroutine(AnimateMenu(1, new Vector3(1.5f, 0.0f, 0f), 0.0f, 0.5f));  		
					}
				}			
				
			}
	
			
			
			
			
			//INFO - CREDITS or INSTRUCTIONS - ID 2
			
			if(guiOpacity[2] > 0.0f && new Rect(-1f, -1f, 2f, 2f).Contains(matPos[2])) { // Check if visible and inside view
		
				infoSwipeControl.controlEnabled = true; //Enable SwipeControl
		
				gCol = GUI.color;
				gCol.a = guiOpacity[2];
				GUI.color = gCol;
		
				quat.eulerAngles = matAngle[2];
				matpos = new Vector3(Mathf.Round(Screen.width * matPos[2].x), Mathf.Round(Screen.height * matPos[2].y), matPos[2].z); //set position to the center of the screen
				GUI.matrix = guiMatrix * Matrix4x4.TRS(matpos, quat, Vector3.Scale(matScale[2], globalScale));	
			
				Rect infoTempTitleRect;
				Rect infoTempB1Rect;
			
				if(!orientationIsLandscape) { //Portrait
					infoTempTitleRect = infoTitleRect;
					infoTempB1Rect = infoB1Rect;
				} else { //Landscape
					infoTempTitleRect = infoTitleLRect;
					infoTempB1Rect = infoB1LRect;				
				}
	
				infoSwipeControl.matrix = GUI.matrix; // Tell SwipeControl to use the same Matrix we use here
				
				float offset;
				float mainCredPos;
				
				if(infoScrollsInsteadOfSwipe) { //Scroll
	
					// This creates the scrolling motion from one page to the next:
					infoSwipeControl.currentValue = (int) (-1f + Mathf.Round(Mathf.Repeat((Time.realtimeSinceStartup - infoStartTimeOffset) * 0.20f, (float) infoSwipeControl.maxValue + 2f))); //makes the credits scroll.
				
					if(infoSwipeControl.smoothValue > infoSwipeControl.maxValue + 0.99f) infoStartTimeOffset = Time.realtimeSinceStartup - 0.9f; //makes the images start over faster
					
					if(infoSwipeControl.currentValue >= 0 && infoSwipeControl.smoothValue <= infoSwipeControl.maxValue + 0.9f) { //This prevents hides the images while the control resets from maxValue+1 back to -1			
						offset = infoSwipeControl.smoothValue - infoSwipeControl.currentValue; //Offset from Center
					
						gCol = GUI.color;
					
						//Draw the images
						mainCredPos = infoRect.y - (offset * infoRect.height);
						if(infoSwipeControl.currentValue >= 0 && infoSwipeControl.currentValue < infoImg.Length) {
							if(infoImg[infoSwipeControl.currentValue] && infoSwipeControl.currentValue >= 0 && infoSwipeControl.currentValue < infoImg.Length) {
								gCol.a = 1f - Mathf.Abs(offset); GUI.color = gCol;
								GUI.DrawTexture(new Rect(infoRect.x, mainCredPos, infoRect.width, infoRect.height), infoImg[infoSwipeControl.currentValue]);
							}
						}
						gCol.a = -offset;
						GUI.color = gCol;
						if(infoSwipeControl.currentValue - 1 >= 0 && infoSwipeControl.currentValue - 1 < infoImg.Length) {
							if(infoImg[infoSwipeControl.currentValue - 1] && GUI.color.a > 0.0f && infoSwipeControl.currentValue - 1 >= 0 && infoSwipeControl.currentValue - 1 < infoImg.Length) {
								GUI.DrawTexture(new Rect(infoRect.x, mainCredPos - infoRect.height, infoRect.width, infoRect.height), infoImg[infoSwipeControl.currentValue - 1]);
							}
						}
						gCol.a = offset;
						GUI.color = gCol;
						if(infoSwipeControl.currentValue + 1 >= 0 && infoSwipeControl.currentValue + 1 < infoImg.Length) {
							if(infoImg[infoSwipeControl.currentValue + 1] && GUI.color.a > 0.0f && infoSwipeControl.currentValue + 1 < infoImg.Length && infoSwipeControl.currentValue + 1 >= 0) {
								GUI.DrawTexture(new Rect(infoRect.x, mainCredPos + infoRect.height, infoRect.width, infoRect.height), infoImg[infoSwipeControl.currentValue + 1]);
							}
						}
						gCol.a = 1.0f;
						GUI.color = gCol;
					}
	
				} else { //Swipe
		
					BlackishGUI.Dots(new Vector2(0f, 86f * scaleFactor), infoImg.Length, Mathf.RoundToInt(infoSwipeControl.smoothValue), scaleFactor);
		
					offset = infoSwipeControl.smoothValue - Mathf.Round(infoSwipeControl.smoothValue); //Offset from Center
					
					gCol = GUI.color;
					
					mainCredPos = infoRect.x - (offset * infoRect.width);
					if(Mathf.Round(infoSwipeControl.smoothValue) >= 0 && Mathf.Round(infoSwipeControl.smoothValue) < infoImg.Length) {
						gCol.a = 1f - Mathf.Abs(offset); GUI.color = gCol;
						GUI.DrawTexture(new Rect(mainCredPos, infoRect.y, infoRect.width, infoRect.height), infoImg[Mathf.RoundToInt(infoSwipeControl.smoothValue)]);
					}
					gCol.a = -offset; GUI.color = gCol;
					if(GUI.color.a > 0.0f && Mathf.Round(infoSwipeControl.smoothValue) - 1 >= 0 && Mathf.Round(infoSwipeControl.smoothValue) - 1 < infoImg.Length) {
						GUI.DrawTexture(new Rect(mainCredPos - infoRect.width, infoRect.y, infoRect.width, infoRect.height), infoImg[Mathf.RoundToInt(infoSwipeControl.smoothValue) - 1]);
					}
					gCol.a = offset; GUI.color = gCol;
					if(GUI.color.a > 0.0f && Mathf.Round(infoSwipeControl.smoothValue) + 1 < infoImg.Length && Mathf.Round(infoSwipeControl.smoothValue) + 1 >= 0) {
						GUI.DrawTexture(new Rect(mainCredPos + infoRect.width, infoRect.y, infoRect.width, infoRect.height), infoImg[Mathf.RoundToInt(infoSwipeControl.smoothValue) + 1]);
					}
					gCol.a = 1.0f; GUI.color = gCol;
				
				}
				
				GUI.DrawTexture(infoTempTitleRect, infoTitleImg);
				GUI.DrawTexture(bracketLeftRect, bracketImg);
				GUI.DrawTexture(bracketRightRect, bracketImg);	
		
				GUI.SetNextControlName("back");
				if(GUI.Button(infoTempB1Rect, "后退")) {	
					AudioController.inst.PlaySound(1);
					StartCoroutine(AnimateBars(-65, 95, 0.5f)); 
					StartCoroutine(AnimateMenu(0, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));
					StartCoroutine(AnimateMenu(2, new Vector3(1.5f, 0.0f, 0f), 0.0f, 0.5f));  		
				}
		
			} else {
				if(infoSwipeControl.controlEnabled) infoSwipeControl.controlEnabled = false; //Disable SwipeControl - not needed if out of view!
			}
			
			
			
			
			
			
			
			
			
			//LEVEL SELECTION - ID 3
				
			if(guiOpacity[3] > 0.0f && new Rect(-1f, -1f, 2f, 2f).Contains(matPos[3])) {
				
				levelSelectionSwipeControl.controlEnabled = true; //Enable SwipeControl
				
				gCol = GUI.color;
				gCol.a = guiOpacity[3];
				GUI.color = gCol;
		
				quat.eulerAngles = matAngle[3];
				matpos = new Vector3(Mathf.Round(Screen.width * matPos[3].x), Mathf.Round(Screen.height * matPos[3].y), matPos[3].z); //set position to the center of the screen
				GUI.matrix = guiMatrix * Matrix4x4.TRS(matpos, quat, Vector3.Scale(matScale[3], globalScale));	
			
				Rect levelSelectionTitleTempRect;
				Rect levelSelectionB1TempRect;
				Rect levelSelectionB2TempRect;
				//zc1415926
				Rect levelPreviousBTempRect;
				Rect levelNextBTempRect;
			
				if(!orientationIsLandscape) { //Portrait
					levelSelectionTitleTempRect = levelSelectionTitleRect;
					levelSelectionB1TempRect = levelSelectionB1Rect;
					levelSelectionB2TempRect = levelSelectionB2Rect;
					//zc1415926
					levelPreviousBTempRect = levelPreviousBRect;
					levelNextBTempRect = levelNextBRect;
				} else { //Landscape
					levelSelectionTitleTempRect = levelSelectionTitleLRect;
					levelSelectionB1TempRect = levelSelectionB1LRect;
					levelSelectionB2TempRect = levelSelectionB2LRect;
					//zc1415926
					levelPreviousBTempRect = levelPreviousBLRect;
					levelNextBTempRect = levelNextBLRect;
				}			
				
			
				GUI.DrawTexture(levelSelectionTitleTempRect, levelSelectionTitleImg);
				GUI.DrawTexture(bracketLeftRect, bracketImg);
				GUI.DrawTexture(bracketRightRect, bracketImg);	
				
				levelSelectionSwipeControl.matrix = GUI.matrix;
				selectedLevel = Mathf.RoundToInt(levelSelectionSwipeControl.smoothValue);
	
				BlackishGUI.Dots(new Vector2(0, 86f * scaleFactor), levelStatus, selectedLevel, scaleFactor);
	
				//var offsetFromCenter : float = Mathf.Round(levelSelectionSwipeControl.smoothValue) - levelSelectionSwipeControl.smoothValue;
				float offsetFromCenter = levelSelectionSwipeControl.smoothValue - Mathf.Round(levelSelectionSwipeControl.smoothValue);
				
				float mainPos = levelSelectionImgRect.x - (offsetFromCenter * levelSelectionImgRect.width);
				if(selectedLevel >= 0 && selectedLevel < levelImg.Length) {
					gCol.a = 1 - Mathf.Abs(offsetFromCenter); GUI.color = gCol;
					GUI.DrawTexture(new Rect(mainPos, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImg[selectedLevel]);
					if(levelImgOverlayLocked && levelStatus[selectedLevel] == -1) GUI.DrawTexture(new Rect(mainPos, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayLocked);	
					else if(levelImgOverlayComplete && levelStatus[selectedLevel] == 1) GUI.DrawTexture(new Rect(mainPos, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayComplete);	
				}
				gCol.a = -offsetFromCenter; GUI.color = gCol;
				if(GUI.color.a > 0.0f && selectedLevel - 1 >= 0 && selectedLevel - 1 < levelImg.Length) {
					//if(levelStatus[selectedLevel - 1] == -1) GUI.color.a *= 0.5;
					GUI.DrawTexture(new Rect(mainPos - levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImg[selectedLevel - 1]);
					if(levelImgOverlayLocked && levelStatus[selectedLevel - 1] == -1) GUI.DrawTexture(new Rect(mainPos - levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayLocked);	
					else if(levelImgOverlayComplete && levelStatus[selectedLevel - 1] == 1) GUI.DrawTexture(new Rect(mainPos - levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayComplete);	
				}
				gCol.a = offsetFromCenter; GUI.color = gCol;
				if(GUI.color.a > 0.0f && selectedLevel + 1 < levelImg.Length && selectedLevel + 1 >= 0) {
					//if(levelStatus[selectedLevel + 1] == -1) GUI.color.a *= 0.5;
					GUI.DrawTexture(new Rect(mainPos + levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImg[selectedLevel + 1]);
					if(levelImgOverlayLocked && levelStatus[selectedLevel + 1] == -1) GUI.DrawTexture(new Rect(mainPos + levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayLocked);	
					else if(levelImgOverlayComplete && levelStatus[selectedLevel + 1] == 1) GUI.DrawTexture(new Rect(mainPos + levelSelectionImgRect.width, levelSelectionImgRect.y, levelSelectionImgRect.width, levelSelectionImgRect.height), levelImgOverlayComplete);	
				}
				gCol.a = 1.0f; GUI.color = gCol;

				//zc1415926
				if(levelSelectionSwipeControl.currentValue == 0){
					GUI.enabled = false;
				}

				//if(GUI.Button(levelPreviousBTempRect, "", mySkin.GetStyle("ButtonPreviousArrow") )){
				if(GUI.Button(levelPreviousBTempRect, "", "ButtonPreviousArrow")){
					AudioController.inst.PlaySound(1);
					levelSelectionSwipeControl.currentValue--;
				}
				GUI.enabled = true;

				if(levelSelectionSwipeControl.currentValue == levelSelectionSwipeControl.maxValue){
					GUI.enabled = false;
				}

				//if(GUI.Button(levelNextBTempRect, "", mySkin.GetStyle("ButtonNextArrow") )){
				if(GUI.Button(levelNextBTempRect, "", "ButtonNextArrow")){
					AudioController.inst.PlaySound(1);
					levelSelectionSwipeControl.currentValue++;
				}
				GUI.enabled = true;

				/*levelSelectionSwipeControl
					if(swipeCtrl.currentValue == 0) GUI.enabled = false;	
				if(GUI.Button(new Rect(-150, 95, 80, 30), "Previous")) swipeCtrl.currentValue--;
				GUI.enabled = true;
				if(swipeCtrl.currentValue == swipeCtrl.maxValue) GUI.enabled = false;
				if(GUI.Button(new Rect(70, 95, 80, 30), "Next")) swipeCtrl.currentValue++;	
				GUI.enabled = true;	*/

				if(GUI.Button(levelSelectionB1TempRect, "后退")) {	
					AudioController.inst.PlaySound(1);
					StartCoroutine(AnimateBars(-65, 95, 0.5f)); 
					StartCoroutine(AnimateMenu(0, new Vector3(0.0f, 0.0f, 0f), 1.0f, 0.5f));
					StartCoroutine(AnimateMenu(3, new Vector3(1.5f, 0.0f, 0f), 0.0f, 0.5f));  
					StartCoroutine(UnloadLevelSelectionImagesInSec(1.0f));	
				}
		
				if(levelStatus[selectedLevel] == -1) {
					GUI.enabled = false;
					GUI.Button(levelSelectionB2TempRect, "锁定");
					
				} else {
					if(GUI.Button(levelSelectionB2TempRect, "开始")) {
						AudioController.inst.PlaySound(1);
						StartCoroutine(AnimateMenu(3, new Vector3(1.5f, 0.0f, 0f), 0.0f, 0.5f)); 


						//zc1415926
						PlayerPrefs.SetString ("Character", characterNames[selectedLevel]);

						//level = RedirectSceneName;
						//StartCoroutine(LoadLevelAndCloseMenu(RedirectSceneName));
						//原来的代码


						StartCoroutine(LoadLevelAndCloseMenu(levelSceneNames[selectedLevel]));
						StartCoroutine(UnloadLevelSelectionImagesInSec(1.0f));
					}
				}
				GUI.enabled = true;
		
			} else {
				if(levelSelectionSwipeControl.controlEnabled) levelSelectionSwipeControl.controlEnabled = false; //Disable SwipeControl - not needed if out of view!
			}
		
		} 
		
		
		
		//POPUP - MODAL DIALOG
		if(showPopupWindow) {
			GUI.enabled = true;
			GUI.Box(popupRect, popupString, mySkin.GetStyle("PopupWindow"));
			if(GUI.Button(popupNoRect, "取消")) {
				AudioController.inst.PlaySound(1);
				showPopupWindow = false;
				if(popupMessageNo != "") {
					this.SendMessage(popupMessageNo);	
					popupMessageNo = "";
				}
			}
			GUI.backgroundColor = Color.yellow;
			if(GUI.Button(popupYesRect, "确定")) {
				AudioController.inst.PlaySound(1);
				showPopupWindow = false;	
				if(popupMessageYes != "") {
					this.SendMessage(popupMessageYes);		
					popupMessageYes = "";	
				}
			}
			GUI.backgroundColor = Color.white;
			GUI.enabled = false;
		}
	
		
	
		//PAUSE BUTTON
		if(showInGamePauseButton && pauseButtonImg) {
			
			if(currentBGOpacity < 1.0f) { 
			
				quat.eulerAngles = new Vector3(0.0f, 0.0f, matrixAngle);
				matpos = new Vector3(Mathf.Round(Screen.width * (0.5f)), Mathf.Round(Screen.height * (0.5f)), globalPos.z); //set position to the center of the screen
				GUI.matrix = Matrix4x4.TRS(matpos, quat, globalScale);		
				guiMatrix = GUI.matrix;
		
				Rect pauseButtonTempRect;
				if((matrixAngleClamped > 45f && matrixAngleClamped < 135f) || (matrixAngleClamped > 225f && matrixAngleClamped < 315f)) {
						pauseButtonTempRect = new Rect(Screen.height * 0.5f - pauseButtonImg.height - 3f, -Screen.width * 0.5f + 2f, pauseButtonImg.width, pauseButtonImg.height);
					} else {
						pauseButtonTempRect = new Rect(Screen.width * 0.5f - pauseButtonImg.width - 3f, -Screen.height * 0.5f + 2f, pauseButtonImg.width, pauseButtonImg.height);
					}
				
				gCol = GUI.color;
				gCol.a = 1.0f - currentBGOpacity;
				GUI.color = gCol;
				
				if(GUI.color.a < 1.0f || mainMenuOn || pauseMenuOn) {
					GUI.Button(pauseButtonTempRect, pauseButtonImg, GUIStyle.none);
				} else if(GUI.Button(pauseButtonTempRect, pauseButtonImg, GUIStyle.none)) {
					if(!pauseButtonBusy) {
						AudioController.inst.PlaySound(1);
						StartCoroutine(ShowPauseMenu());	
					}
				}
				gCol.a = 1.0f;
				GUI.color = gCol;
			
			}
		}	
		
		
		//FADER
		GUI.matrix = Matrix4x4.identity;
		if(currentFaderOpacity > 0.0f) {
			gCol.a = currentFaderOpacity;
			GUI.color = gCol;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), faderImg);
			gCol.a = 1.0f;
			GUI.color = gCol;
		}
	
	}
	
	
	void Update() {
				
		if(!mainMenuOn && !pauseMenuOn) {
			if(Input.GetKeyUp("escape")) {
				 StartCoroutine(ShowPauseMenu());
			}
		}
		
		//Calculate a version of the matrixAngle that stays between 0 and 360
		matrixAngleClamped = matrixAngle;
		
		while(matrixAngleClamped < 0f) matrixAngleClamped += 360f;
		while(matrixAngleClamped > 360f) matrixAngleClamped -= 360f;
		
		//Determine Orientation - This is needed for the low resolutions, it determines whether wide elements will fit. 
		if((Screen.width > 480 * scaleFactor && Screen.height > 480 * scaleFactor) || (Screen.width < 480 * scaleFactor && ((matrixAngleClamped > 45f && matrixAngleClamped < 135f) || (matrixAngleClamped > 225f && matrixAngleClamped < 315f))) || (Screen.height < 480 * scaleFactor && (matrixAngleClamped <= 45f || (matrixAngleClamped >= 135f && matrixAngleClamped <= 225f) || matrixAngleClamped >= 315f))) {
			orientationIsLandscape = true;
		} else {
			orientationIsLandscape = false;
		}
		
		//Rotate cameras	
		if(rotateCamerasWithMatrixAngle) {
			for(int i = 0; i < cams.Length; i++) {
				if(cams[i]) {
					Vector3 newAngles = cams[i].transform.eulerAngles;
					newAngles.z = matrixAngleClamped;
					cams[i].transform.eulerAngles = newAngles;
				} else Debug.Log("cam missing!");
			}
		}
	
	
		if(!rotationBusy) {
			
			//Check if the device-orientation changed!
			if(currentDeviceOrientation != Input.deviceOrientation && Input.deviceOrientation != DeviceOrientation.Unknown && Input.deviceOrientation != DeviceOrientation.FaceDown && Input.deviceOrientation != DeviceOrientation.FaceUp) {
	
				//Find out how far we have to rotate		
				//Take current angle and calculate difference to desired angle
				
				float rotDiff = 0.0f;
														
				if(currentDeviceOrientation == DeviceOrientation.Portrait) rotDiff = PortraitAngleOffset;
				else if(currentDeviceOrientation == DeviceOrientation.PortraitUpsideDown) rotDiff = PortraitUpsideDownAngleOffset;
				else if(currentDeviceOrientation == DeviceOrientation.LandscapeLeft) rotDiff = LandscapeLeftAngleOffset;
				else if(currentDeviceOrientation == DeviceOrientation.LandscapeRight) rotDiff = LandscapeRightAngleOffset;
			
				if(Input.deviceOrientation == DeviceOrientation.Portrait) rotDiff -= PortraitAngleOffset;
				else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) rotDiff -= PortraitUpsideDownAngleOffset;
				else if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft) rotDiff -= LandscapeLeftAngleOffset;
				else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight) rotDiff -= LandscapeRightAngleOffset;			
			
				if(rotDiff > 180f) rotDiff = rotDiff - 360f; //if a > 180 then a = 360 - a
				if(rotDiff < -180f) rotDiff = rotDiff + 360f;
	
				StartCoroutine(RotateScreenTo(matrixAngle + rotDiff, 0.4f, Input.deviceOrientation));
		
			}
		}
	
		if(scaleFactor != prevScaleFactor) { //ScaleFactor changed!
			string resourcePath = resourcePath100Percent;
			if(scaleFactor >= 1.5f) resourcePath = resourcePath200Percent;
			LoadResourcesAndScaleRects(resourcePath, scaleFactor);
		}
	
			
	}
	
	
	void OnLevelWasLoaded () {
		Debug.Log("OnLevelWasLoaded!");	
	}
	
	
	void OnApplicationQuit () {
		
		Debug.Log("ApplictionQuit");
		SaveSettings();	
		
		s_Instance = null;	
	}
}
