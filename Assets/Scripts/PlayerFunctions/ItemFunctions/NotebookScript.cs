using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NotebookScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private Transform playerTransform;
#endregion

#region MainFunctions
	private void Update(){
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) && Time.timeScale != 0f){
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			
			if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Notebook" && Vector3.Distance(playerTransform.position, transform.position) < 10f)){
				transform.position = new Vector3(transform.position.x, -20f, transform.position.z);
				gameController.collectBox();
			}
		}
	}
#endregion
}
