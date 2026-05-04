using UnityEngine;
using System;

public class CameraScript : MonoBehaviour{
	[Header("Script")]
	[SerializeField] private GameControllerScript gameController;
	
	[Header("Player/Camera")]
	[SerializeField] private GameObject playerObject;
	[SerializeField] private Vector3 camOffset;
	[SerializeField] private int lookBehind;	
	
	[Header("Baldi")]
	[SerializeField] private Transform baldiTransform;
	[SerializeField] private Vector3 baldiOffset;	
	
	private void Start(){
		camOffset = transform.position - playerObject.transform.position;
	}
	
	private void Update(){
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.LookBehind)){
			lookBehind = 180; //Look behind you
		}
		else{
			lookBehind = 0; //Don't look behind you
		}
	}
	
	private void LateUpdate(){
		transform.position = playerObject.transform.position + camOffset;
		
		if (!gameController.isGameOver){
			transform.position = playerObject.transform.position + camOffset; 
			transform.rotation = playerObject.transform.rotation * Quaternion.Euler(0f, (float)lookBehind, 0f);
		}
		else if (gameController.isGameOver){
			transform.position = baldiTransform.transform.position + baldiTransform.transform.forward * baldiOffset.z + new Vector3(0f, baldiOffset.y, 0f);
			transform.LookAt(new Vector3(baldiTransform.position.x, baldiTransform.position.y + baldiOffset.y, baldiTransform.position.z));
		}
	}
}
