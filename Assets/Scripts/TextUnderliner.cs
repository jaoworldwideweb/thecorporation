using UnityEngine;
using TMPro;

public class TextUnderliner : MonoBehaviour{
	public TMP_Text textObject;	
	
	public void Underline(){
		textObject.fontStyle = FontStyles.Underline;
	}
	public void Normal(){
		textObject.fontStyle = FontStyles.Normal;
	}
}
