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
	[SerializeField] private GameObject boxGPIGameObject;
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
		updateBoxCount();
		startPosition = boxGPITransform.localPosition;
		boxGPIGameObject.SetActive(false);
		
		soundHandler.loopMusic(musicTracks[0], 0);
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
		
		// bob
		if(!isGamePaused){
			bobUIBox();
		}
		
		// time
		if (!isGamePaused & Time.timeScale != 1f){
			Time.timeScale = 1f;
		}
		else{
			if (Time.timeScale != 0f){
				Time.timeScale = 0f;
			}
		}
		
		// game over
		if (isGameOver){
			gameOverState();
		}
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
		float gameOverDelay = 0.5f;
		Time.timeScale = 0f;
		
		/* this is wrong; the function for WaitDeltaTime in loop has been
		deprecated because of lag and heavy overhead for the c# compiler.
		*/
		
		gameOverDelay -= Time.unscaledDeltaTime * 0.5f;
		playerCamera.farClipPlane = gameOverDelay * 400f;
		
		RenderSettings.skybox = gameOverSkybox;
		StartCoroutine(hideHUD());
		
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
 	private void updateBoxCount(){
		boxCounter.text = collectedBoxes.ToString() + "/" + maxBoxes.ToString() + "Boxes";
	}
	
	private void bobUIBox(){
		if (!playerScript.isMoving){
			boxGPITransform.localPosition = Vector3.Lerp(boxGPITransform.localPosition, startPosition, Time.deltaTime * 8f);
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
		boxGPIGameObject.SetActive(true);
		updateBoxCount();
		
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina - 30f;
		}
	}
	
	public void putBoxInPlace(){
		soundHandler.playSound(grabBoxSound, 0);
		
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
