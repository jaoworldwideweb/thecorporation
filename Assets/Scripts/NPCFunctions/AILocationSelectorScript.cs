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
	
	public bool IsNearHallwayCorner(Vector3 position, float radius){
		float radiusSqr = radius * radius;

		for (int i = 0; i < hallways.Length; i++){
			Transform hallway = hallways[i];
			
			if (hallway == null){
				continue;
			}

			Vector3 diff = hallway.position - position;
			if (diff.sqrMagnitude <= radiusSqr){
				return true;
			}
		}

		return false;
	}
}
