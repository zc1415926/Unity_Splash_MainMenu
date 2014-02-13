using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OccluderController : HighlightingController
{
	private int ox = -220;
	private int oy = 20;
	
	void OnGUI()
	{
		//zc1415926
		ho.OccluderSwitch();
		/*
		float newX = Screen.width + ox;
		GUI.Label(new Rect(newX, oy, 500, 100), "Occluder (moving wall) controls:");
		
		if (GUI.Button(new Rect(newX, oy + 30, 200, 30), "Toggle Occluder"))
		{
			ho.OccluderSwitch();
		}*/
	}
}
