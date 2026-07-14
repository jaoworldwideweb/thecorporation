using MaterialKit;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour{
	public CanvasScaler normScaler;
	public RectTransform[] transforms;
	
	private void Start(){
		int resolutionPreset = PlayerPrefs.GetInt("UiSize");
		int verticalOffsetMode = PlayerPrefs.GetInt("UiHeight");
		
		Vector2[] resolutions = {
			Vector2.zero,
			new Vector2(640f, 480f),
			new Vector2(800f, 600f),
			new Vector2(900f, 720f),
			new Vector2(1024f, 720f)
		};

		float offset = 0f;

		switch (verticalOffsetMode){
			case 1:
				offset = Screen.height / 8f;
				break;

			case 2:
				offset = Screen.height / 4f;
				break;
		}
		
		if (resolutionPreset >= 1 && resolutionPreset < resolutions.Length){
			normScaler.referenceResolution = resolutions[resolutionPreset];
		}
		
		if (offset != 0){
			foreach (RectTransform rect in transforms){
				rect.position += Vector3.up * offset;
			}
		}
	}
}
