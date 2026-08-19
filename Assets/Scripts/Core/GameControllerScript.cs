using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TextLibrary;
using MathLibrary;
using GeneralLibrary;
using GameLibrary;

public class GameControllerScript : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;
	
	[Header("Player")]
	public Transform playerTransform;
	[SerializeField] private Camera[] sceneCameras;
	[SerializeField] private Camera playerCamera;
	
	[Header("Baldi")]
	[SerializeField] private GameObject baldi;
	[SerializeField] private BaldiScript baldiScript;
	
	[Header("States")]
	public bool hasGameStarted = false;
	public bool isGameFinale = false;
	public bool isGameOver = false;
	public bool isDebugMode = false;
	public bool isMouseLocked = true;
	public bool isGamePaused = false;
	public bool isInsideRoomTrigger = false;
	
	[Header("Graphical Player Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	
	[Header("Box")]
	public BoxData currentBoxData;
	[SerializeField] private UITextObject boxInformation;
	[SerializeField] private UITextObject roomInformation;
	[SerializeField] private FullObject boxViewmodel;	
	
	[SerializeField] private TMP_Text boxCounter;
					 public bool isHoldingBox = false;
					 public int collectedBoxes = 0;
					 public int maxBoxes = 9;
	[SerializeField] private Vector3 boxViewmodelFinalPoint;
	[SerializeField] private float boxViewmodelBobSpeed;
					 private float boxViewmodelBobTime;
					 private float boxViewmodelBobAmount;
	[HideInInspector] public BoxColor roomColor = BoxColor.Red;
	
	[Header("Game Over")]
	[SerializeField] private Image gameOverRender;
	[SerializeField] private Creature[] creatures;
	
	[Header("Exit")]
	[HideInInspector] public int exitsReached;
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
		UIObjectToggle(boxInformation, InputAction.Tab, MoveBoxInformation);
		UIObjectToggle(roomInformation, InputAction.Q, MoveRoomInformation);
		
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
		
		// raycast stuff
		General.DoRaycastForObject(hit =>{
			BoxScript boxViewmodel = hit.transform.GetComponent<BoxScript>();
			
			if (boxViewmodel == null){
				return;			
			}
			boxViewmodel.Collect();			
		}, playerCamera, playerTransform, 0);
		
		General.DoRaycastForObject(hit =>{
			ItemObject item = hit.transform.GetComponent<ItemObject>();
			
			if (item == null){
				return;		
			}
			item.Collect();	
		}, playerCamera, playerTransform, 0);
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
	
	private void PauseSwitch(){
		if(isGamePaused){
			UnpauseGame();
		}
		else{
			PauseGame();
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
	public void PauseGame(){
		UnlockMouse();
		Time.timeScale = 0f;
		isGamePaused = true;
		pauseMenu.SetActive(true);
	}
	
	public void UnpauseGame(){
		Time.timeScale = 1f;
		isGamePaused = false;
		pauseMenu.SetActive(false);
		LockMouse();
	}
	
	private void ActivateGame(){
		hasGameStarted = true;
		baldi.SetActive(true);
		entrance.wallAction(EntranceScript.wallState.lowerWall);
	}
	
	private void ActivateFinaleMode(){
		isGameFinale = true;
		entrance.wallAction(EntranceScript.wallState.raiseWall);
	}
	
	public void ExitReached(){}
	
	public void ExitGame(){
		// Time.timeScale = 1f;
		SceneManager.LoadScene(exitGameScene);
	}
#endregion

#region Interactions
	private void BoxObjectHandler(){
		/*if (!Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) || Time.timeScale == 0f){
			return;			
		}
		
		Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

		if (!Physics.Raycast(ray, out RaycastHit hit)){
			return;	
		}
		if (Vector3.Distance(playerTransform.position, hit.transform.position) > 10f){
			return;			
		}*/
		
		General.DoRaycastForObject(hit =>{
			BoxScript boxViewmodel = hit.transform.GetComponent<BoxScript>();
			
			if (boxViewmodel == null){
				return;			
			}
			boxViewmodel.Collect();			
		}, playerCamera, playerTransform, 0);
		

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
		return $"{TextLib.ReadOutNum(collectedBoxes)} out of {TextLib.ReadOutNum(maxBoxes)} boxes.";
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
		return $"You are in the {roomColor.ToString().ToLower()} room";
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
			playerScript.stamina = Mathf.Min(playerScript.maxStamina,playerScript.stamina +(playerScript.maxStamina - playerScript.stamina) / 4f); // this feels stupid.
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
			playerScript.stamina = playerScript.maxStamina;
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
		}
		else if(hasGameStarted){
			if (collectedBoxes >= maxBoxes){
				ActivateFinaleMode();
			}
		}
	}
#endregion
}
