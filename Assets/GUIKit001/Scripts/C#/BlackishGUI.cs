///////////////////////////////////////////////
//// 			BlackishGUI.cs             ////
////  copyright (c) 2010 by Markus Hofer   ////
////          for GameAssets.net           ////
///////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public static class BlackishGUI : object {

	//OnOff Slider Switch
	static public float OnOffSwitch(Vector2 pos, float onOff, GUIStyle style, GUIStyle thumbStyle) { return OnOffSwitch(pos, onOff, style, thumbStyle, 1.0f); }
	static public float OnOffSwitch(Vector2 pos, float onOff, GUIStyle style, GUIStyle thumbStyle, float factor) {
		return OnOffSwitch(new Rect(pos.x, pos.y, 121f * factor, 41f * factor), onOff, style, thumbStyle);	
	}
	
	
	static public float OnOffSwitch(Rect screenRect, float onOff, GUIStyle style, GUIStyle thumbStyle) {
		onOff = GUI.HorizontalSlider(screenRect, onOff, 0.0f, 1.0f, style, thumbStyle);
		if(Input.GetMouseButtonUp(0)) {
			if(onOff > 0.5f) onOff = 1.0f;
			else onOff = 0.0f;
		}
		return onOff;
	}
	
	
	//OnOff Slider Switch with Label
	static public float OnOffSwitchWithLabel(Rect screenRect, float onOff, string labelText, GUIStyle style, GUIStyle thumbStyle, GUIStyle labelStyle) { return OnOffSwitchWithLabel(screenRect, onOff, labelText, style, thumbStyle, labelStyle, 1.0f); }
	static public float OnOffSwitchWithLabel(Rect screenRect, float onOff, string labelText, GUIStyle style, GUIStyle thumbStyle, GUIStyle labelStyle, float factor) {
	
		GUI.Label(new Rect(screenRect.x, screenRect.y, screenRect.width - (121f * factor) + (3f * factor), screenRect.height), labelText, labelStyle);
		onOff = OnOffSwitch(new Vector2(screenRect.x + screenRect.width - (121f * factor), screenRect.y), onOff, style, thumbStyle, factor);
		return onOff;
	}
	
	
	//Horizontal Slider with Label

	static public float HorizontalSliderWithLabel(Rect screenRect, float labelWidth, float value, float from, float to, string labelText) { return HorizontalSliderWithLabel(screenRect, labelWidth, value, from, to, labelText, GUI.skin.GetStyle("musicSlider"), GUI.skin.GetStyle("onOffSliderThumb"), GUI.skin.GetStyle("SliderLabel"), 1.0f); }
	static public float HorizontalSliderWithLabel(Rect screenRect, float labelWidth, float value, string labelText) { return HorizontalSliderWithLabel(screenRect, labelWidth, value, 0.0f, 1.0f, labelText, GUI.skin.GetStyle("musicSlider"), GUI.skin.GetStyle("onOffSliderThumb"), GUI.skin.GetStyle("SliderLabel"), 1.0f); }
	static public float HorizontalSliderWithLabel(Rect screenRect, float labelWidth, float value, string labelText, GUIStyle style, GUIStyle thumbStyle, GUIStyle labelStyle) { return HorizontalSliderWithLabel(screenRect, labelWidth, value, 0.0f, 1.0f, labelText, style, thumbStyle, labelStyle, 1.0f); }
	static public float HorizontalSliderWithLabel(Rect screenRect, float labelWidth, float value, string labelText, GUIStyle style, GUIStyle thumbStyle, GUIStyle labelStyle, float factor) { return HorizontalSliderWithLabel(screenRect, labelWidth, value, 0.0f, 1.0f, labelText, style, thumbStyle, labelStyle, 1.0f); }
	static public float HorizontalSliderWithLabel(Rect screenRect, float labelWidth, float value, float from, float to, string labelText, GUIStyle style, GUIStyle thumbStyle, GUIStyle labelStyle, float factor) {
		GUI.Label(new Rect(screenRect.x, screenRect.y, (labelWidth + 3f) * factor, screenRect.height), labelText, labelStyle);
		return GUI.HorizontalSlider(new Rect(screenRect.x + labelWidth * factor, screenRect.y, screenRect.width - labelWidth * factor, screenRect.height), value, from, to, style, thumbStyle);
	}
	
	
	
	//TextFields
	static public string TextFieldWithX(Rect screenRect, string nameString, int flen, bool pw, char maskChar, string fieldName) {
		if(GUI.Button(new Rect(screenRect.x + screenRect.width - 29f, screenRect.y + 12f, 19f, 19f), GUIContent.none, GUI.skin.GetStyle("xButton"))) {
			nameString = "";
			GUI.FocusControl (fieldName);
		} //THIS IS A HACK - we have to draw the button below the text field as well, because the one on top doesn't register when clicked
		GUI.SetNextControlName (fieldName);
		if(pw) nameString = GUI.PasswordField(screenRect, nameString, maskChar, flen, GUI.skin.GetStyle("TextFieldWithX"));
		else nameString = GUI.TextArea(screenRect, nameString, flen, GUI.skin.GetStyle("TextFieldWithX")); //This should really be a TextField, but if I make it a TextField it behaves like a TextArea. Weird stuff...
		if(GUI.Button(new Rect(screenRect.x + screenRect.width - 29f, screenRect.y + 12f, 19f, 19f), GUIContent.none, GUI.skin.GetStyle("xButton"))) {
			nameString = "";
			GUI.FocusControl (fieldName);
		} //HACK: we still need the one on top to show the highlight, though...
		return nameString;
	}

	static public string TextFieldWithX(Rect screenRect, string nameString) { return TextFieldWithX(screenRect, nameString, 255, false, "*"[0], "Field" + screenRect.x + "" + screenRect.y); }
	static public string TextFieldWithX(Rect screenRect, string nameString, int flen) { return TextFieldWithX(screenRect, nameString, flen, false, "*"[0], "Field" + screenRect.x + "" + screenRect.y); }

	static public string TextFieldWithX(Rect screenRect, string nameString, string fieldName) { return TextFieldWithX(screenRect, nameString, 255, false, "*"[0], fieldName); }
	static public string TextFieldWithX(Rect screenRect, string nameString, int flen, string fieldName) { return TextFieldWithX(screenRect, nameString, flen, false, "*"[0], fieldName); }

	static public string PasswordFieldWithX(Rect screenRect, string nameString) { return TextFieldWithX(screenRect, nameString, 255, true, "*"[0], "Field" + screenRect.x + "" + screenRect.y); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, int flen) { return TextFieldWithX(screenRect, nameString, flen, true, "*"[0], "Field" + screenRect.x + "" + screenRect.y); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, char maskChar) { return TextFieldWithX(screenRect, nameString, 255, true, maskChar, "Field" + screenRect.x + "" + screenRect.y); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, char maskChar, int flen) { return TextFieldWithX(screenRect, nameString, flen, true, maskChar, "Field" + screenRect.x + "" + screenRect.y); }

	static public string PasswordFieldWithX(Rect screenRect, string nameString, string fieldName) { return TextFieldWithX(screenRect, nameString, 255, true, "*"[0], fieldName); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, int flen, string fieldName) { return TextFieldWithX(screenRect, nameString, flen, true, "*"[0], fieldName); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, char maskChar, string fieldName) { return TextFieldWithX(screenRect, nameString, 255, true, maskChar, fieldName); }
	static public string PasswordFieldWithX(Rect screenRect, string nameString, char maskChar, int flen, string fieldName) { return TextFieldWithX(screenRect, nameString, flen, true, maskChar, fieldName); }


	static public void Dots(Vector2 centerPos, int dotAmount, int currentPosition) { Dots(centerPos, new int[dotAmount], currentPosition, 1.0f); }
	static public void Dots(Vector2 centerPos, int dotAmount, int currentPosition, GUIStyle guiStyle) { Dots(centerPos, new int[dotAmount], currentPosition, 1.0f, guiStyle); }
	static public void Dots(Vector2 centerPos, int dotAmount, int currentPosition, float factor) { Dots(centerPos, new int[dotAmount], currentPosition, factor, GUI.skin.GetStyle("Dot")); }
	static public void Dots(Vector2 centerPos, int dotAmount, int currentPosition, float factor, GUIStyle guiStyle) { Dots(centerPos, new int[dotAmount], currentPosition, factor, guiStyle); }
	static public void Dots(Vector2 centerPos, int[] dotStatusArray, int currentPosition) { Dots(centerPos, dotStatusArray, currentPosition, 1.0f); }
	static public void Dots(Vector2 centerPos, int[] dotStatusArray, int currentPosition, GUIStyle guiStyle) { Dots(centerPos, dotStatusArray, currentPosition, 1.0f, guiStyle); }
	static public void Dots(Vector2 centerPos, int[] dotStatusArray, int currentPosition, float factor) { Dots(centerPos, dotStatusArray, currentPosition, factor, GUI.skin.GetStyle("Dot")); }
	static public void Dots(Vector2 centerPos, int[] dotStatusArray, int currentPosition, float factor, GUIStyle guiStyle) {
		for(int i = 0; i < dotStatusArray.Length; i++) {
			bool activeOrNot = false;
			if(i == currentPosition) activeOrNot = true; 
			else if(dotStatusArray[i] == -1) GUI.enabled = false;
			//GUI.DrawTexture(new Rect(Mathf.Round(centerPos.x - (dotStatusArray.Length * 16 * factor * 0.5) + (i * 16 * factor)), Mathf.Round(centerPos.y - (16 * factor * 0.5)), 16 * factor, 16 * factor), GUI.skin.GetStyle("Dot").onNormal.background);
			//GUI.Toggle(new Rect(Mathf.Round(centerPos.x - (dotStatusArray.Length * 13 * factor * 0.5) + (i * 13 * factor)), Mathf.Round(centerPos.y - (13 * factor * 0.5)), 13 * factor, 13 * factor), activeOrNot, GUIContent.none, GUI.skin.GetStyle("Dot"));	
			GUI.Toggle(new Rect((centerPos.x - (dotStatusArray.Length * 13f * factor * 0.5f) + (i * 13f * factor)), (centerPos.y - (13f * factor * 0.5f)), 13f * factor, 13f * factor), activeOrNot, GUIContent.none, guiStyle);	
			if(dotStatusArray[i] == -1) GUI.enabled = true;
		}
	}

}
