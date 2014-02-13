var img : Texture2D;

function OnGUI () {
	GUI.depth = 100;

	var quat : Quaternion = Quaternion.identity;
	quat.eulerAngles = Vector3(0.0, 0.0, 0.0);
	var matpos : Vector3 = Vector3(Mathf.Round(Screen.width * (0.5)), Mathf.Round(Screen.height * (0.5)), 0.0); //set position to the center of the screen
	GUI.matrix = Matrix4x4.TRS(matpos, quat, Vector3.one);		
	
	if(Screen.width > Screen.height) {
		GUI.DrawTexture(Rect((-0.5) * Screen.width, (-0.5) * Screen.width, Screen.width * (1), Screen.width * (1)), img); 
	} else {
		GUI.DrawTexture(Rect((-0.5) * Screen.height, (-0.5) * Screen.height, Screen.height * (1), Screen.height * (1)), img); 
	}	
	
}