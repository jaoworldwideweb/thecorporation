using UnityEngine;

public class AspectRatioManager : MonoBehaviour{
	private void Start(){
		SetAspectRatio();
	}
	
	private void Awake(){
		SetAspectRatio();
	}

	private void SetAspectRatio(){
		Screen.SetResolution(1920, 1080, true);
	}
}