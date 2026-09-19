using UnityEngine;
using System;
using System.Collections;
using GeneralLibrary;
using GameLibrary;

public class BoxScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	public Box box;
#endregion

#region MainFunctions
	private void Start(){
		box.SetID(UnityEngine.Random.Range(0, 10000).ToString("D4"));
	}

	public void Collect(){
		if (gameController.isHoldingBox){
			return;			
		}
		
		gameController.currentBox.Transfer(box);
		gameController.CollectBox();
		gameObject.SetActive(false);
	}
}
#endregion
