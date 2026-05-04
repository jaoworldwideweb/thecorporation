using UnityEngine;
using System;

public class EntranceScript : MonoBehaviour{
#region Inspector
	public enum wallState{
		raiseWall,
		lowerWall
	}
#endregion

#region Main
	public void wallAction(wallState currentWallState){
		switch (currentWallState){
			case wallState.raiseWall:
				transform.position += new Vector3(0f, 10f, 0f);
				break;

			case wallState.lowerWall:
				transform.position -= new Vector3(0f, 10f, 0f);
				break;
		}
	}
#endregion
}