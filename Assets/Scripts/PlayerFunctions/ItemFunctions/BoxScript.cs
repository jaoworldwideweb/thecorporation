using UnityEngine;
using System;
using System.Collections;
using GeneralLibrary;
using GameLibrary;

public class BoxScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private BoxData boxData;
#endregion

#region MainFunctions
	private void Start(){
		if (boxData == null){
			boxData = new BoxData();			
		}
		
		boxData.boxCode = UnityEngine.Random.Range(1000, 9000);
	}

	public void Collect(){
		if (gameController.isHoldingBox){
			return;			
		}
		
		gameController.currentBoxData.Transfer(boxData);
		gameController.CollectBox();
		gameObject.SetActive(false);
	}
}
#endregion
