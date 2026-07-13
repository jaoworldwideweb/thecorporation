using UnityEngine;

public class AspectRatioManager : MonoBehaviour{
	[Header("Target Aspect Ratio")]
	public int aspectWidth = 16;
	public int aspectHeight = 9;
	public bool fullscreen = true;
	
	private void Start(){
		SetAspectRatio();
	}
	
	private void Awake(){
		SetAspectRatio();
	}

	private void SetAspectRatio(){
		Resolution current = Screen.currentResolution;
		int screenWidth = current.width;
		int screenHeight = current.height;
		float targetAspect = (float)aspectWidth / aspectHeight;
		int newWidth = screenWidth;
		int newHeight = Mathf.RoundToInt(newWidth / targetAspect);

		if (newHeight > screenHeight){
			newHeight = screenHeight;
			newWidth = Mathf.RoundToInt(newHeight * targetAspect);
		}
		
		Screen.SetResolution(newWidth, newHeight, fullscreen);
	}
}