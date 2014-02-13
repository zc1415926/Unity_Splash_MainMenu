using UnityEngine;
using System.Collections;

public static class LevelStatus : object {

	public static void Set (int levelID, int status) {
		PlayerPrefs.SetInt("LevelStatus" + levelID, status);
	}

	public static void Lock (int levelID) {
		Set(levelID, -1);	
	}
	
	public static void Unlock (int levelID) {
		Set(levelID, 0);	
	}
	
	public static void Complete(int levelID) {
		Set(levelID, 1);	
	}
	
	
	public static int Get (int levelID) {
		return PlayerPrefs.GetInt("LevelStatus" + levelID);	
	}
	
	
}
