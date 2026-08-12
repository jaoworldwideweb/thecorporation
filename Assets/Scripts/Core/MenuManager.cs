using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using GeneralLibrary;
using GameLibrary;

public class MenuManager : MonoBehaviour{
#region Inspector
	[SerializeField] private GameObject forgroundObject;
	[SerializeField] private Image foregroundImage;
	[SerializeField] private Slider staminaSlider;
	[SerializeField] private TMP_Text[] employeeDescriptionOutput = new TMP_Text[2];
	[SerializeField] private Image idPhoto;
	
	[Header("Charcter")]
	[SerializeField] private EmployeeData currentEmployee;
	[SerializeField] private Date currentDate;
	
	private Queue<MenuTransition> transitionQueue = new Queue<MenuTransition>();
	private bool isProcessingQueue = false;
	
	private struct MenuTransition{
		public GameObject CurrentMenu;
		public GameObject NextMenu;

		public MenuTransition(GameObject currentMenu, GameObject nextMenu){
			CurrentMenu = currentMenu;
			NextMenu = nextMenu;
		}
	}
#endregion

#region MainFunctions
	private void Start(){
		if (PlayerPrefs.HasKey("OptionsSet")){
			staminaSlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
			PlayerPrefs.Save();
		}
		else{
			PlayerPrefs.SetInt("OptionsSet", 1);
			PlayerPrefs.Save();
		}
		
		// for vex since he got the old builds that did horrid screen size changes
		if(!PlayerPrefs.HasKey("beta_HasNewResolutionBeenSet")){
			Screen.SetResolution(1920, 1080, true);
			PlayerPrefs.SetInt("beta_HasNewResolutionBeenSet", 1);
			PlayerPrefs.Save();
		}
		
		forgroundObject.SetActive(false);
	}
	
	private void Update(){
		PlayerPrefs.SetFloat("MouseSensitivity", staminaSlider.value);
	}
#endregion

#region ButtonCalls
	public void SaveData(){
		PlayerPrefs.Save();
	}
	
	public void LoadScene(string sceneName){
		StartCoroutine(ILoadScene(sceneName));
	}
	
	private IEnumerator ILoadScene(string sceneName){
		float waitTime = 2f;
		forgroundObject.SetActive(true);
		StartCoroutine(FadeImage(0f, 1f, waitTime, foregroundImage));
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene(sceneName);
	}
	
	public void SetEmployeeDescription(){
		idPhoto.sprite = currentEmployee.photo;
		
		employeeDescriptionOutput[0].text = currentEmployee.name;
		employeeDescriptionOutput[1].text = currentEmployee.GetFormattedData();
	}
	
	public void ExitGame(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			UnityEngine.Application.Quit();
		#endif
	}
	
	public void SwitchMenus(GameObject currentMenu, GameObject nextMenu){
		transitionQueue.Enqueue(new MenuTransition(currentMenu, nextMenu));

		if (!isProcessingQueue){
			StartCoroutine(ProcessQueue());
		}
	}
#endregion

#region HelperFunctions
	public IEnumerator FadeImage(float startAlpha, float endAlpha, float duration, Image image){
		float elapsed = 0f;
		Color color = image.color;
		
		while (elapsed < duration){
			elapsed += Time.deltaTime;
			
			color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
			image.color = color;
			
			yield return null;
		}
		
		color.a = endAlpha;
		image.color = color;
	}
	
	private IEnumerator ProcessQueue(){
		isProcessingQueue = true;
		
		while (transitionQueue.Count > 0){
			MenuTransition transition = transitionQueue.Dequeue();
			yield return StartCoroutine(SlowlySetObjectStates(false, transition.CurrentMenu));
			yield return StartCoroutine(SlowlySetObjectStates(true, transition.NextMenu));
		}

		isProcessingQueue = false;
	}
	
	public IEnumerator SlowlySetObjectStates(bool isActive, GameObject mainObject){
		List<GameObject> children = new List<GameObject>();

		foreach (Transform child in mainObject.transform){
			children.Add(child.gameObject);
		}

		if (isActive){
			mainObject.SetActive(true);
			
			foreach (GameObject child in children){
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));
				child.SetActive(true);
			}
		}
		else{
			foreach (GameObject child in children){
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
				child.SetActive(false);
			}
			
			mainObject.SetActive(false);
		}
	}	
#endregion
}