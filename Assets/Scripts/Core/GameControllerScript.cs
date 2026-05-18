using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private CursorControllerScript cursorController;
	[SerializeField] private PlayerScript playerScript;
	
	[Header("Player")]
	[SerializeField] private Transform playerTransform;
	[SerializeField] private Camera playerCamera;
	
	[Header("Baldi")]
	[SerializeField] private GameObject baldi;
	[SerializeField] public BaldiScript baldiScript;
	
	[Header("States")]
	[SerializeField] private bool hasGameStarted = false;
	[SerializeField] public bool isGameFinale = false;
	[SerializeField] public bool isGameOver = false;
	[SerializeField] public bool isDebugMode = false;
	[SerializeField] public bool isMouseLocked =  true;
	[SerializeField] private bool isGamePaused = false;
	
	[Header("Graphical Player Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	
	[Header("Box")]
	[SerializeField] private TMP_Text boxCounter;
	
	[SerializeField] private Transform boxGPITransform;
	[SerializeField] private GameObject[] boxGPIGameObject;
	[SerializeField] private float bobSpeed;
	[SerializeField] private Vector3 startPosition;
	[SerializeField] private Vector3 bobOffset;
	
	[SerializeField] private AudioClip grabBoxSound;
	[SerializeField] private AudioClip dropBoxSound;
	
	[SerializeField] public bool isHoldingBox;	
	[SerializeField] public int collectedBoxes = 0;
	[SerializeField] public int maxBoxes = 9;
	
	[Header("Exit")]
	[SerializeField] public int exitsReached;
	[SerializeField] private EntranceScript[] entrances;
	
	[Header("Scene Management")]
	[SerializeField] private Material gameOverSkybox;
	[SerializeField] private Color finaleColor;
	[SerializeField] private string exitGameScene;
	[SerializeField] private string gameOverScene;
	
	[Header("Audio")]
	[SerializeField] private SoundHandler soundHandler; 
	[SerializeField] private AudioClip[] musicTracks;
	
	/*
	AUDIO STACK ORDER:
		0 - music
		1 - general sound effects
		2 - player made sound effects
	*/
	
#region MainFunctions
	private void Start(){
		lockMouse();
		boxCounter.text = updateBoxCount();
		startPosition = boxGPITransform.localPosition;
		
		foreach (GameObject HUDobject in boxGPIGameObject){
			HUDobject.SetActive(false);
		}
		
		soundHandler.loopMusic(musicTracks[UnityEngine.Random.Range(0, musicTracks.Length)], 0);
	}
	
	private void Update(){
		// pause switch
		if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.PauseOrCancel) && !isGameOver){
			if (!isGamePaused){
				pauseGame();
			}
			else{
				unpauseGame();
			}
		}

		if (!isGamePaused & Time.timeScale != 1f){
			Time.timeScale = 1f;
		}
		else{
			if (Time.timeScale != 0f){
				Time.timeScale = 0f;
			}
		}
		
		bobUIBox();
		gameOverState();
	}
#endregion

#region MouseFunction
	public void lockMouse(){
		cursorController.LockCursor();
		isMouseLocked = true;
	}
	
	public void unlockMouse(){
		cursorController.UnlockCursor();
		isMouseLocked = false;
	}
#endregion
	
#region GameStateFunction
	public void pauseGame(){
		unlockMouse();
		Time.timeScale = 0f;
		isGamePaused = true;
		pauseMenu.SetActive(true);
	}
	
	public void unpauseGame(){
		Time.timeScale = 1f;
		isGamePaused = false;
		pauseMenu.SetActive(false);
		lockMouse();
	}
	
	private void activateGame(){
		hasGameStarted = true;
		baldi.SetActive(true);
		
		entranceState(EntranceScript.wallState.lowerWall);
	}
	
	private void activateFinaleMode(){
		isGameFinale = true;
		
		entranceState(EntranceScript.wallState.raiseWall);
	}
	
	private void entranceState(EntranceScript.wallState currentWallState){
		foreach (EntranceScript entrance in entrances){
			entrance.wallAction(currentWallState);
		}
	}	
	
	public void exitReached(){
		exitsReached++;
		RenderSettings.ambientLight = finaleColor;
	}
#endregion

#region GameOverFunctions
	private void gameOverState(){
		if(!isGameOver){
			return;
		}
		
		float gameOverDelay = 0.5f;
		bool hasGameOverInitialized = false;
		
		if (!hasGameOverInitialized){
			hasGameOverInitialized = true;
			Time.timeScale = 0f;
			RenderSettings.skybox = gameOverSkybox;
			StartCoroutine(hideHUD());
		}
		
		gameOverDelay -= Time.unscaledDeltaTime * 0.5f;
		playerCamera.farClipPlane = gameOverDelay * 400f;
		
		if (gameOverDelay <= 0f){
			Time.timeScale = 1f;
			SceneManager.LoadScene(gameOverScene);
		}
	}
	
	private IEnumerator hideHUD(){
		while (isGameOver){
			playerHUD.SetActive(false);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}
#endregion
	
#region NotebookFunctions
 	private string updateBoxCount(){
		// boxCounter.text = updateBoxCount;
		
		return $"{collectedBoxes} out of {maxBoxes} boxes";
	}
	
	private void bobUIBox(){
		if (!playerScript.isMoving){
			boxGPITransform.localPosition = Vector3.Lerp(boxGPITransform.localPosition, startPosition, Time.deltaTime * 8f); // larppppp
			return;
		}

		float wave = (Mathf.Sin(Time.time * bobSpeed) + 1f) / 2f;

		boxGPITransform.localPosition = Vector3.Lerp(startPosition, startPosition + bobOffset, wave);
	}
	
	public void collectBox(){
		if(isHoldingBox){
			return;
		}
		
		isHoldingBox = true;
		soundHandler.playSound(grabBoxSound, 0);
		updateBoxCount();
		foreach (GameObject HUDobject in boxGPIGameObject){
			HUDobject.SetActive(true);
		}		
		
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = Mathf.Min(playerScript.maxStamina, playerScript.stamina + (playerScript.maxStamina - playerScript.stamina) / 4f);
		}
	}
	
	public void putBoxInPlace(){
		if(!isHoldingBox){
			return;
		}
		
		isHoldingBox = false;		
		soundHandler.playSound(dropBoxSound, 0);
		boxCounter.text = updateBoxCount();
		foreach (GameObject HUDobject in boxGPIGameObject){
			HUDobject.SetActive(true);
		}		
		
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina;
		}
		
		if(!hasGameStarted){
			if(collectedBoxes > 1){
				activateGame();
			}
		}
		else if(hasGameStarted){
			if (collectedBoxes >= maxBoxes){
				activateFinaleMode();
			}
		}
	}
#endregion
	
#region MiscFunctions
	public void exitGame(){
		Time.timeScale = 1f;
		SceneManager.LoadScene(exitGameScene);
	}
	
	public void getAngry(float value){ // are we deadass mystman12
		if (!hasGameStarted){
			activateGame();
		}
		
		baldiScript.GetAngry(value);
	}
#endregion
}
