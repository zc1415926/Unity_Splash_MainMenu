using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public bool busy = false;
	public AudioClip[] sound = new AudioClip[3];
	
	public bool on = true;
	public bool voiceOff = false;
	


	//SINGLETON MAGIC

	private static AudioController s_Instance = null;

    public static AudioController inst { 
        get {
            if (s_Instance == null) {
                s_Instance =  FindObjectOfType(typeof (AudioController)) as AudioController;
                if (s_Instance == null)
                    Debug.Log ("Could not locate a AudioController object. You have to have exactly one AudioController in the scene.");
            }
            
            return s_Instance;
        }
    }
    
	// Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }    
 
	
	
	void Start () {
		Initialize();
	}
	
	
	
	public void Initialize () {
	
		if(PlayerPrefs.HasKey("SoundLevel")) {
			float vol = PlayerPrefs.GetFloat("SoundLevel");
			if(vol > 0) {
				audio.volume = vol;
				SwitchAudioOn();
			} else {
				SwitchAudioOff(); 
			}
		} else {
			SwitchAudioOff(); 
		}	
		
	}
	
	
	public IEnumerator QueueSound (string soundFile) {
	
		while(audio.isPlaying) {
			yield return new WaitForSeconds(1.0f);	
		}
		
		if(on) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = (AudioClip) Resources.Load("Audio/" + soundFile, typeof(AudioClip));
			//print("playing soundfile!");	
			if(myClip) {
				PlayThisSound(myClip);
			} else {
				Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + soundFile + " not fount");
			}
		}
		
	}
	
	
	public IEnumerator QueueSound (string soundFile, bool isVoice) {
		
		while(audio.isPlaying) {
			yield return new WaitForSeconds(1.0f);	
		}
		
		if((isVoice && !voiceOff) || (on && !isVoice)) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = (AudioClip) Resources.Load("Sound/" + soundFile, typeof(AudioClip));
			//print("playing soundfile!");	
			if(myClip) {
				PlayThisSound(myClip);
			} else {
				Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + soundFile + " not fount");
			}
		}
		
	}
	
	
	public IEnumerator QueueSound (int soundID) {
	
		while(audio.isPlaying) {
			yield return new WaitForSeconds(1.0f);	
		}
		
		if(on) {
			PlayThisSound(sound[soundID]);
		}
		
	}
	
	
	public IEnumerator QueueSound (int soundID, bool isVoice) {
	
		while(audio.isPlaying) {
			yield return new WaitForSeconds(1.0f);	
		}
		
		if((isVoice && !voiceOff) || (on && !isVoice)) {
			PlayThisSound(sound[soundID]);
		}
		
	}
	
	
	
	
	public void PlayThisSound (AudioClip soundClip) {
		audio.clip = soundClip;
		audio.Play();	
	}
	
	
	public void PlaySound (string soundFile) {
		if(on) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = (AudioClip) Resources.Load("Sound/" + soundFile, typeof(AudioClip));
			//print("playing soundfile!");
			if(myClip) audio.PlayOneShot(myClip);
			else Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + soundFile + " not fount");
		}
	}
	
	public void PlaySound (string soundFile, bool isVoice) {
		if((isVoice && !voiceOff) || (on && !isVoice)) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = (AudioClip) Resources.Load("Sound/" + soundFile, typeof(AudioClip));
			//print("playing soundfile!");
			if(myClip) audio.PlayOneShot(myClip);
			else Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + soundFile + " not fount");
		}
	}
	
	
	public void PlaySound (int soundID) {
		
		if(on) {
			//print("playing soundfile " + soundID + "!");
			if(sound.Length > soundID) {
				if(sound[soundID]) audio.PlayOneShot(sound[soundID]);
				else Debug.LogWarning("THIS SHOULD NOT HAPPEN: SoundID "+soundID+" not fount");
			} else Debug.LogWarning("THIS SHOULD NOT HAPPEN: SoundID "+soundID+" not available - array too short!");
		}
	}
	
	
	public void PlayBackgroundSound (string soundFile) {
		if(on) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = (AudioClip) Resources.Load("Sound/" + soundFile, typeof(AudioClip));
			//print("playing soundfile!");
			if(myClip) {
				audio.clip = myClip;
				audio.Play();
			} else Debug.LogWarning("THIS SHOULD NOT HAPPEN: Soundfile " + soundFile + " not fount");
		}
	}
	
	
	public void PlayBackgroundSound (int soundID) {
		if(on) {
			AudioClip myClip;
			//print("loading soundfile");
			myClip = sound[soundID];
			//print("playing soundfile!");
			if(myClip) {
				audio.clip = myClip;
				audio.Play();
			} else Debug.LogWarning("THIS SHOULD NOT HAPPEN: SoundID " + soundID + " not fount");
		}
	}
	
	public void SetLooping (bool yes) {
		audio.loop = yes;	
	}
	
	
	public void SwitchAudioOn () {
		on = true;	
		//audio.Play();
		if(audio.clip) audio.Play();
	}
	
	public void SwitchAudioOff() {
		on = false;
		audio.Stop();	
	}
    
    
}
