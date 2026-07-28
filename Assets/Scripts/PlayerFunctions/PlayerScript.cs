using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FootstepSound{
	public SoundType soundType = SoundType.Concrete;
	public PhysicMaterial material;
	public AudioClip[] sounds;
	
	public enum SoundType{
		Wood,
		Vent,		
		Metal,
		MetalGrate,
		Tile,
		Concrete,
		Water
	}
}

public class PlayerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private CharacterFaceManager faceManager;
	
	[Header("Footsteps")]
	[SerializeField] private FootstepSound.SoundType currentFloorType = FootstepSound.SoundType.Concrete;
	[SerializeField] private FootstepSound[] footstepSounds;
	[SerializeField] private AudioSource footstepSource;
	[SerializeField] private float footstepDelayWalk;
	[SerializeField] private float footstepDelayRun;
	private float footstepTimer;
	
	[Header("Main")]
	[SerializeField] private CharacterController characterController;
	[SerializeField] private bool sensitivityActive;
	[SerializeField] private float sensitivity;	
	[SerializeField] private float mouseSensitivity = 100f;
	private Vector3 moveDirection;
	
	[Header("Walk/Run")]
	[SerializeField] private float playerSpeed;	
	[SerializeField] private float walkSpeed = 5f;
	[SerializeField] private float runSpeed = 8f;
	[SerializeField] public bool isMoving;
	private float mouseX;	
	private float verticalVelocity;	
	// private const float gravity = -9.81f;
	// why am i calculating gravity, the game doesn't need this
	
	[Header("Stamina")]
	[SerializeField] public float stamina;	
	[SerializeField] public float maxStamina = 100f;
	[SerializeField] private float staminaRate = 20f;
	[SerializeField] public bool isRunning;
	[SerializeField] private Slider staminaBar;
	
	[Header("Health")]
	[SerializeField] public float healthValue;	
	[SerializeField] public float maxHealthValue = 100f;
	[SerializeField] private float healthRate = 5f;
	[SerializeField] private Slider healthBar;
	public enum HealthAction{
		Damage,
		Regeneration,
		FullRegeneration,
		InstaKill
	}

#region MainFunctions
	private void Start(){
		if (PlayerPrefs.GetInt("AnalogMove") == 1){
			sensitivityActive = true;
		}
		
		mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity) * 3.14f;		
		
		stamina = maxStamina;
		healthValue = maxHealthValue;
		
		if (staminaBar != null){
			staminaBar.minValue = 0f;
			staminaBar.maxValue = 1f;
			staminaBar.value = 1f;
		}

		if (healthBar != null){
			healthBar.minValue = 0f;
			healthBar.maxValue = 1f;
			healthBar.value = 1f;
		}
	}
	
	private void Update(){
		isRunning = Singleton<InputManager>.Instance.GetActionKey(InputAction.Run);
		isMoving = GetMovementInput().sqrMagnitude > 0f;
		
		MouseMove();
		PlayerMove();
		HealthCheck();
		HandleFootsteps();
		StaminaCheck();
		
		if (characterController.velocity.sqrMagnitude > 0.01f){
			gameController.lockMouse();
		}
	}
	
	// todo: make this use tags instead of obj names
	private void OnTriggerEnter(Collider other){
		if (other.CompareTag("BadGuy") && !gameController.isDebugMode){
			//gameController.GameOver();
		}
	}	
#endregion

#region MovmentFunctions
	private void HandleFootsteps(){
		if (isMoving){
			footstepTimer -= Time.deltaTime;
			if (footstepTimer <= 0f){
				PlayFootstep();
				footstepTimer = isRunning ? footstepDelayRun : footstepDelayWalk;
			}
		}
		else{
			footstepTimer = 0f;
		}
	}

	private void PlayFootstep(){
		FootstepSound footstepSound = GetFootstepSound(currentFloorType);
		if (footstepSound == null || footstepSound.sounds.Length == 0){
			return;
		}
		
		soundHandler.PlaySound(footstepSound.sounds[UnityEngine.Random.Range(0, footstepSound.sounds.Length)], 2);
	}
	
	private void UpdateFloorType(){
		Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

		if (Physics.Raycast(ray, out RaycastHit hit, 2f)){
			PhysicMaterial material = hit.collider.sharedMaterial;

			foreach (FootstepSound footstepSound in footstepSounds){
				if (footstepSound.material == material){
					currentFloorType = footstepSound.soundType;
					return;
				}
			}
		}

		currentFloorType = FootstepSound.SoundType.Concrete;
	}
	
	private FootstepSound GetFootstepSound(FootstepSound.SoundType soundType){
		foreach (FootstepSound footstepSound in footstepSounds){
			if (footstepSound.soundType == soundType){
				return footstepSound;
			}
		}
		return null;
	}
	
	private void MouseMove(){
		float sensitivity = mouseSensitivity * 2f;
		
		mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		transform.Rotate(Vector3.up * mouseX);
	}
	
	private Vector3 GetMovementInput(){
		Vector3 input = Vector3.zero;
		
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveForward)){
			input += transform.forward;			
		}
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveBackward)){
			input -= transform.forward;			
		}
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveLeft)){
			input -= transform.right;			
		}
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveRight)){
			input += transform.right;			
		}
		
		return input.normalized;
	}
	
	// this has some bugs for sure
	private void PlayerMove(){
		float inputMagnitude = Mathf.Clamp01(GetMovementInput().magnitude);
		
		if (stamina > 0.1f & isRunning){
			playerSpeed = runSpeed;
			sensitivity = 1f;
		}
		else if(stamina >= 0 & isRunning){
			playerSpeed = walkSpeed / 1.5f;
			sensitivity = sensitivityActive ? inputMagnitude : 1f;
		}
		else{
			playerSpeed = walkSpeed;
			sensitivity = sensitivityActive ? inputMagnitude : 1f;
		}
		
		Vector3 horizontalMove = GetMovementInput() * playerSpeed * sensitivity;
		
		// removed this part. pointless calculation.
		
		/*
		if (characterController.isGrounded && verticalVelocity < 0f){
			verticalVelocity = -2f;
		}
		verticalVelocity += gravity * Time.deltaTime;
		finalMove.y = verticalVelocity;
		*/
		
		Vector3 finalMove = horizontalMove;		

		moveDirection = finalMove * Time.deltaTime;
		characterController.Move(moveDirection);
	}
	
	private void StaminaCheck(){
		if(isMoving & isRunning && stamina > 0.1f){
			stamina -= staminaRate * Time.deltaTime;
		}
		else if(stamina < maxStamina && isMoving || isRunning){
			stamina += staminaRate * Time.deltaTime / 2.5f;
		}
		else if(stamina < maxStamina && !isMoving || !isRunning){
			stamina += staminaRate * Time.deltaTime;
		}

		stamina = Mathf.Clamp(stamina, 0f, maxStamina);

		if (staminaBar != null){
			float target = stamina / maxStamina;
			staminaBar.value = Mathf.MoveTowards(staminaBar.value, target, Time.deltaTime * 5f);
		}
	}
#endregion

#region HealthHandler
	private void HealthCheck(){
		float regenThreshold = maxHealthValue * 0.4f;
		
		if (!isMoving && !isRunning && healthValue < regenThreshold){
			healthValue += (healthRate * 0.5f) * Time.deltaTime;
		}
		
		if(healthValue > 1f){
			
		}
		
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
		
		if (healthBar != null){
			float target = healthValue / maxHealthValue;
			healthBar.value = Mathf.MoveTowards(healthBar.value, target, Time.deltaTime * 5f);
		}
	}
	
	public void DoHealthAction(HealthAction action, float ammount){
		float absoluteAmmount = Mathf.Abs(ammount);
		float negativeAmmount = ammount * -1f;
		
		switch(action){
			case HealthAction.Damage:
				if(absoluteAmmount > healthValue){
					// gameController.GameOver();
					// canPlayerMove = true;
					DebugConsole.SendLog("Started game over inside PlayerScript.");
					return;
				}
				
				SetHealth(negativeAmmount);
				StartCoroutine(faceManager.TakeDamage(absoluteAmmount));
				break;
				
			case HealthAction.Regeneration:
				SetHealth(absoluteAmmount);
				break;
				
				
			case HealthAction.FullRegeneration:
				SetHealth(maxHealthValue - healthValue);
				break;
		}
	}
	
	private void SetHealth(float ammount){
		healthValue += ammount; // no 'absoluteAmmount' correction here because a+-b = a-b 
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue) 
	}
	
	public IEnumerator DoHealthRegeneration
	
	/*
	public void GetHealth(float value){
		healthValue += Mathf.Abs(value);
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
	}
	
	public void TakeDamage(float value){
		float absoluteOfValue = Mathf.Abs(value);
		
		faceManager.TakeDamage(absoluteOfValue);
		healthValue -= absoluteOfValue;
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
	}
	
	public void InstaKill(){
		
	}*/
#endregion
}
