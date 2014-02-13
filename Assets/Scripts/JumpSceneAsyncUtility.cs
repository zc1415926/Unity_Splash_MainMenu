using UnityEngine;
using System.Collections;

public class JumpSceneAsyncUtility : MonoBehaviour
{
	private AsyncOperation async;
	private int progress;
	
	public IEnumerator loadSceneAsync(string sceneName)
	{		
		async = Application.LoadLevelAsync(sceneName);
		
		async.allowSceneActivation = false;
		yield return async ;
	}
	
	public AsyncOperation Async
	{
		get { return async; }
		//set { async = value; }
	}
	
	public int Progress
	{
		get
		{
			return (int)(async.progress * 100);
		}
	}
	
	public void jumpLevel()
	{
		async.allowSceneActivation = true;
	}
}