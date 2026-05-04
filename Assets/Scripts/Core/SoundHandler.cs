using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundHandler : MonoBehaviour {
#region Inspector
	[Header("Sound Output")]
	public AudioSource[] soundEffects;
	public AudioSource[] music;

	private Queue<AudioClip>[] soundQueues;
	private bool[] isPlaying;
#endregion

#region MainFunctions
	private void Start(){
		// nullspace
	}

	private void Awake(){
		soundQueues = new Queue<AudioClip>[soundEffects.Length];
		isPlaying = new bool[soundEffects.Length];

		for (int i = 0; i < soundEffects.Length; i++)
		{
			soundQueues[i] = new Queue<AudioClip>();
			isPlaying[i] = false;
		}
	}
	
	private void Update(){
		// nullspace
	}
#endregion

#region SoundFunctions
	private bool isSoundReady(AudioClip sound, int outputSlot){
		if (sound == null){
			return false;
		}
		if (outputSlot < 0){
			return false;
		}

		return true;
	}
	
	public void playSound(AudioClip sound, int outputSlot = 0){
		if(!isSoundReady(sound, outputSlot)){
			return;
		}
		
		soundEffects[outputSlot].PlayOneShot(sound);	
	}

	public void playSoundQueue(AudioClip sound, int outputSlot = 0){
		if(!isSoundReady(sound, outputSlot)){
			return;
		}
		
		soundQueues[outputSlot].Enqueue(sound);	

		if (!isPlaying[outputSlot]){
			StartCoroutine(processQueue(outputSlot));
		}
	}

	private IEnumerator processQueue(int outputSlot){
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
	private bool isSongReady(AudioClip sound, int outputSlot){
		if (sound == null){
			Debug.LogError("SoundHandler: No song was specified. Did you write the song's name incorrectly?");
			return false;
		}

		if (outputSlot < 0){
			Debug.LogError("SoundHandler: The output slot is invalid. Did you provide a negative slot number?");
			return false;
		}

		return true;
	}
	
	public void loopMusic(AudioClip song, int outputSlot = 0)
	{
		if(!isSongReady(song, outputSlot)){
			return;
		}
		
		music[outputSlot].loop = true;
		
		if(music[outputSlot].isPlaying){
			music[outputSlot].Stop();
			music[outputSlot].clip = null;
		}
		
		music[outputSlot].clip = song;
		music[outputSlot].Play();
	}
	
	public void stopMusic(int outputSlot = 0){
		if(music[outputSlot].isPlaying){
			music[outputSlot].loop = false;
			music[outputSlot].Stop();
			music[outputSlot].clip = null;
		}
	}
#endregion
}