using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GeneralLibrary;

[System.Serializable]
public class CharacterFace
{
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
	
	private void Update() { }
#endregion

#region FaceHandler
	private IEnumerator DefaultFaceHandler(){
		while (true){
			if (currentAction != Actions.LookingAround){
				yield return null;
				continue;
			}
			
			SetDirection(General.RandomEnumValue<Direction>());
			int faceIndex = Mathf.Max(GetFaceIndex() - 1, 1);
			yield return new WaitForSeconds(character.lookingFrequency / faceIndex);
		}
	}
	
	public void TakeDamage(float pain){
		bool isReallyBad = pain >= 20f;
		
		if (!isReallyBad){
			SetFace();
			return;
		}
		
		currentAction = Actions.TakeDamage;
		SetFace(true);
		StartCoroutine(TakeDamageRoutine(pain));
	}
	
	private IEnumerator TakeDamageRoutine(float pain){
		yield return new WaitForSeconds(CalculatePainTime(pain));
		currentAction = Actions.LookingAround;
		SetFace();
	}
	
	private void SetDirection(Direction dir){
		currentDirection = dir;
		
		if (currentAction != Actions.TakeDamage){
			SetFace();
		}
	}
	
	private void SetFace(bool shocked = false){
		int faceIndex = GetFaceIndex();

		if (shocked){
			faceCanvas.sprite = character.shocked[faceIndex];
			return;
		}

		switch (currentDirection){
			case Direction.Forward:
				faceCanvas.sprite = character.lookingForward[faceIndex];
				break;

			case Direction.Left:
				faceCanvas.sprite = character.lookingLeft[faceIndex];
				break;

			case Direction.Right:
				faceCanvas.sprite = character.lookingRight[faceIndex];
				break;
		}
	}
	
	private int GetFaceIndex(){
		return Mathf.Clamp(4 - Mathf.FloorToInt(playerScript.health / 20f), 0, 4);
	}
	
	private float CalculatePainTime(float pain){
		return Mathf.Clamp(Mathf.CeilToInt(pain / 20f), 2, 4);
	}
	#endregion
}
