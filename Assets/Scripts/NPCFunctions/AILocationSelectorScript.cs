using UnityEngine;

public class AILocationSelectorScript : MonoBehaviour{
	[SerializeField] private Transform[] hallways = new Transform[29];
	[SerializeField] private Transform[] rooms = new Transform[29];
	
	public Vector3 GetNewTarget(){
		if (Random.value < 0.5f){
			return GetNewTargetHallway();			
		}
		
		return GetNewTargetRoom();
	}
	
	public Vector3 GetNewTargetHallway(){
		return hallways[UnityEngine.Random.Range(0, hallways.Length)].position;
	}
	
	public Vector3 GetNewTargetRoom(){
		return rooms[UnityEngine.Random.Range(0, rooms.Length)].position;
	}
}
