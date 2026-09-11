using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MathLibrary;
using GeneralLibrary;
using GameLibrary;

public class FakeMessagePing : MonoBehaviour{
#region Inspector
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private AudioClip messagePing;
#endregion

#region MainFunctions
	private void Start(){
		// nullspace
	}
	
	private void Update(){
		// nullspace
	}
#endregion

#region MessagePingFunctions
	private void StartMessagePing(){
		StartCoroutine(MessagePingLoop());
	}
	
	private IEnumerator MessagePingLoop(){
		while(true){
			dint randomRange;
			
			do{
				randomRange = new dint(UnityEngine.Random.Range(300, 600), UnityEngine.Random.Range(300, 600));
			}while(randomRange.a > randomRange.b);
			
			float time = UnityEngine.Random.Range(randomRange.a, randomRange.b);
			
			yield return new WaitForSeconds(time);
			
			if(time % 2 == 0){
				continue;
			}
			
			soundHandler.PlaySound(messagePing, SoundOutput.GameSounds);
		}
	}
#endregion
}