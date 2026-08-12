using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GeneralLibrary;
using GameLibrary;

public class ColorRoomTrigger : MonoBehaviour{
#region Inspector
	[Header("References")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private Slider progressSlider;
	[SerializeField] private GameObject progressSliderObject;
	[SerializeField] private BoxColor roomColor = BoxColor.Red;
	
	[Header("Settings")]
	[SerializeField] private float requiredHoldTime = 8f;
	[SerializeField] private float maxSpeedMultiplier = 3f;

	private Collider triggerCollider;
	private float triggerRadius;
	private float holdProgress;
	private bool isInsideRoomTrigger;
#endregion	

#region MainFunctions
	private void Awake(){
		triggerCollider = GetComponent<Collider>();
		triggerRadius = triggerCollider.bounds.extents.x;

		progressSlider.minValue = 0f;
		progressSlider.maxValue = requiredHoldTime;
		progressSlider.value = 0f;
		progressSlider.gameObject.SetActive(false);
	}
	
	private void Update(){
		if (!isInsideRoomTrigger || !isBoxValid() || !gameController.isHoldingBox){
			return;			
		}
		
		if (!Singleton<InputManager>.Instance.GetActionKey(InputAction.BoxAction)){
			ResetProgress();
			return;
		}

		float distance = Vector3.Distance(gameController.playerTransform.position, transform.position);
		float closeness = Mathf.Clamp01(1f - distance / triggerRadius);
		float speed = Mathf.Lerp(1f, maxSpeedMultiplier, closeness);
		
		progressSliderObject.SetActive(true);
		holdProgress += Time.deltaTime * speed;
		progressSlider.value = holdProgress;

		if (holdProgress >= requiredHoldTime){
			gameController.PutBoxInPlace();
			progressSliderObject.SetActive(false);
			ResetProgress();
		}
	}
	
	private bool isBoxValid(){
		if(gameController.currentBoxData.currentBoxColor == roomColor){
			return true;
		}
		
		return false;
	}
	
	private void ResetProgress(){
		holdProgress = 0f;
		progressSlider.value = 0f;
	}
	
	private void OnTriggerEnter(Collider other){
		if (!other.CompareTag("Player")){
			return;			
		}	
		
		gameController.roomColor = roomColor;
		isInsideRoomTrigger = true;
		gameController.isInsideRoomTrigger = true;
	}

	private void OnTriggerExit(Collider other){
		if (!other.CompareTag("Player")){
			return;	
		}
		
		isInsideRoomTrigger = false;
		progressSliderObject.SetActive(false);
		ResetProgress();
		
		StartCoroutine(gameController.MoveRoomInformation(false));
		gameController.isInsideRoomTrigger = false;
	}
#endregion
}
