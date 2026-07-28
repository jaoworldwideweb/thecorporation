using UnityEngine;
using System.Collections;

public class HumanoidKiller : Character{
#region Inspector
	[SerializeField] private Transform playerTransform;
#endregion

#region MainFunctions
	private void Start(){
		StartRoutine(WanderRoutine());
	}
	
	private void Update(){
		if(!IsNearPlayer()){
			return;
		}
	}
	
	private void OnDestinationReached(){
		StartRoutine(WanderRoutine());
	}
	
	private bool IsNearPlayer(){
		if (Vector3.Distance(playerTransform.position, gameObject.transform.position) > 20f){
			return false;
		}
		return true;
	}
#endregion
}