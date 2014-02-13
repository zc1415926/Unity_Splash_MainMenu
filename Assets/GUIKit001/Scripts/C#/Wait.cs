using UnityEngine;
using System.Collections;

public static class Wait : object {

	public static IEnumerator WaitForSecRealtime (float sec) {
	
		float waitTime = Time.realtimeSinceStartup + sec;
		while (Time.realtimeSinceStartup < waitTime) {
			// we are waiting...
			yield return null;
		}
		//print("end realtime" + Time.realtimeSinceStartup);
	}
}
