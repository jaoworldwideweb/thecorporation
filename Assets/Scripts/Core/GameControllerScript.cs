using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MathLibrary;
using GeneralLibrary;
using GameLibrary;

public class GameControllerScript : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;
	[SerializeField] private ItemHandler itemHandler;
	
	[Header("Player")]
	public Transform playerTransform;
	[SerializeField] private Camera[] sceneCameras;
	[SerializeField] private Camera playerCamera;
	
	[Header("NPCs")]
	[SerializeField] private GameObject[] npcObjects;
	
	[HideInInspector] public bool hasGameStarted = false;
	[HideInInspector] public bool isGameFinale = false;
	[HideInInspector] public bool isGameOver = false;
	[HideInInspector] public bool isDebugMode = false;
	[HideInInspector] public bool isMouseLocked = true;
	[HideInInspector] public bool isGamePaused = false;
	[HideInInspector] public bool isInsideRoomTrigger = false;
	
	[Header("User Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	
	[Header("Box")]
	[SerializeField] private TMP_Text boxCounter;
	[SerializeField] private UITextObject boxInformation;
	[SerializeField] private UITextObject roomInformation;
	public int maxBoxes = 9;
	[SerializeField] private FullObject boxViewmodel;
	[SerializeField] private Vector3 boxViewmodelFinalPoint;
	[SerializeField] private float boxViewmodelBobSpeed;
	
	[HideInInspector] public int collectedBoxes = 0;
	[HideInInspector] public BoxData currentBoxData;
	[HideInInspector] public BoxColor roomColor = BoxColor.Red;
	[HideInInspector] public bool isHoldingBox = false;
	private float boxViewmodelBobTime;
	private float boxViewmodelBobAmount;	
	
	[Header("Game Over")]
	[SerializeField] private Image gameOverRender;
	[SerializeField] private Creature[] creatures;
	
	[Header("Exit")]
	[SerializeField] private EntranceScript entrance;
	
	[Header("Scene Management")]
	[SerializeField] private string exitGameScene;
	[SerializeField] private string gameOverScene;
	
	[Header("Audio")]
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private AudioClip[] musicTracks;
	[SerializeField] private AudioClip grabBoxSound;
	[SerializeField] private AudioClip dropBoxSound;
	private AudioClip lastMusicTrack;
#endregion

#region MainFunctions
	private void Start(){
		LockMouse();
		currentBoxData.ClearData();
		
		boxCounter.text = UpdateBoxCount();
		boxViewmodel.obj.SetActive(false);
		boxViewmodel.SetOldTransform();
		boxInformation.SetOldTransform();
		
		soundHandler.PlayMusicFromList(musicTracks);
	}
	
	private void Update(){
		General.DoActionFromInput(PauseSwitch, InputAction.PauseOrCancel);
		
		if (!soundHandler.IsMusicPlaying()){
			soundHandler.PlayMusicFromList(musicTracks);
		}
		
		// some bs
		if (!isGamePaused & Time.timeScale != 1f){
			Time.timeScale = 1f;
		}
		else{
			if (Time.timeScale != 0f){
				Time.timeScale = 0f;
			}
		}
		
		if(isGamePaused || isGameOver){
			return;
		}
		
		BobBox();
		
		// input
		General.DoActionFromInput(itemHandler.SetItemSelection, InputAction.Slot0, 0);
		General.DoActionFromInput(itemHandler.SetItemSelection, InputAction.Slot1, 1);
		General.DoActionFromInput(itemHandler.UseItem, InputAction.UseItem);
		
		UIObjectToggle(boxInformation, InputAction.Tab, MoveBoxInformation);
		UIObjectToggle(roomInformation, InputAction.Q, MoveRoomInformation);
		
		// raycast
		General.DoRaycastForObject(hit =>{
			BoxScript boxViewmodel = hit.transform.GetComponent<BoxScript>();
			
			if (boxViewmodel == null){
				return;			
			}
			boxViewmodel.Collect();			
		}, playerCamera, playerTransform, 40f);
		
		General.DoRaycastForObject(hit =>{
			ItemObject item = hit.transform.GetComponent<ItemObject>();
			
			if (item == null){
				return;		
			}
			item.Collect();	
		}, playerCamera, playerTransform, 40f);
		
		if(Input.GetAxis("Mouse ScrollWheel") > 0f){
			itemHandler.DecreaseItemSelection();
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0f){
			itemHandler.IncreaseItemSelection();
		}
	}
#endregion

#region ControlFunctions
	public void LockMouse(){
		General.LockCursor();
		isMouseLocked = true;
	}
	
	public void UnlockMouse(){
		General.UnlockCursor();
		isMouseLocked = false;
	}
	
	public void PauseSwitch(){
		if(isGamePaused){
			LockMouse();
			Time.timeScale = 1f;
			isGamePaused = false;
			pauseMenu.SetActive(false);
		}
		else{
			UnlockMouse();
			Time.timeScale = 0f;
			isGamePaused = true;
			pauseMenu.SetActive(true);
		}
	}
	
	private void UIObjectToggle(UITextObject obj, InputAction action, Func<bool, float, IEnumerator> function){
		if(!Singleton<InputManager>.Instance.GetActionKeyDown(action) || isGameOver || isGamePaused){
			return;
		}
		
		if(obj.isMoving){
			return;			
		}
		
		obj.isInState = !obj.isInState;
		StartCoroutine(function(obj.isInState, 0.45f));
	}
#endregion
	
#region GameStateFunction
	private void ActivateGame(){
		hasGameStarted = true;
		// baldi.SetActive(true);
		entrance.wallAction(EntranceScript.wallState.lowerWall);
	}
	
	private void ActivateFinaleMode(){
		isGameFinale = true;
		entrance.wallAction(EntranceScript.wallState.raiseWall);
	}
	
	public void ExitGame(){
		// Time.timeScale = 1f; // why
		SceneManager.LoadScene(exitGameScene);
	}
#endregion
	
#region GameOverFunctions
	public void GameOver(CreatureType death){
		TurnOffCameras();
		soundHandler.StopMusic();
		playerHUD.SetActive(false);
		
		PlayerPrefs.SetString("DeathCause", death.ToString());
		PlayerPrefs.Save();
		
		SceneManager.LoadSceneAsync(gameOverScene);
	}
	
	private void TurnOffCameras(){
		foreach(Camera cam in sceneCameras){
			cam.farClipPlane = 0f;
		}
	}
#endregion
	
#region BoxFunctions
	// this block of code makes me go insane every day.
 	private string UpdateBoxCount(){
		return $"{General.ReadOutNumber(collectedBoxes)} out of {General.ReadOutNumber(maxBoxes)} boxes.";
	}
	
	public IEnumerator MoveRoomInformation(bool putInterfaceUp, float time = 0.45f){
		if(!isInsideRoomTrigger){
			yield break;
		}
		
		Vector2 target = putInterfaceUp ? new Vector2(-100, 50) : new Vector2(-100, -50);
		roomInformation.objText.text = GetFormattedRoomName();
		yield return roomInformation.MoveObject(target, CommonMath.EaseOutCubic, time);
	}
	
	public IEnumerator MoveBoxInformation(bool putInterfaceUp, float time = 0.45f){
		if(!isHoldingBox){
			yield break;
		}
		
		Vector2 target = putInterfaceUp ? new Vector2(-100, -100) : new Vector2(100, -100);
		boxInformation.objText.text = currentBoxData.GetFormatted();
		yield return boxInformation.MoveObject(target, CommonMath.EaseOutCubic, time);
	}
	
	public string GetFormattedRoomName(){
		return $"You are in the {General.GetFormattedColor(roomColor).ToLower()} room";
	}
	
	private void BobBox(){
		float targetAmount = playerScript.isMoving ? 1f : 0f;
		boxViewmodelBobAmount = Mathf.Lerp(boxViewmodelBobAmount, targetAmount, Time.deltaTime * 8f);

		if(playerScript.isMoving){
			boxViewmodelBobTime += Time.deltaTime * boxViewmodelBobSpeed;
			
			if(!boxViewmodel.isInState){
				boxViewmodel.isInState = true;
			}
		}
		else if(boxViewmodel.isInState){
			boxViewmodel.isInState = false;
			boxViewmodelBobTime = 0f;
		}

		float wave = (Mathf.Sin(boxViewmodelBobTime) + 1f) * 0.5f;

		Vector3 bobTarget = Vector3.Lerp(boxViewmodel.oldTransform, boxViewmodelFinalPoint, wave);
		boxViewmodel.obj.transform.localPosition = Vector3.Lerp(boxViewmodel.oldTransform, bobTarget, boxViewmodelBobAmount);
	}
		
	public void CollectBox(){
		if(isHoldingBox){
			return;
		}
		
		isHoldingBox = true;
		
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = Mathf.Min(playerScript.maxStamina, playerScript.stamina + (playerScript.maxStamina - playerScript.stamina) / 4f); // this feels stupid.
		}
		
		soundHandler.PlaySound(grabBoxSound, 0);
		boxViewmodel.obj.SetActive(true);
	}
	
	public void PutBoxInPlace(){
		if(!isHoldingBox){
			return;
		}
		
		collectedBoxes++;
		boxCounter.text = UpdateBoxCount();
		isHoldingBox = false;
		currentBoxData.ClearData();
		
		if(playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina / UnityEngine.Random.Range(0, 4); // i love rng
		}
		
		if(boxInformation.isInState){
			StartCoroutine(MoveBoxInformation(false));
			boxInformation.objText.text = null;
		}
		
		soundHandler.PlaySound(dropBoxSound, 0);		
		boxViewmodel.obj.SetActive(false);
		
		if(!hasGameStarted){
			if(collectedBoxes > 1){
				ActivateGame();
			}
			
			return;
		}
		
		if(collectedBoxes >= maxBoxes){
			ActivateFinaleMode();
		}
	}
#endregion
}
