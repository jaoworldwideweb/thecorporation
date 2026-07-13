using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GeneralLibrary;

[System.Serializable]
public class CharacterFace{
	public Sprite[] shocked = new Sprite[5];
	public Sprite[] lookingForward = new Sprite[5];
	public Sprite[] lookingLeft = new Sprite[5];
	public Sprite[] lookingRight = new Sprite[5];

	public float lookingFrequency = 1.5f;
}

public class CharacterFaceManager : MonoBehaviour{
#region Inspector
	[SerializeField] private PlayerScript playerScript;
	[SerializeField] private CharacterFace character;
	[SerializeField] private Image faceCanvas;
	[SerializeField] private Actions currentAction = Actions.LookingAround;
	[SerializeField] private Direction currentDirection = Direction.Forward;

	private enum Actions{
		LookingAround,
		TakeDamage
	};
	private enum Direction{
		Forward,
		Left,
		Right
	};
#endregion

#region MainFunctions
	private void Start(){
		SetDirection(Direction.Forward);
		StartCoroutine(DefaultFaceHandler());
	}
	
	private void Update(){
		// null
	}
#endregion

#region FaceHandler
	private IEnumerator DefaultFaceHandler(){
		while(true){
			if(currentAction == Actions.LookingAround){
				SetDirection(cGeneral.RandomEnumValue<Direction>());

				int faceIndex = Mathf.Max(GetFaceIndex() - 1, 1);
				yield return new WaitForSeconds(character.lookingFrequency / faceIndex);
			}
			else{
				yield return null;
			}
		}
	}

	public IEnumerator TakeDamage(float pain){
		Direction previousDirection = currentDirection;

		if(currentAction != Actions.TakeDamage){
			currentAction = Actions.TakeDamage;
		}

		SetDirection(Direction.Forward);

		int faceIndex = GetFaceIndex();
		faceCanvas.sprite = character.shocked[faceIndex];

		yield return new WaitForSeconds(CalculatePainTime(pain));

		currentAction = Actions.LookingAround;
		SetDirection(previousDirection);
	}

	private void SetDirection(Direction dir){
		currentDirection = dir;
		faceCanvas.sprite = GetDirectionSprites(dir)[GetFaceIndex()];
	}

	private Sprite[] GetDirectionSprites(Direction dir){
		switch(dir){
			case Direction.Forward:
				return character.lookingForward;
				
			case Direction.Left:
				return character.lookingLeft;
			
			case Direction.Right:
				return character.lookingRight;
		}

		return character.lookingForward;
	}

	private float CalculatePainTime(float pain){
		return Mathf.Clamp(Mathf.CeilToInt(pain / 20f), 2, 4);
	}

	private int GetFaceIndex(){
		return Mathf.Clamp(4 - Mathf.FloorToInt(playerScript.healthValue / 20f), 0, 4);
	}
#endregion
}