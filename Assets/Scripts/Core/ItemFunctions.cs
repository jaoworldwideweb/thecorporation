using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using GeneralLibrary;
using GameLibrary;

public class ItemFunctions : MonoBehaviour{
#region Inspector
	[SerializeField] private PlayerScript playerScript;
	[SerializeField] private ItemHandler itemHandler;
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private Transform playerTransform; 
	[SerializeField] private Transform cameraTransform; 
	
	[Header("Prefabs")]
	[SerializeField] private GameObject sodaSpray;
#endregion

#region ItemFunctions
	public bool ChocolateBar(){
		if(playerScript.stamina >= playerScript.maxStamina){
			return false;
		}
		
		playerScript.stamina = playerScript.maxStamina;
		return true;
	}
	
	public bool DrinkableSoda(){
		if(playerScript.isDrinking){
			return false;
		}
		
		StartCoroutine(IDrinkSoda(5f, 0.5f));
		return true;
	}
	
	private IEnumerator IDrinkSoda(float healthPerSip, float sipTime){
		playerScript.isDrinking = true;
		
		for (int i = 0; i < 3; i++){
			yield return new WaitForSeconds(sipTime);
			playerScript.DoHealthAction(HealthAction.Regeneration, healthPerSip, CreatureType.None);
		}
		
		playerScript.isDrinking = false;
	}
	
	public bool SprayableSoda(){
		Quaternion rotation = cameraTransform.rotation;
		rotation.eulerAngles = new Vector3(5f, rotation.eulerAngles.y, rotation.eulerAngles.z);
		
		UnityEngine.Object.Instantiate(sodaSpray, playerTransform.position, rotation);
		return true;
	}
#endregion
}