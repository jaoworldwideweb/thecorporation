using UnityEngine;

public class AILocationSelectorScript : MonoBehaviour{
	public Transform[] newLocation = new Transform[29];
	
	public Vector3 GetNewTarget(){
		return newLocation[UnityEngine.Random.Range(0, newLocation.Length)].position;
	}
	
	public Vector3 GetNewTargetHallway(){
		return newLocation[UnityEngine.Random.Range(0, 15)].position;
	}
}
