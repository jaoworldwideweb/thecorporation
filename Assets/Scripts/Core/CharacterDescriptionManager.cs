using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using GeneralLibrary;
using GameLibrary;

public class CharacterDescriptionManager : MonoBehaviour{
#region Inspector
	[SerializeField] private CharacterDescription[] characters;
	[SerializeField] private Image spriteOutput;
	[SerializeField] private TMP_Text[] textOutput;
	[Header("Sounds")]
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private AudioClip selectionSound;
	private int currentCharacter = 0;
#endregion

#region MainFunctions
	private void Start(){
		ChangeCurrentCharacter(0);
	}
	
	private void Update(){}
#endregion

#region MenuFunctions
	public void IncreaseCharacterSelection(){
		ChangeCurrentCharacter(1);
		soundHandler.PlaySound(selectionSound, SoundOutput.PlayerSounds);
	}
	
	public void DecreaseCharacterSelection(){
		ChangeCurrentCharacter(-1);
		soundHandler.PlaySound(selectionSound, SoundOutput.PlayerSounds);
	}
	
	public void ChangeCurrentCharacter(int amount){
		currentCharacter += amount;

		if (currentCharacter >= characters.Length){
			currentCharacter = 0;			
		}
		else if (currentCharacter < 0){
			currentCharacter = characters.Length - 1;			
		}
		
		SetMenu(characters[currentCharacter]);
	}
	
	public void SetMenu(CharacterDescription character){
		spriteOutput.sprite = character.photo;		
		textOutput[0].text = character.GetFormatted();
		textOutput[1].text = character.description;
	}
#endregion
}