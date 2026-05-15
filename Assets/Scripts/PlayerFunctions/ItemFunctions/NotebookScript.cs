using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NotebookScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private Transform playerTransform;
	[SerializeField] private boxColorList boxColor;
	
	public enum boxColorList{
		//normal
		red,
		green,
		blue,
		yellow,
		orange,
		
		//light
		lightred,
		lightgreen,
		lightblue,
		lightorange,
	};
#endregion

#region MainFunctions
	private void Update(){
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) && Time.timeScale != 0f){
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			
			if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Box" && Vector3.Distance(playerTransform.position, transform.position) < 10f)){
				if(gameController.isHoldingBox){
					return;
				}
				
				transform.position = new Vector3(transform.position.x, -20f, transform.position.z);
				gameController.collectBox();
			}
		}
	}
#endregion
}
