// main libraries
using System;
using System.Collections;

// unity libraries
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// my libraries
using TextLibrary;
using MathLibrary;
using GeneralLibrary;

public class GameControllerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;
	
	[Header("Player")]
	[SerializeField] private Transform playerTransform;
	[SerializeField] private Camera playerCamera;
	
	[Header("Baldi")]
	[SerializeField] private GameObject baldi;
	[SerializeField] private BaldiScript baldiScript;
	
	[Header("States")]
	public bool hasGameStarted = false;
	public bool isGameFinale = false;
	public bool isGameOver = false;
	public bool isDebugMode = false;
	public bool isMouseLocked =  true;
	public bool isGamePaused = false;
	
	[Header("Graphical Player Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	
	[Header("Box")]
	[SerializeField] private BoxData currentBoxData;
	[SerializeField] private TMP_Text boxCounter;
	[SerializeField] private GameObject boxGPIGameObject;
	[SerializeField] private Vector3 startPosition;
	[SerializeField] private Vector3 bobOffset;	
	[SerializeField] private float bobSpeed;
	[SerializeField] private float bobTime;
	[SerializeField] private float bobAmount;
	[SerializeField] private AudioClip grabBoxSound;
	[SerializeField] private AudioClip dropBoxSound;
	public bool isHoldingBox;	
	public int collectedBoxes = 0;
	public int maxBoxes = 9;
	
	[Header("Exit")]
	public int exitsReached;
	[SerializeField] private EntranceScript entrance;
	
	[Header("Scene Management")]
	[SerializeField] private Material gameOverSkybox;
	[SerializeField] private Color finaleColor;
	[SerializeField] private string exitGameScene;
	[SerializeField] private string gameOverScene;
	
	[Header("Audio")]
	[SerializeField] private SoundHandler soundHandler; 
	[SerializeField] private AudioClip[] musicTracks;
	
#region MainFunctions
	private void Start(){
		lockMouse();
		boxCounter.text = updateBoxCount();
		startPosition = boxGPIGameObject.transform.localPosition;
		
		boxGPIGameObject.SetActive(false);
		
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
		gActions.LockCursor();
		isMouseLocked = true;
	}
	
	public void unlockMouse(){
		gActions.UnlockCursor();
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
		entrance.wallAction(EntranceScript.wallState.lowerWall);
	}
	
	private void activateFinaleMode(){
		isGameFinale = true;
		entrance.wallAction(EntranceScript.wallState.raiseWall);
	}
	
	/*private void entranceState(EntranceScript.wallState currentWallState){
		foreach (EntranceScript entrance in entrances){
			entrance.wallAction(currentWallState);
		}
	}*/	
	
	public void exitReached(){
		exitsReached++;
		RenderSettings.ambientLight = finaleColor;
	}
	
	public void exitGame(){
		Time.timeScale = 1f;
		SceneManager.LoadScene(exitGameScene);
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
		return $"{cText.ReadOutNum(collectedBoxes, true)} out of {cText.ReadOutNum(maxBoxes)} boxes.";
	}

	private void bobUIBox(){
		float targetAmount = playerScript.isMoving ? 1f : 0f;
		float wave = (fMath.Sine(bobTime) + 1f) * 0.5f;		
		
		bobAmount = Mathf.Lerp(bobAmount, targetAmount, Time.deltaTime * 8f);
		
		if (playerScript.isMoving){
			bobTime += Time.deltaTime * bobSpeed;
		}
		
		Vector3 targetPos = Vector3.Lerp(startPosition, startPosition + bobOffset, wave);
		boxGPIGameObject.transform.localPosition = Vector3.Lerp(startPosition, targetPos, bobAmount);
	}
	
	public void collectBox(){
		if(isHoldingBox){
			return;
		}
		
		isHoldingBox = true;
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = Mathf.Min(playerScript.maxStamina, playerScript.stamina + (playerScript.maxStamina - playerScript.stamina) / 4f);
		}
		
		soundHandler.playSound(grabBoxSound, 0);
		boxGPIGameObject.SetActive(true);
	}
	
	public void putBoxInPlace(){
		if(!isHoldingBox){
			return;
		}
		
		collectedBoxes++;
		boxCounter.text = updateBoxCount();
		isHoldingBox = false;
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina;
		}		
		
		soundHandler.playSound(dropBoxSound, 0);		
		boxGPIGameObject.SetActive(true);
		
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
}
