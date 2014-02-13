using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public string[] songNames = new string[1]; //resource-names, place actual files in Resources/Audio/
	public bool on = false;
	
	public bool autoStart = false;
	public int autoStartWithID = 0;


	//SINGLETON MAGIC

	private static MusicController s_Instance = null;

    public static MusicController inst { 
        get {
            if (s_Instance == null) {
                s_Instance =  FindObjectOfType(typeof (MusicController)) as MusicController;
                if (s_Instance == null)
                    Debug.Log ("Could not locate a MusicController object. You have to have exactly one MusicController in the scene.");
            }
            
            return s_Instance;
        }
    }	

	
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }
	
	
	IEnumerator Start() {
	
		yield return StartCoroutine(Wait.WaitForSecRealtime(0.5f));
		Initialize();
		
	}
	
	
	public void Initialize() {
		
		if(PlayerPrefs.HasKey("MusicLevel")) {
			float vol = PlayerPrefs.GetFloat("MusicLevel");
			if(vol > 0) {
				on = true;	
				audio.volume = vol;
				if(autoStart && !audio.isPlaying) {
					StartMusic(autoStartWithID);
				} else if(!audio.isPlaying){
					StartMusic();				
				}
			} else {
				StopMusic();
			}
		} else {
			StopMusic();
		}		
	
	}
	
	
	
	public void ReInitialize() {
		
		Initialize();
		
	//	if(!audio.isPlaying && !audio.clip) { //NONE-GENERIC
	//		var tempID : int = 0; //default for main menu, play menu music
	//		if(GC) { //ingame, set to stage music
	//			tempID = GC.stage - 1; 
	//			if(GC.stage > 5) tempID = 4;
	//			if(!GC.survivalMode) if(GC.stage < 5 && GC.wave == GC.numWaves[GC.stage]) tempID = 4 + GC.stage;
	//		}
	//		StartMusic(tempID); 
	//	}
		
	}
	
	
	
	public void StopMusic (bool empty) {
		
		audio.Stop();
		if(empty) audio.clip = null;
		on = false;
		
	}
	
	
	public void StopMusic() {
		StopMusic(false);	
	}
	
	
	
	public IEnumerator FadeOutMusic(float sec) { 
		
		if(on) {
			float startVolume = audio.volume; //A var that helps to Make it start from current volume instead of 1.0
			
			//print("FadOutMusic Duration: " + sec);
			//print("FadeOutMusic StartTime: " + Time.time);
			sec -= 0.1f;
			
			for(float i = sec; i >= (sec * 0.25f); i -= 0.1f) {
				audio.volume = startVolume * (0.5f + (i/sec)*0.5f);
				//yield new WaitForSeconds(0.1);
				yield return StartCoroutine(Wait.WaitForSecRealtime(0.1f));
				//print(audio.volume);
			}
			
			for(float i = (sec * 0.75f); i >= 0f; i -= 0.1f) {
				audio.volume = startVolume * (i/sec);
				//yield new WaitForSeconds(0.1);
				yield return StartCoroutine(Wait.WaitForSecRealtime(0.1f));
				//print(audio.volume);
			}
			audio.volume = 0.0f;
			//print(audio.volume);
			//print("FadeOutMusic EndTime: " + Time.time);
		}
	
		StopMusic(true);
		
	}
	
	
	
	public void FadeOutMusic() {
		
		FadeOutMusic(2.0f);	
		
	}
	
	
	public void StartMusic () {
		
		if(on) {
			audio.Play();
		}
	
	}
	
	
	public void StartMusic(int id) {
		
		if(on) {
			if(id < songNames.Length) {
				if(songNames[id] != "") {
					AudioClip myClip;
					//print("loading soundfile");
					myClip = (AudioClip) Resources.Load("Audio/" + songNames[id], typeof(AudioClip));
					//print("playing soundfile!");
					if(myClip) {
						audio.clip = myClip;
						audio.Play();
					} else Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + songNames[id] + " not fount", gameObject);
				} else {
					StopMusic();	
				}
			}
				
		}
		
	}
	
	
	public void ChangeVolume(float vol) {
		audio.volume = vol;	
	}

}
