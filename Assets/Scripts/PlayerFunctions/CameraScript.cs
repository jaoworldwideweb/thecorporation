using UnityEngine;
using System;

public class CameraScript : MonoBehaviour{
	[Header("Script")]
	[SerializeField] private GameControllerScript gameController;
	
	[Header("Player/Camera")]
	[SerializeField] private GameObject playerObject;
	[SerializeField] private int lookBehind;
	[SerializeField] private float mouseSensitivity = 100f;	
	private float xRotation = 0f;	
	
	[Header("Baldi")]
	[SerializeField] private Transform baldiTransform;
	[SerializeField] private Vector3 baldiOffset;
	
	private void Start(){
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
		if (!gameController.isGameOver){

			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 25f);
			
			transform.localRotation = Quaternion.Euler(xRotation, lookBehind, 0f);
		}
		else{
			transform.position = baldiTransform.position + baldiTransform.forward * baldiOffset.z + new Vector3(0f, baldiOffset.y, 0f);
			transform.LookAt(new Vector3(baldiTransform.position.x, baldiTransform.position.y + baldiOffset.y, baldiTransform.position.z));
		}
	}
}
