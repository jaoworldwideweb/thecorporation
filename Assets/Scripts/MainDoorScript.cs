using UnityEngine;
using System.Collections;

[System.Serializable]
public class DoorObject{
	public Transform doorTransform;
	public Vector3 closedRotation;
	public Vector3[] openRotation = new Vector3[2];
	// 0 = frontSide
	// 1 = backSide		
}

public class MainDoorScript : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private BaldiScript baldiScript;

	[Header("Main")]
	[SerializeField] private bool isDoorOpen = false;
	[SerializeField] private bool isDoorLocked = false;
	[SerializeField] private float openTime = 1f;
	[SerializeField] private float rotateSpeed = 2f;
	private Coroutine rotateRoutine;
	private Coroutine doorRoutine;
	private doorOpenSide currentSide = doorOpenSide.frontSide;
	public enum doorState{
		openDoor,
		closeDoor,
		lockDoor
	}
	public enum doorOpenSide{
		frontSide = 0,
		backSide = 1
	}

	[Header("Block")]
	[SerializeField] private GameObject lockBlockObstacle;

	[Header("Door")]
	[SerializeField] private DoorObject[] doorObjects;

	[Header("Audio")]
	[SerializeField] private AudioClip doorOpen;
	[SerializeField] private AudioClip doorClose;
	[SerializeField] private AudioSource audioOutput;
#endregion

#region MainFunctions
	private void Start(){
		lockBlockObstacle.SetActive(false);
		isDoorLocked = false;
		isDoorOpen = false;

		foreach (DoorObject door in doorObjects){
			door.doorTransform.localRotation = Quaternion.Euler(door.closedRotation);
		}
	}

	private void OnTriggerEnter(Collider other){
		if (isDoorLocked){
			return;
		}
		if (!other.CompareTag("Player")){
			return;
		}
		if (!isDoorOpen){
			audioOutput.PlayOneShot(doorOpen, 1f);
		}
		if (baldiScript != null && baldiScript.isActiveAndEnabled){
			baldiScript.Hear(transform.position, 1);
		}
		
		Vector3 localPos = transform.InverseTransformPoint(other.transform.position);
		currentSide = localPos.z >= 0 ? doorOpenSide.frontSide : doorOpenSide.backSide;
	}

	private void OnTriggerStay(Collider other){
		if (isDoorLocked || isDoorOpen){
			return;
		}
		if (!other.CompareTag("Player")){
			return;
		}

		doorAction(doorState.openDoor);
	}

	private void OnTriggerExit(Collider other){
		if (isDoorLocked){
			return;
		}
		if (!other.CompareTag("Player")){
			return;
		}

		doorAction(doorState.closeDoor);
	}
#endregion

#region Utility
	private void StopRoutine(ref Coroutine routine){
		if (routine != null){
			StopCoroutine(routine);
			routine = null;
		}
	}
#endregion

#region DoorFunctions
	public void doorAction(doorState performAction){
		switch (performAction){

			case doorState.openDoor:
				openDoor(currentSide);
				break;

			case doorState.closeDoor:
				StopRoutine(ref doorRoutine);
				doorRoutine = StartCoroutine(closeDoor());
				break;

			case doorState.lockDoor:
				StopRoutine(ref doorRoutine);
				doorRoutine = StartCoroutine(lockDoor());
				break;
		}
	}

	private IEnumerator rotateDoors(bool opening, doorOpenSide side){
		float time = 0f;

		Quaternion[] startRotations = new Quaternion[doorObjects.Length];
		Quaternion[] targetRotations = new Quaternion[doorObjects.Length];

		for (int i = 0; i < doorObjects.Length; i++){
			startRotations[i] = doorObjects[i].doorTransform.localRotation;
			targetRotations[i] = Quaternion.Euler(opening ? doorObjects[i].openRotation[(int)side] : doorObjects[i].closedRotation);
		}

		while (time < 1f){
			time += Time.deltaTime * rotateSpeed;
			for (int i = 0; i < doorObjects.Length; i++){
				doorObjects[i].doorTransform.localRotation = Quaternion.Slerp(startRotations[i], targetRotations[i], time);
			}
			
			yield return null;
		}

		for (int i = 0; i < doorObjects.Length; i++){
			doorObjects[i].doorTransform.localRotation = targetRotations[i];
		}

		rotateRoutine = null;
	}

	private void openDoor(doorOpenSide side){
		if (isDoorOpen){
			return;
		}
		
		isDoorOpen = true;
		StopRoutine(ref rotateRoutine);
		rotateRoutine = StartCoroutine(rotateDoors(true, side));
	}

	private IEnumerator closeDoor(){
		yield return new WaitForSeconds(openTime);
		
		StopRoutine(ref rotateRoutine);
		rotateRoutine = StartCoroutine(rotateDoors(false, currentSide));
		doorRoutine = null;
		
		audioOutput.PlayOneShot(doorClose, 1f);
		isDoorOpen = false;
	}

	private IEnumerator lockDoor(int lockTime = 30){
		if(isDoorOpen){
			StopRoutine(ref rotateRoutine);
			rotateRoutine = StartCoroutine(rotateDoors(false, currentSide));
			doorRoutine = null;
			
			audioOutput.PlayOneShot(doorClose, 1f);
			isDoorOpen = false;
		}
		
		isDoorLocked = true;
		lockBlockObstacle.SetActive(true);

		yield return new WaitForSeconds(lockTime);

		isDoorLocked = false;
		lockBlockObstacle.SetActive(false);
		doorRoutine = null;
	}
#endregion
}