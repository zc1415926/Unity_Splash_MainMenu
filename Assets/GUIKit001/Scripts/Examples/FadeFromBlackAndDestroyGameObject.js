// Fade with one color in, wait, and fade out again
// Default settings: Fade from transparent to opaque and back to transparent


#pragma strict


var mytexture : Texture2D;
var fadeColor : Color = new Color(0,0,0,1.0);

var textString : String = "Loading...";

var startOpacity = 0.0;
var waitOpacity = 1.0;
var endOpacity = 1.0;

var lengthInSec = 1.0;
var lengthWait = 0.0;
var lengthOutSec = 0.0;
var fade : float = 0.0; //progress of the Lerp, set it to 0.5 to start half-ways, for example...
var waiting : float = 0.0;
var timePassed : float = 0;
private var xsize : int = 500;
private var ysize : int = 40;
private var fadeInRate : float;
private var waitingRate : float;
private var fadeOutRate : float;
 var currentOpacity : float;
private var phase : int = 1;
private var cols : Color[];

var positionRect : Rect;

var GUIDepth : int = -5;

var destroyGameObject : boolean = false;


function Start () {
	
	if(positionRect.width < 1 || positionRect.height < 1) positionRect = Rect(0,0,Screen.width,Screen.height);

	fadeInRate = 1 / lengthInSec;
	waitingRate = 1 / lengthWait;
	fadeOutRate = 1 / lengthOutSec;
	
	currentOpacity = startOpacity;

	if(!mytexture) { //if no texture is given, create one in the color given	
		mytexture = new Texture2D(1, 1);
		cols = mytexture.GetPixels();
	
		for( var i = 0; i < cols.Length; ++i ) {
			cols[i] = fadeColor;
		}
	
		mytexture.SetPixels(cols);
	
		// Apply all SetPixel calls
		mytexture.Apply();	
	} 
}


function Update () {

	switch(phase) {
		case 1:
			fade += Time.deltaTime * fadeInRate; 
			currentOpacity = Mathf.Lerp(startOpacity, waitOpacity, fade);
			if(fade > 1) {
				phase++;
				fade = 0;
			}
			break;
		case 2:
			fade += Time.deltaTime * waitingRate; 
			currentOpacity = waitOpacity;
			if(fade > 1) { 
				phase++;
				fade = 0;
			}
			break;
		case 3:
			fade += Time.deltaTime * fadeOutRate; 
			currentOpacity = Mathf.Lerp(waitOpacity, endOpacity, fade);
			if(fade > 1) {
				if(destroyGameObject) 
					Destroy(gameObject);	
				else
					Destroy(this);
			}
			break;	
	}
		
	

}


function OnLevelWasLoaded () {
	if(!mytexture) { //if no texture is given, create one in the color given	
		mytexture = new Texture2D(1, 1);
		cols = mytexture.GetPixels();
	
		for( var i = 0; i < cols.Length; ++i ) {
			cols[i] = fadeColor;
		}
	
		mytexture.SetPixels(cols);
	
		// Apply all SetPixel calls
		mytexture.Apply();	
	} 
}



function OnGUI () {
	
	GUI.depth = GUIDepth;

	GUI.color = Color(1,1,1);
	GUI.color.a = currentOpacity;
	GUI.DrawTexture (positionRect, mytexture,ScaleMode.StretchToFill, true);
	if(textString) GUI.Label (Rect(5,Screen.height-23, 100, 20), textString);

}