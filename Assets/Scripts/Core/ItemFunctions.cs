using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using GeneralLibrary;
using GameLibrary;
using System.Runtime.Intrinsics.X86;

public class ItemFunctions : MonoBehaviour{
#region Inspector
	[SerializeField] private PlayerScript playerScript;
#endregion

#region ItemFunctions
	public void ChocolateBar(){
		playerScript.stamina = playerScript.maxStamina;
	}
	
	public void DrinkableSoda(){
		if(playerScript.isPlayerDrinking){
			return;
		}
		
		StartCoroutine(IDrinkSoda(5f, 0.5f));
	}
	
	private IEnumerator IDrinkSoda(float healthPerSip, float sipTime){
		playerScript.isDrinking = true;
		
		for (int i = 0; i < 3; i++){
			yield return new WaitForSeconds(sipTime);
			playerScript.health += healthPerSip;
		}
		
		playerScript.isDrinking = false;
	}
#endregion
}