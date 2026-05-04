using UnityEngine;
using System;

public class NearExitTriggerScript : MonoBehaviour{
	private void OnTriggerEnter(Collider other){
		if (gameController.exitsReached < 3 && gameController.isGameFinale && other.tag == "Player"){
			gameController.exitReached();
			// entranceScript.wallAction(EntranceScript.wallState.lowerWall);
			
			if (gameController.baldiScript.isActiveAndEnabled){
				gameController.baldiScript.Hear(transform.position, 8f);
			}
		}
	}
	
	[SerializeField] private GameControllerScript gameController;
	// [SerializeField] private EntranceScript entranceScript;
}
