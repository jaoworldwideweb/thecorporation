using UnityEngine;
using System;

public class CameraScript : MonoBehaviour{
	[SerializeField] private float mouseSensitivity = 100f;	
	private float xRotation = 0f;	
	
	private void Start(){}
	
	private void Update(){}
	
	private void LateUpdate(){
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		
		transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);;
	}
}
