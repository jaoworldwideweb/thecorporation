using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuScript : MonoBehaviour{
	// fucking foobar bro
	
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private bool usingJoystick{
		get{
			return false;
		}
	}	
	
	private void Update(){
		if (usingJoystick && EventSystem.current.currentSelectedGameObject == null){
			if (!gameController.isMouseLocked){
				gameController.lockMouse();
			}
		}
		else if (!usingJoystick && gameController.isMouseLocked){
			gameController.unlockMouse();
		}
	}
}
