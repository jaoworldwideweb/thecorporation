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
	public Camera playerCamera;
	[SerializeField] private Camera[] sceneCameras;
	
	[Header("NPCs")]
	[SerializeField] private GameObject[] npcObjects;
	
	// game states
	[HideInInspector] public bool hasGameStarted = false;
	[HideInInspector] public bool isGameFinale = false;
	[HideInInspector] public bool isGameOver = false;
	[HideInInspector] public bool isDebugMode = false;
	[HideInInspector] public bool isMouseLocked = true;
	[HideInInspector] public bool isGamePaused = false;
	[HideInInspector] public bool isInsideRoomTrigger = false;
	[HideInInspector] public bool isInteractingWithCharacter = false;
	
	[Header("User Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	[SerializeField] private RenderTexture cameraOutput;
	
	[Header("Character Interactions")]
	[SerializeField] private UserInterfaceTextObject textOutput;
	[SerializeField] private TMP_Text characterName;
	[SerializeField] private Image facePhoto;
	
	[Header("Box")]
	[SerializeField] private TMP_Text boxCounter;
	public UserInterfaceTextObject boxInformation;
	public UserInterfaceTextObject roomInformation;
	public int maxBoxes { get; private set; }
	[HideInInspector] public int collectedBoxes { get; private set; }
	[HideInInspector] public Box currentBox;
	[HideInInspector] public BoxColor roomColor = BoxColor.Red;
	[HideInInspector] public bool isHoldingBox = false;	
	
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
	#if UNITY_STANDALONE && !UNITY_EDITOR
		bool isLow = SaveData.GetBool("IS_LOW")
		
		int monitorWidth = isLow ? Screen.currentResolution.width / 2 : Screen.currentResolution.width;
		int monitorHeight = isLow ? Screen.currentResolution.height / 2 : Screen.currentResolution.height;
		
		UserInterface.ResizeRenderTexture(new dint(monitorWidth, monitorHeight), cameraOutput, playerCamera);
	#endif
		
		LockMouse();
		currentBox.Clear();
		boxCounter.text = UpdateBoxCount();
		
		playerScript.boxViewmodel.SetCachedPosition();
		boxInformation.Initiate();
		textOutput.Initiate();
		
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
		
		// input
		General.DoActionFromInput(itemHandler.SetItemSelection, InputAction.Slot0, 0);
		General.DoActionFromInput(itemHandler.SetItemSelection, InputAction.Slot1, 1);
		General.DoActionFromInput(itemHandler.UseItem, InputAction.UseItem);
		
		PanelToggle(() => boxInformation.tmpText.text = currentBox.GetFormatted(), isHoldingBox, boxInformation, InputAction.Tab);
		PanelToggle(() => roomInformation.tmpText.text = GetFormattedRoomName(), isInsideRoomTrigger, roomInformation, InputAction.Q);
		
		// raycast
		General.DoRaycastForObject(hit =>{
			BoxScript box = hit.transform.GetComponent<BoxScript>();
			
			if(box == null){
				return;			
			}
			box.Collect();			
		}, playerCamera, playerTransform, 40f);
		
		General.DoRaycastForObject(hit =>{
			ItemObject item = hit.transform.GetComponent<ItemObject>();
			
			if(item == null){
				return;		
			}
			item.Collect();	
		}, playerCamera, playerTransform, 40f);
		
		General.DoRaycastForObject(hit =>{
			CharacterNode character = hit.transform.GetComponent<CharacterNode>();
			
			if(character == null){
				return;		
			}
			
			
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
	
	public void ExitGame() => SceneManager.LoadScene(exitGameScene);
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

#region CharacterFunctions
	/*public void InteractWithCharacter(){
		StartCoroutine();
	}*/
	
	private IEnumerator ICharacterInteract(InteractableCharacter character){
		isInteractingWithCharacter = true;
		
		facePhoto.sprite = character.GetPhoto();
		
		IShowCharacterInteractPanel(Direction.Up);
		
		float charactersPerSecond = character.GetCharactersPerSecond();
		
		yield return ITypeText(character.GetName(), characterName, charactersPerSecond * 2f);
		yield return ITypeText(character.GetSmallTalk(), textOutput.tmpText, charactersPerSecond);
		yield return General.IWaitUntilInput(InputAction.CloseInteraction);
		
		IShowCharacterInteractPanel(Direction.Down);
		
		isInteractingWithCharacter = false;
	}
	
	private IEnumerator IShowCharacterInteractPanel(Direction direction){
		Vector2 target = textOutput.GetDirection(direction);
		bool setActive = direction == Direction.Up ? true : false;
		
		textOutput.obj.SetActive(setActive);
		yield return textOutput.MoveObject(target, CommonMath.EaseOutCubic, 0.45f);
	}
	
	private IEnumerator ITypeText(string text, TMP_Text output, float charactersPerSecond = 5f){
		for(int i = 0; i < text.Length; i++){
			textOutput.tmpText.text += text[i];
			yield return new WaitForSeconds(1f / charactersPerSecond); 
		}
	}
#endregion

#region BoxFunctions
 	private string UpdateBoxCount() => $"{General.ReadOutNumber(collectedBoxes)} out of {General.ReadOutNumber(maxBoxes)} boxes.";
	public string GetFormattedRoomName() => $"You are in the {General.GetFormattedColor(roomColor).ToLower()} room";
	
	public void PanelToggle(Action function, bool check, UserInterfaceTextObject obj, InputAction input, float time = 0.45f){
		if (!Singleton<InputManager>.Instance.GetActionKeyDown(input) || isGameOver || isGamePaused){
			return;
		}
		
		if (obj.isMoving){
			return;			
		}
		
		obj.state = obj.state == ObjectState.Showing ? ObjectState.Hidden : ObjectState.Showing;
		Direction direction = obj.state == ObjectState.Showing ? Direction.Up : Direction.Down;
		StartCoroutine(IMovePanel(function, check, direction, obj, time));
	}
	
	public IEnumerator IMovePanel(Action function, bool check, Direction direction, UserInterfaceTextObject obj, float time = 0.45f){
		if (!check){
			yield break;
		}
		
		Vector2 target = obj.GetDirection(direction);
		function();
		yield return obj.MoveObject(target, CommonMath.EaseOutCubic, time);
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
		playerScript.boxViewmodel.obj.SetActive(true);
	}
	
	public void PutBoxInPlace(){
		if(!isHoldingBox){
			return;
		}
		
		collectedBoxes++;
		boxCounter.text = UpdateBoxCount();
		isHoldingBox = false;
		currentBox.Clear();
		
		if(playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina / UnityEngine.Random.Range(0, 4); // i love rng
		}
		
		if(boxInformation.state == ObjectState.Showing){
			StartCoroutine(
				IMovePanel(
					() => boxInformation.tmpText.text = currentBox.GetFormatted(),
					isHoldingBox,
					Direction.Down,
					boxInformation
				)
			);
			
			boxInformation.tmpText.text = null;
		}
		
		soundHandler.PlaySound(dropBoxSound, 0);		
		playerScript.boxViewmodel.obj.SetActive(false);
		
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
