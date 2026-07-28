```csharp id="q8x2vn"
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TextLibrary;
using MathLibrary;
using GeneralLibrary;

public class GameControllerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;
	
	[Header("Player")]
	public Transform playerTransform;
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
	public enum DeathType{
		Skinwalker,
		StarCreature,
		Bleeding
	};
	
	[Header("Graphical Player Interface")]
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject playerHUD;
	
	[Header("Box")]
	public BoxData currentBoxData;
	[SerializeField] private TMP_Text boxCounter;
	[SerializeField] private TMP_Text boxInfo;
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
	
	[Header("Physics Object")]
	[SerializeField] private float pickupDistance = 3f;
	[SerializeField] private float holdDistance = 2f;
	[SerializeField] private float moveForce = 20f;
	[SerializeField] private float maxVelocity = 10f;
	[SerializeField] private float throwForce = 10f;
	private PhysicsObject heldObject;
	private Rigidbody heldRigidbody;
	
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
		currentBoxData.ClearData();
		// DebugConsole.StartConsole();
		boxCounter.text = UpdateBoxCount();
		startPosition = boxGPIGameObject.transform.localPosition;
		boxGPIGameObject.SetActive(false);
		soundHandler.LoopMusic(musicTracks[UnityEngine.Random.Range(0, musicTracks.Length)], 0);
	}
	
	private void Update(){
		// pause switch
		if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.PauseOrCancel) && !isGameOver){
			if (!isGamePaused){
				PauseGame();
			}
			else{
				UnpauseGame();
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
		
		if(isGamePaused){
			return;
		}
		
		BobBox();
		BoxObjectHandler();
		ObjectPickup();
	}
#endregion

#region MouseFunction
	public void lockMouse(){
		Actions.LockCursor();
		isMouseLocked = true;
	}
	
	public void unlockMouse(){
		Actions.UnlockCursor();
		isMouseLocked = false;
	}
#endregion
	
#region GameStateFunction
	public void PauseGame(){
		unlockMouse();
		Time.timeScale = 0f;
		isGamePaused = true;
		pauseMenu.SetActive(true);
	}
	
	public void UnpauseGame(){
		Time.timeScale = 1f;
		isGamePaused = false;
		pauseMenu.SetActive(false);
		lockMouse();
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
	
	public void ExitReached(){
		exitsReached++;
		RenderSettings.ambientLight = finaleColor;
	}
	
	public void ExitGame(){
		Time.timeScale = 1f;
		SceneManager.LoadScene(exitGameScene);
	}
	
	private void BoxObjectHandler(){
		if (!Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact)){
			return;			
		}
		
		if (Time.timeScale == 0f){
			return;
		}

		Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

		if (!Physics.Raycast(ray, out RaycastHit hit)){
			return;	
		}	
		
		if (Vector3.Distance(playerTransform.position, hit.transform.position) > 10f){
			return;			
		}
		
		BoxScript box = hit.transform.GetComponent<BoxScript>();
		
		if (box == null){
			return;			
		}

		box.Collect();
	}
#endregion

#region PhysicsObjectww
	private void ObjectPickup(){
		if (heldObject != null){
			HandleHeldObject();
			
			if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.Interact)){
				DropObject();
			}
			
			return;
		}
		
		if (Time.timeScale == 0f){
			return;
		}
		if (!Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.Interact)){
			return;
		}

		Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

		if (!Physics.Raycast(ray, out RaycastHit hit, pickupDistance)){
			return;	
		}
		if (Vector3.Distance(playerTransform.position, hit.transform.position) > pickupDistance){
			return;			
		}
		
		PhysicsObject physicsObject = hit.collider.GetComponentInParent<PhysicsObject>();
		
		if (physicsObject == null){
			return;
		}
		
		Rigidbody rigidbody = physicsObject.GetRigidbody();
		
		if (rigidbody == null){
			return;
		}
		
		heldObject = physicsObject;
		heldRigidbody = rigidbody;
		
		heldObject.OnObjectPickup();
	}
	
	private void HandleHeldObject(){
		if (heldObject == null){
			return;
		}
		
		if (heldRigidbody == null){
			return;
		}
		
		Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
		
		float objectWeight = heldObject.GetObjectWeight();
		float weightMultiplier = 5f / objectWeight;
		float finalForce = moveForce * weightMultiplier;
		
		heldObject.MoveObject(targetPosition, finalForce);
		LimitHeldObjectVelocity();
	}
	
	private void LimitHeldObjectVelocity(){
		if (heldRigidbody == null){
			return;
		}
		
		if (heldRigidbody.linearVelocity.magnitude > maxVelocity){
			heldRigidbody.linearVelocity = heldRigidbody.linearVelocity.normalized * maxVelocity;
		}
	}
	
	private void DropObject(){
		if (heldObject == null){
			return;
		}
		
		heldObject.OnObjectDrop();
		ClearHeldObject();
	}
	
	private void ThrowObject(){
		if (heldObject == null){
			return;
		}
		
		Vector3 throwDirection = playerCamera.transform.forward;
		heldObject.ThrowObject(throwDirection, throwForce);
		ClearHeldObject();
	}
	
	private void ClearHeldObject(){
		heldObject = null;
		heldRigidbody = null;
	}
#endregion
	
#region GameOverFunctions
	public void GameOver(DeathType death){
		isGameOver = true;
		
		switch(death){
			case DeathType.StarCreature:
				break;
			
			case DeathType.Skinwalker:
				break;
			
			case DeathType.Bleeding:
				break;
		}
	}
	
	private IEnumerator HideHUD(){
		while (isGameOver){
			playerHUD.SetActive(false);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}
#endregion
	
#region NotebookFunctions
 	private string UpdateBoxCount(){		
		return $"{TextLib.ReadOutNum(collectedBoxes)} out of {TextLib.ReadOutNum(maxBoxes)} boxes.";
	}
	
	private void PullUpBoxStats(){
		/*boxInfo.text = 	$"{currentBoxData.GetFormattedData()}\n" +
			$"Date of birth: {Date.GetDate(birthday)}\n"*/
	}

	private void BobBox(){
		float targetAmount = playerScript.isMoving ? 1f : 0f;
		float wave = (Mathf.Sin(bobTime) + 1f) * 0.5f;		
		
		bobAmount = Mathf.Lerp(bobAmount, targetAmount,Time.deltaTime * 8f);
		
		if (playerScript.isMoving){
			bobTime += Time.deltaTime * bobSpeed;
		}
		
		Vector3 targetPos = Vector3.Lerp(startPosition, startPosition + bobOffset, wave);
		boxGPIGameObject.transform.localPosition = Vector3.Lerp(startPosition, targetPos, bobAmount);
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
		boxGPIGameObject.SetActive(true);
	}
	
	public void PutBoxInPlace(){
		if(!isHoldingBox){
			return;
		}
		
		collectedBoxes++;
		boxCounter.text = UpdateBoxCount();
		isHoldingBox = false;
		currentBoxData.ClearData();
		
		if (playerScript.stamina < playerScript.maxStamina){
			playerScript.stamina = playerScript.maxStamina;
		}		
		
		soundHandler.PlaySound(dropBoxSound, 0);		
		boxGPIGameObject.SetActive(false);
		
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
```
