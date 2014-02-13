using UnityEngine;
using System.Collections;

public class News : MonoBehaviour {
	
	public int gameID = 0; //unique id for your game
	public int version = 100; //100 equals 1.0.0

	public string newsURL = "http://GameAssets.net/news/"; //Leave empty to display just the default newsText
	public string newsTextIfOffline = "News disabled! If enabled, news are downloaded once per session.\nA unique ID is used to allow for personalized messages."; //Default Text to show if Online toggle is set to "off"
	
	public bool online = true; //Does the Player want to retrieve news or not?
	private int launchCount = 0; //how often has the game been launched?

	public string newsLoadingText = "正在读取新闻......";
	public string newsText = ""; //This is where the news string will end up. Read this to display the news!
	public string newsTextTargetURL = ""; //If the news string comes with a clickable link, store the link in here.
	

		
	void Start() {
	
		//PlayerPrefs.DeleteKey("UniqueID"); //TEMP DELETE
		
		launchCount = PlayerPrefs.GetInt("NewsLaunchCount");
		if(launchCount < 1) { //first launch ever? 
			if(online){
				PlayerPrefs.SetInt("Online", 1); //set online to true!
			}
		}
		
		if(PlayerPrefs.HasKey("Online")) online = true; else online = false;
		
		if(Time.realtimeSinceStartup < 15.0f) {
			launchCount ++;
			PlayerPrefs.SetInt("NewsLaunchCount", launchCount);
		}
		
		DownloadNews();
	}
	
	
	public void ToggleOnline() {
		ToggleOnline(!online);
	}
	public void ToggleOnline(bool newValue) {
		online = newValue;
		if(online) PlayerPrefs.SetInt("Online", 1);
		else PlayerPrefs.DeleteKey("Online");
		StartCoroutine(DownloadNews(online));
	}
	
	
	public void DownloadNews () { StartCoroutine(DownloadNews(true)); } 
	public IEnumerator DownloadNews (bool on) {
		
		if(on && PlayerPrefs.HasKey("Online") && newsURL != "") { //playerPrefsOnlineToggleName holds the name of the PlayerPrefs value that determines if the user has enabled the online connection, newsURL contains the target URL
	
			int platform = 0; // 1 = iPhone/iPodTouch/iPad, 2 = Win, 3 = Mac, 4 = Android, 5 = WinWeb, 6 = OSXWeb, 7 = OSXDashboard, 8 = XBOX360, 9 = PS3
			if(Application.platform == RuntimePlatform.IPhonePlayer) platform = 1;
			else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) platform = 2;
			else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) platform = 3;
			else if(Application.platform == RuntimePlatform.Android) platform = 4;
			else if(Application.platform == RuntimePlatform.WindowsWebPlayer) platform = 5;
			else if(Application.platform == RuntimePlatform.OSXWebPlayer) platform = 6;
			else if(Application.platform == RuntimePlatform.OSXDashboardPlayer) platform = 7;				
			else if(Application.platform == RuntimePlatform.XBOX360) platform = 8;
			else if(Application.platform == RuntimePlatform.PS3) platform = 9;		
	
			string finalURL = newsURL;
			
			finalURL += "news.php?gid=" + gameID + "&v=" + version + "&pf=" + platform;
			Debug.Log(finalURL);
			
			if(Time.realtimeSinceStartup > 20f && PlayerPrefs.HasKey("News")) { //Skip download if game has been running for 20+ sec (this most likely means that the player is returning to the main menu from a level)
				newsText = PlayerPrefs.GetString("News");	
				newsTextTargetURL = PlayerPrefs.GetString("NewsLink");
				Debug.Log("News Loaded from PlayerPrefs, since they have been downloaded once already this session!");	
			} else {
				newsText = newsLoadingText +"\n\n\n\n\n";
				
				// Start a download of the given URL
				WWW www = new WWW (finalURL);
				
				float wwwStartTime = Time.realtimeSinceStartup;
				float wwwTimeoutSec = 10.0f;
				
				// Wait for download to complete
				while(!www.isDone) {
					if(www.error != null || Time.realtimeSinceStartup - wwwStartTime > wwwTimeoutSec) break;
					yield return StartCoroutine(Wait.WaitForSecRealtime(0.2f));
				}
				
				if (www.error != null)
				{
					Debug.Log(www.error);
				}
	
				if(www.isDone && www.error == null) { //it worked!
				
					Debug.Log(www.text);
				
					if(www.text.Length > 1) {
						if(www.text.Substring(0,1) == "<")	{ //ERROR
							//seems like we got an error-page instead of the proper result	
							Debug.Log("Looks like an Error: " + www.text);
							newsText = "Downloading News Failed\nPlease try again later!";
							
						} else { //RESULT OK!
					
							string[] stringArray = www.text.Split(char.Parse("|"));  //expected result: NewsText|Link
							
							if(stringArray.Length > 1) { //code
								if(stringArray[0] != "x" && stringArray[0] != "Error") { 
									
									newsText = stringArray[0];
									PlayerPrefs.SetString("News", newsText);	
															
									if(stringArray.Length >= 2) { //NewsText links to something?
										if(stringArray[1] != "") { 
											newsTextTargetURL = stringArray[1]; 
											PlayerPrefs.SetString("NewsLink", newsTextTargetURL);
											print("news target url found: " + newsTextTargetURL); 
										}
									}
		
								} else if(stringArray[0] == "Error") { //if result is Error|Message, display Message
									newsText = stringArray[1];
								} else {
									newsText = "Downloading News Failed.\n\n\n\n\n";
								}			
							} else {
								newsText = www.text;
								PlayerPrefs.SetString("News", newsText);						
							}	
						}
					}
				} else {
					Debug.Log("Seems downloading news didn't work " + www.error);
					//Downloading news did not work

					//zc1415926
					//newsText = "No News, couldn't reach server. Are you offline?\n\n\n\n\n";
					newsText = newsTextIfOffline;
				}
			}
		} else { // news off
			newsText = newsTextIfOffline;
			newsTextTargetURL = ""; //Clear the link that opens when you click on the News-Message.
		}
	
		
	}
	
	
}
