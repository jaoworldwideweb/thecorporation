using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameLibrary;

public class SoundHandler : MonoBehaviour{
#region Inspector
	[Header("Sound Output")]
	public AudioSource[] soundEffects = new AudioSource[2];
	public AudioSource[] music = new AudioSource[2];

	private Queue<AudioClip>[] soundQueues;
	private bool[] isPlaying;
	private int lastMusicIndex = -1;
#endregion

#region MainFunctions
	private void Start(){
		// nullspace
	}

	private void Awake(){
		soundQueues = new Queue<AudioClip>[soundEffects.Length];
		isPlaying = new bool[soundEffects.Length];
		
		for (int i = 0; i < soundEffects.Length; i++){
			soundQueues[i] = new Queue<AudioClip>();
			isPlaying[i] = false;
		}
	}

	private void Update(){
		// nullspace
	}
#endregion

#region SoundFunctions
	private bool IsSoundReady(AudioClip sound, SoundEffectsOutput output){
		if (sound == null){
			return false;
		}
		
		int outputSlot = (int)output;
		
		if (outputSlot < 0 || outputSlot >= soundEffects.Length){
			return false;
		}
		if (soundEffects[outputSlot] == null){
			return false;
		}
		
		return true;
	}

	public void PlaySound(AudioClip sound, SoundEffectsOutput output = SoundEffectsOutput.PlayerSounds){
		if (!IsSoundReady(sound, output)){
			return;
		}

		int outputSlot = (int)output;
		soundEffects[outputSlot].PlayOneShot(sound);
	}

	public void PlaySoundOnQueue(AudioClip sound, SoundEffectsOutput output = SoundEffectsOutput.PlayerSounds){
		if (!IsSoundReady(sound, output)){
			return;
		}
		
		int outputSlot = (int)output;
		soundQueues[outputSlot].Enqueue(sound);
		
		if (!isPlaying[outputSlot]){
			StartCoroutine(ProcessQueue(output));
		}
	}

	private IEnumerator ProcessQueue(SoundEffectsOutput output){
		int outputSlot = (int)output;
		isPlaying[outputSlot] = true;
		
		while (soundQueues[outputSlot].Count > 0){
			AudioClip clip = soundQueues[outputSlot].Dequeue();
			soundEffects[outputSlot].PlayOneShot(clip);
			yield return new WaitForSeconds(clip.length);
		}
		
		isPlaying[outputSlot] = false;
	}
#endregion

#region MusicFunctions
	private bool IsSongReady(AudioClip sound, MusicOutput output){
		if (sound == null){
			return false;
		}
		
		int outputSlot = (int)output;
		
		if (outputSlot < 0 || outputSlot >= music.Length){
			return false;
		}
		if (music[outputSlot] == null){
			return false;
		}
		
		return true;
	}

	public void PlayMusicFromList(AudioClip[] musicTracks, bool isRandom = true){
		if (musicTracks == null || musicTracks.Length == 0){
			return;
		}
		
		int index = 0;
		
		// i hate this part of code.
		if (!isRandom){
			index = (lastMusicIndex + 1) % musicTracks.Length;
		}
		else{
			index = UnityEngine.Random.Range(0, musicTracks.Length);

			if (musicTracks.Length > 1){
				while (index == lastMusicIndex){
					index = UnityEngine.Random.Range(0, musicTracks.Length);
				}
			}
		}
		
		lastMusicIndex = index;
		AudioClip newTrack = musicTracks[index];
		PlayMusic(newTrack, MusicOutput.MainSong, false);
	}
	
	public void PlayMusic(AudioClip song, MusicOutput output = MusicOutput.MainSong, bool canLoop = true){
		if (!IsSongReady(song, output)){
			return;
		}
		
		int outputSlot = (int)output;
		
		if (music[outputSlot].isPlaying){
			music[outputSlot].Stop();
			music[outputSlot].loop = false;
			music[outputSlot].clip = null;
		}
		
		music[outputSlot].clip = song;
		music[outputSlot].Play();
		music[outputSlot].loop = canLoop;
	}

	public void FadeMusic(float time = 3f, float targetVolume = 0f, MusicOutput output = MusicOutput.MainSong,bool isFadeOut = false){
		int outputSlot = (int)output;
		float startVolume = music[outputSlot].volume;
		float endVolume = isFadeOut ? targetVolume : 1f;
		
		StartCoroutine(IFadeMusic(time, startVolume, endVolume, outputSlot));
	}

	private IEnumerator IFadeMusic(float time, float startVolume, float endVolume, int outputSlot){
		if (time <= 0f){
			music[outputSlot].volume = endVolume;
			yield break;
		}
		
		float elapsed = 0f;
		
		while (elapsed < time){
			elapsed += Time.deltaTime;
			
			float t = elapsed / time;
			music[outputSlot].volume = Mathf.Lerp(startVolume, endVolume, t);
			
			yield return null;
		}
		music[outputSlot].volume = endVolume;
	}

	public void StopMusic(MusicOutput output = MusicOutput.MainSong){
		int outputSlot = (int)output;

		if (outputSlot < 0 || outputSlot >= music.Length){
			return;
		}
		if (music[outputSlot] == null){
			return;
		}
		if (music[outputSlot].isPlaying){
			music[outputSlot].loop = false;
			music[outputSlot].Stop();
			music[outputSlot].clip = null;
		}
	}

	public bool IsMusicPlaying(MusicOutput output = MusicOutput.MainSong){
		int outputSlot = (int)output;

		if (outputSlot < 0 || outputSlot >= music.Length){
			return false;
		}
		if (music[outputSlot] == null){
			return false;
		}
		
		return music[outputSlot].isPlaying;
	}
#endregion
}
