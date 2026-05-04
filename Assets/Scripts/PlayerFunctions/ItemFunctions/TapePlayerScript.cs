using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TapePlayerScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private string itemName;
	[SerializeField] private AudioSource audioDevice;
	[SerializeField] private Sprite[] tapeSprites;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private BaldiScript baldiScript;
	[SerializeField] private Transform playerTransform;
	
	private bool isPlaying = false;
	private float waitTime;
#endregion

#region MainFunctions
	private void Start(){
		waitTime = audioDevice.clip.length;
	} 
	
	private void Update(){
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) && Time.timeScale != 0f){
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			
			if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == itemName && Vector3.Distance(playerTransform.position, transform.position) < 10f)){
				StartCoroutine(queuePlayTape());
			}
		}
	}
#endregion

#region TapeFunction
	private IEnumerator queuePlayTape(){
		if(isPlaying){
			yield break;
		}
		
		isPlaying = true;
		StartCoroutine(playTape());
		yield return new WaitForSeconds(waitTime);
		isPlaying = false;
	}
	
	private IEnumerator playTape(){
		if(!baldiScript.isActiveAndEnabled){
			yield break; // remember this shit for later.
		}
		
		spriteRenderer.sprite = tapeSprites[1];
		audioDevice.Play();
		
		baldiScript.ActivateAntiHearing(waitTime);
		yield return new WaitForSeconds(waitTime);
		
		spriteRenderer.sprite = tapeSprites[0];
		audioDevice.Stop();
	}
#endregion
}
