using UnityEngine;
using System;
using System.Collections;
using GeneralLibrary;

[System.Serializable]
public class BoxData{
#region Data
	public Color currentBoxColor = Color.Red;
	public int boxCode = 0;
	
	public enum Color{
		Red,
		Green,
		Blue,
		Yellow,
		Orange,
		
		LightRed,
		LightGreen,
		LightBlue,
		LightOrange
	};
	public enum DataType{
		Color,
		Code
	};
#endregion

#region Functions	
	public string GetFormattedData(){
		return $"{currentBoxColor.ToString().ToUpper()}-{boxCode}";
	}
	
	public string GetRawData(DataType type){
		switch (type){
			case DataType.Color:
				return currentBoxColor.ToString();

			case DataType.Code:
				return boxCode.ToString();

			default:
				return "nullpointer";
		}
	}
	
	public void ClearData(){
		currentBoxColor = Color.Red;
		boxCode = 0;
	}
#endregion
}

public class BoxScript : MonoBehaviour{
#region Inspector
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private BoxData boxData;
#endregion

#region MainFunctions
	private void Start(){
		if (boxData == null){
			boxData = new BoxData();			
		}
		
		boxData.boxCode = UnityEngine.Random.Range(1000, 9000);
	}

	public void Collect(){
		if (gameController.isHoldingBox){
			return;			
		}
		
		gameController.currentBoxData.currentBoxColor = boxData.currentBoxColor;
		gameController.currentBoxData.boxCode = boxData.boxCode;
		gameController.CollectBox();
		gameObject.SetActive(false);
	}
}
#endregion
