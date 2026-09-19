using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using MathLibrary;
using GeneralLibrary;
using GameLibrary;

public class MenuManager : MonoBehaviour{
#region Inspector
	[Header("User Interface")]
	[SerializeField] private GameObject forgroundObject;
	[SerializeField] private Image foregroundImage;
	[SerializeField] private CharacterOutput mainCharacterCard;
	[SerializeField] private CharacterOutput characterDescription;
	
	[Header("Interactable User Interface")]
	[SerializeField] private Slider staminaSlider;
	
	[Header("Sound")]
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private AudioClip music;
	[SerializeField] private AudioClip selectionSound;
	
	[Header("Characters")]
	[SerializeField] private CharacterData[] characters;
	private CharacterData mainCharacter;
	[SerializeField] private Date currentDate;
	
	private int currentCharacter = 0;
	private Queue<MenuTransition> transitionQueue = new Queue<MenuTransition>();
	private bool isProcessingQueue = false;
	private bool isChangingMenu = false;
#endregion

#region MainFunctions
	private void Awake(){
		mainCharacter = characters[0];
		SetDescriptionCharacter(mainCharacter);
	}
	
	private void Start(){
		if(!PlayerPrefs.HasKey("OptionsSet")){
			staminaSlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
			PlayerPrefs.Save();
		}
		else{
			PlayerPrefs.SetInt("OptionsSet", 1);
			PlayerPrefs.Save();
		}
		
		soundHandler.PlayMusic(music, MusicOutput.MainSong);
		StartCoroutine(StartFadeIn());
		staminaSlider.onValueChanged.AddListener(SetMouseSensitivity);
	}
	
	private void Update(){}
#endregion

#region CharacterDescription
	public void IncreaseCharacterSelection(){
		ChangeCurrentCharacter(1);
		soundHandler.PlaySound(selectionSound, SoundOutput.PlayerSounds);
	}
	
	public void DecreaseCharacterSelection(){
		ChangeCurrentCharacter(-1);
		soundHandler.PlaySound(selectionSound, SoundOutput.PlayerSounds);
	}
	
	private void ChangeCurrentCharacter(int amount){
		currentCharacter += amount;

		if (currentCharacter >= characters.Length){
			currentCharacter = 0;			
		}
		else if (currentCharacter < 0){
			currentCharacter = characters.Length - 1;			
		}
		
		SetDescriptionCharacter(characters[currentCharacter]);
	}
	
	private void SetDescriptionCharacter(CharacterData character){
		characterDescription.image.sprite = character.GetPhoto();
		characterDescription.name.text = character.GetBasicFormatted(currentDate);
		characterDescription.description.text = character.GetDescription();
	}
#endregion

#region MenuCalls
	private void SetMouseSensitivity(float value){
		PlayerPrefs.SetFloat("MouseSensitivity", value);
	}
	
	private void SetLowQualitySettings(bool value){
		GeneralLibrary.SaveData.SetBool("IS_LOW", value);
	}
	
	private IEnumerator StartFadeIn(){
		forgroundObject.SetActive(true);
		yield return UserInterface.IFadeImage(foregroundImage, new dfloat(1f, 0f), 3f);
		forgroundObject.SetActive(false);
	}
	
	public void SaveData(){
		PlayerPrefs.Save();
	}
	
	public void LoadScene(string sceneName){
		StartCoroutine(ILoadScene(sceneName));
	}
	
	private IEnumerator ILoadScene(string sceneName){
		float waitTime = 2f;
		
		soundHandler.FadeMusic(3f, 0f, MusicOutput.Ambience, true);
		
		forgroundObject.SetActive(true);
		UserInterface.FadeImage(foregroundImage, new dfloat(0f, 1f), waitTime);
		
		yield return new WaitForSeconds(waitTime * 2f);
		SceneManager.LoadScene(sceneName);
	}
	
	public void SetMainCharacterCard(){
		mainCharacterCard.image.sprite = mainCharacter.GetPhoto();
		mainCharacterCard.name.text = mainCharacter.GetName();
		mainCharacterCard.description.text = mainCharacter.GetFullFormatted();
	}
	
	public void ExitGame(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			UnityEngine.Application.Quit();
		#endif
	}
	
	public void SwitchMenus(GameObject currentMenu, GameObject nextMenu){
		if(isChangingMenu){
			return;
		}
		
		isChangingMenu = true;
		
		transitionQueue.Enqueue(new MenuTransition(currentMenu, nextMenu));

		if (!isProcessingQueue){
			StartCoroutine(ProcessQueue());
		}
	}
	
	private IEnumerator ProcessQueue(){
		isProcessingQueue = true;
		
		while (transitionQueue.Count > 0){
			MenuTransition transition = transitionQueue.Dequeue();
			yield return StartCoroutine(SlowlySetObjectStates(false, transition.currentMenu));
			yield return StartCoroutine(SlowlySetObjectStates(true, transition.nextMenu));
		}

		isProcessingQueue = false;
		isChangingMenu = false;
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