using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SwingingDoorScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private BaldiScript baldiScript;
	
	[Header("States")]
	[SerializeField] private bool isDoorOpen;
	[SerializeField] private bool isDoorLocked;	
	
	[Header("Lock Block")]
	[SerializeField] private MeshCollider lockBlockMesh;
	[SerializeField] private GameObject lockBlockObstacle;
	
	[Header("Mesh Renderers")]
	[SerializeField] private MeshRenderer insideMeshRenderer;
	[SerializeField] private MeshRenderer outsideMeshRenderer;
	
	[Header("Materials")]
	[SerializeField] private Material closedDoorMaterial;
	[SerializeField] private Material openedDoorMaterial;
	[SerializeField] private Material lockedDoorMaterial;
	
	[Header("Audio")]
	[SerializeField] private AudioClip doorOpen;
	[SerializeField] private AudioSource audioOutput;
	
	private Coroutine doorRoutine;
	private Coroutine doorLockRoutine;
	
	private void OnTriggerStay(Collider other){
		if (!isDoorLocked){
			if (doorRoutine != null){
				StopCoroutine(doorRoutine);				
			}
			
			doorRoutine = StartCoroutine(openDoor(2f));
		}
	}
	
	private void OnTriggerEnter(Collider other){
		if (!isDoorLocked){
			audioOutput.PlayOneShot(doorOpen, 1f);
			
			if (other.tag == "Player" && baldiScript.isActiveAndEnabled){
				baldiScript.Hear(transform.position, 1f);
			}
		}
	}	
	
	private IEnumerator openDoor(float openTime = 2f){
		isDoorOpen = true;
		setDoorMaterial(openedDoorMaterial);
		
		yield return new WaitForSeconds(openTime);
		
		isDoorOpen = false;
		setDoorMaterial(closedDoorMaterial);
		
		doorRoutine = null;
	}
	
	public void lockDoor(float time){
		if (doorLockRoutine != null){
			StopCoroutine(doorLockRoutine);				
		}
		
		doorLockRoutine = StartCoroutine(lockDoorFull(2f));
	}
	
	private IEnumerator lockDoorFull(float lockTime){
		setDoorState(true);
		setDoorMaterial(lockedDoorMaterial);

		yield return new WaitForSeconds(lockTime);
		
		setDoorState(false);
		setDoorMaterial(closedDoorMaterial);
		
		doorLockRoutine = null;
	}
	
	private void setDoorMaterial(Material setMaterial){
		insideMeshRenderer.material = setMaterial;
		outsideMeshRenderer.material = setMaterial;
	}
	
	private void setDoorState(bool isOpen){
		lockBlockMesh.enabled = isOpen;
		lockBlockObstacle.SetActive(isOpen);
		isDoorLocked = isOpen;
	}
}
