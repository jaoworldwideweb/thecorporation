using System;
using UnityEngine;

public class Billboard : MonoBehaviour{
	private Camera PointCamera;
	
	private void Start(){
		PointCamera = Camera.main;
	}

	private void LateUpdate(){
		if (PointCamera == null){
			return;
		}
		
		Vector3 lookDirection = PointCamera.transform.forward;
		lookDirection.y = 0f;
		lookDirection.Normalize();

		transform.rotation = Quaternion.LookRotation(lookDirection);
	}
}
