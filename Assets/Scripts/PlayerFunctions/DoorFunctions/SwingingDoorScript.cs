using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SwingingDoorScript : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private BaldiScript baldiScript;
	
	[Header("Main")]
	[SerializeField] private bool isDoorOpen = false;
	[SerializeField] private bool isDoorLocked = false;
	[SerializeField] private float openTime = 1.3f;
	
	private Coroutine doorRoutine;
	public enum doorState{
		openDoor,
		closeDoor,
		lockDoor
	};
	
	[Header("Block")]
	[SerializeField] private GameObject lockBlockObstacle;
	
	[Header("Textures")]
	[SerializeField] private MeshRenderer insideMeshRenderer;
	[SerializeField] private MeshRenderer outsideMeshRenderer;
	
	[Header("Materials")]
	[SerializeField] private Material openedDoorMaterial;	
	[SerializeField] private Material closedDoorMaterial;
	[SerializeField] private Material lockedDoorMaterial;
	
	[Header("Audio")]
	[SerializeField] private AudioClip doorOpen;
	[SerializeField] private AudioSource audioOutput;
#endregion

#region MainFunctions0
	private void Start(){
		lockBlockObstacle.SetActive(false);
		isDoorLocked = false;
		isDoorOpen = false;
	}
	
	private void Update(){
		// nullspace
	}
	
	private void OnTriggerEnter(Collider other){
		if(isDoorLocked){
			return;
		}
		
		audioOutput.PlayOneShot(doorOpen, 1f);
		
		if(other.tag == "Player" && baldiScript.isActiveAndEnabled){
			baldiScript.Hear(transform.position, 1f);
		}
	}
	
	private void OnTriggerStay(Collider other){
		if(isDoorOpen){
			return;
		}
		
		doorAction(doorState.openDoor);
	}
	
	private void OnTriggerExit(Collider other){
		doorAction(doorState.closeDoor);
	}
#endregion

#region MainDoorFunctions
	public void doorAction(doorState performAction){
		if(isDoorLocked){
			return;
		}
		
		switch(performAction){
			case doorState.openDoor:
				openDoor();
				break;
			
			case doorState.closeDoor:
				if (doorRoutine != null){
					StopCoroutine(doorRoutine);				
				}
				
				if(!isDoorOpen){
					return;
				}
				
				doorRoutine = StartCoroutine(closeDoor());				
				break;
				
			case doorState.lockDoor:
				lockDoor();
				break;
		}
	}
	
	// main
	private void openDoor(){
		if(isDoorOpen){
			return;
		}
		
		isDoorOpen = true;
		setDoorMaterial(openedDoorMaterial);
	}
	
	private IEnumerator closeDoor(){
		yield return new WaitForSeconds(openTime);
		
		isDoorOpen = false;
		setDoorMaterial(closedDoorMaterial);
	} 
	
	private IEnumerator lockDoor(int lockTime = 30){
		if(isDoorOpen){
			closeDoor();
		}
		
		setDoorMaterial(lockedDoorMaterial);		
		isDoorLocked = true;
		lockBlockObstacle.SetActive(true);
		
		yield return new WaitForSeconds(lockTime);
		
		setDoorMaterial(closedDoorMaterial);
		isDoorLocked = false;
		lockBlockObstacle.SetActive(false);
	}
	
#endregion

#region HelperDoorFunctions
	private void setDoorMaterial(Material setMaterial){
		insideMeshRenderer.material = setMaterial;
		outsideMeshRenderer.material = setMaterial;
	}
#endregion
}