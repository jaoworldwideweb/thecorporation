using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GeneralLibrary;
using MathLibrary;
using GameLibrary;

[System.Serializable]
public class FilmCamera{
	public Transform camera;
	public Transform light;
}

// korrekt schnitzel!!!

public class PlayerScript : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private CharacterFaceManager faceManager;
	
	[Header("Footsteps")]
	[SerializeField] private FootstepSound[] footstepSounds;
	[SerializeField] private float footstepDelay;
	private float footstepTimer;
	
	[Header("Main")]
	[SerializeField] private CharacterController characterController;
	[SerializeField] private FilmCamera[] cameras;
	[SerializeField] private float mouseSensitivity = 100f;
	[SerializeField] private float walkSpeed = 15f;
	[HideInInspector] public bool isMoving = false;
	[HideInInspector] public bool canPlayerMove = true;
	[HideInInspector] public bool isDrinking = false;
	private float rotation;
	private float playerSpeed;	
	private float verticalVelocity;
	private const float GRAVITY = -9;
	
	[Header("Viewmodel")]
	public FullObject boxViewmodel;

	[Header("Stamina")]
	public float stamina;
	public float maxStamina = 100f;
	[SerializeField] private float staminaRate = 20f;
	[SerializeField] private Slider staminaBar;
	[HideInInspector] public bool isRunning = false;
	
	[Header("Health")]
	public float health;
	public float maxHealth = 100f;
	[SerializeField] private float healthRate = 5f;
	[SerializeField] private Slider healthBar;
	[HideInInspector] public bool isBleeding = false;
#endregion

#region MainFunctions
	private void Start(){
		mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
		stamina = maxStamina;
		health = maxHealth;
		
		boxViewmodel.obj.SetActive(false);
		boxViewmodel.SetCachedPosition();
		// i love hardcoding sometimes <3
		StartCoroutine(BobBoxViewmodel(boxViewmodel, new Vector3(0f, 0.25f, 0f), 2f, 2f, HighMath.PI));
	}
	
	private void Update(){
		isRunning = Singleton<InputManager>.Instance.GetActionKey(InputAction.Run);
		isMoving = GetMovementInput().sqrMagnitude > 0f;
		
		if (characterController.velocity.sqrMagnitude > 0.01f){
			gameController.LockMouse();
		}
		
		if(!canPlayerMove){
			return;
		}
		
		// visual
		MoveCamera();
		FlashlightMove(cameras[0], 9f);
		FlashlightMove(cameras[1], 9f);
		
		// movement
		PlayerMove();
		StaminaCheck();
		HandleFootsteps();
		
		// misc
		HealthCheck();
	}	
#endregion

#region FootstepFunctions
	private void HandleFootsteps(){
		if(isMoving && !gameController.isGamePaused){
			footstepTimer -= Time.deltaTime;
						
			if(footstepTimer <= 0f){
				PlayFootstep();
				footstepTimer = isRunning ? footstepDelay / 1.5f : footstepDelay;
			}
			
			return;
		}
		
		footstepTimer = 0f;
	}
	
	private void PlayFootstep(){
		PhysicMaterial material = GetFloorMaterial();
		FootstepSound footstepSound = GetFootstepSound(material);
		
		soundHandler.PlaySound(footstepSound.sounds[UnityEngine.Random.Range(0, footstepSound.sounds.Length)]);
	}

	private PhysicMaterial GetFloorMaterial() {
		Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
		
		if(Physics.Raycast(ray, out RaycastHit hit, 2f)){
			return hit.collider.sharedMaterial;			
		}
		
		return null;
	}
	
	private FootstepSound GetFootstepSound(PhysicMaterial material) {
		foreach(FootstepSound footstepSound in footstepSounds) {
			if(footstepSound.material == material){
				return footstepSound;				
			}
		}
		
		return null;
	}
#endregion

#region MovementFunctions
	private void MoveCamera(){
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		
		rotation -= mouseY;
		rotation = Mathf.Clamp(rotation, -90f, 90f);
		
		cameras[0].camera.localRotation = Quaternion.Euler(rotation, 0f, 0f);
		transform.Rotate(0f, mouseX, 0f);
	}
	
	private IEnumerator BobBoxViewmodel(FullObject obj, Vector3 targetPosition, float amount, float speed, float time){
		while (true){
			float targetAmount = isMoving ? 1f : 0f;
			float bobAmount = Mathf.Lerp(amount, targetAmount, Time.deltaTime * 8f); // why 8f? i forgot.
			
			if(isMoving && gameController.isHoldingBox){
				time += Time.deltaTime * speed;
				
				float wave = (Mathf.Sin(time) + 1f) * 0.5f;
				
				Vector3 bobTarget = Vector3.Lerp(boxViewmodel.cachedPosition, targetPosition, wave);
				boxViewmodel.obj.transform.localPosition = Vector3.Lerp(boxViewmodel.cachedPosition, bobTarget, amount);
			}
			
			yield return null;
		}
	}
	
	private void FlashlightMove(FilmCamera filmCamera, float smoothness){
		Transform cameraTransform = filmCamera.camera;
		Transform lightTransform = filmCamera.light;
		float t = smoothness * Time.deltaTime;
		
		lightTransform.position = Vector3.Lerp(lightTransform.position, cameraTransform.position, t);
		lightTransform.rotation = Quaternion.Slerp(lightTransform.rotation, cameraTransform.rotation, t);
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
	
	private void PlayerMove(){
		if (isRunning){
			if (stamina > 0.1f){
				playerSpeed = walkSpeed * 2f;				
			}
			else{
				playerSpeed = walkSpeed / 1.5f;				
			}
		}
		else{
			playerSpeed = walkSpeed;
		}
		
		Vector3 finalMove = GetMovementInput() * playerSpeed;
		
		if (characterController.isGrounded && verticalVelocity < 0f){
			verticalVelocity = -2f;			
		}
		
		verticalVelocity += GRAVITY * Time.deltaTime;
		finalMove.y = verticalVelocity;
		
		characterController.Move(finalMove * Time.deltaTime);
	}
	
	// this finally feels clean.	
	private void StaminaCheck(){
		if (isMoving && isRunning){
			stamina -= staminaRate * Time.deltaTime;
		}
		else if(stamina < maxStamina){
			float regenerationRate = (isMoving || isRunning) ? 0.4f : 1f;
			stamina += staminaRate * regenerationRate * Time.deltaTime;
		}
		
		stamina = Mathf.Clamp(stamina, 0f, maxStamina);

		if (staminaBar != null){
			float target = stamina / maxStamina;
			staminaBar.value = Mathf.MoveTowards(staminaBar.value, target, Time.deltaTime * 5f);
		}
	}
#endregion

#region HealthFunctions
	private void HealthCheck(){
		float regenerationThreshold = maxHealth * 0.4f;
		
		if(health < regenerationThreshold){
			float regenerationRate = (isMoving || isRunning) ?  0.5f : 1f;
			health += (healthRate * regenerationRate) * Time.deltaTime;
		}
		
		health = Mathf.Clamp(health, 0f, maxHealth);
		
		if (healthBar != null){
			float target = health / maxHealth;
			healthBar.value = Mathf.MoveTowards(healthBar.value, target, Time.deltaTime * 5f);
		}
	}
	
	public void DoHealthAction(HealthAction action, float ammount, CreatureType hit = CreatureType.None){
		float absoluteAmmount = Mathf.Abs(ammount);
		float negativeAmmount = ammount;
		bool isAmmountNull = ammount == 0f;
		
		if(isAmmountNull){
			return;
		}
		
		if(hit == CreatureType.None & action == HealthAction.Damage){
			return;
		}
		
		switch(action){
			case HealthAction.Damage:
				if(hit == CreatureType.None){
					return;
				}
						
				if(absoluteAmmount > health){
					Kill(hit);
					return;
				}
				
				AddToHealth(negativeAmmount);
				faceManager.TakeDamage(absoluteAmmount);
				
				if(hit != CreatureType.StarCreature){
					return;
				}
				
				Bleed(UnityEngine.Random.Range(0f, 8f));
				break;
				
			case HealthAction.Regeneration:
				AddToHealth(absoluteAmmount);
				break;
		}
	}
	
	private void AddToHealth(float ammount){
		health += ammount;
		health = Mathf.Clamp(health, 0f, maxHealth);
	}
	
	public void Kill(CreatureType hit){
		canPlayerMove = false;
		gameController.GameOver(hit);
	}
	
	public void Bleed(float time){
		StartCoroutine(ApplyBleeding(UnityEngine.Random.Range(1.5f, 6f)));
	}
	
	private IEnumerator ApplyBleeding(float time){
		if(isBleeding || gameController.isGameOver){
			yield break;
		}
		
		isBleeding = true;
		float elapsedTime = time;
		float damage = UnityEngine.Random.Range(1.5f, 4.5f);
		
		while(elapsedTime > 0f){
			float damageRate = isRunning ? 2f : 1f;
			health -= (damage * damageRate) * Time.deltaTime;
			elapsedTime -= Time.deltaTime;
		}
		
		isBleeding = false;
	}
#endregion
}
