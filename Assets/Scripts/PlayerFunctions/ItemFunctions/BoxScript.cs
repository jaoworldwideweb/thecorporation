using UnityEngine;
using System;
using System.Collections;

public class BoxScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private Transform playerTransform;
	[SerializeField] private boxColorList boxColor;

	public enum boxColorList{
		Red,
		Green,
		Blue,
		Yellow,
		Orange,
		
		LightRed,
		LightGreen,
		LightBlue,
		LightOrange
	}
#endregion

#region MainFunctions
	private void Update(){
		if (!Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact)){
			return;			
		}
		if (Time.timeScale == 0f){
			return;			
		}
		
		Ray raycast	= Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
		RaycastHit raycastHit;

		if (Physics.Raycast(raycast, out raycastHit)){
			if (raycastHit.transform.CompareTag("Box")){
				float distance = Vector3.Distance(playerTransform.position, raycastHit.transform.position);

				if (distance > 10f){
					return;					
				}
				if (gameController.isHoldingBox){
					return;					
				}
				
				gameObject.SetActive(false);
				gameController.collectBox();
			}
		}
	}
#endregion
}