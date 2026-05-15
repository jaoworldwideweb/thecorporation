using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	
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
	private const float gravity = -9.81f;	
	
	[Header("Stamina")]
	[SerializeField] public float stamina;	
	[SerializeField] public float maxStamina = 100f;
	[SerializeField] private float staminaRate = 20f;
	[SerializeField] private bool isRunning;
	[SerializeField] private Slider staminaBar;
	
	[Header("Health")]
	[SerializeField] public float healthValue;	
	[SerializeField] public float maxHealthValue = 100f;
	[SerializeField] private float healthRate = 5f;
	[SerializeField] private Slider healthBar;
	
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
		isMoving = getMovementInput().sqrMagnitude > 0f;
		
		mouseMove();
		playerMove();
		healthCheck();
		staminaCheck();
		
		if (characterController.velocity.sqrMagnitude > 0.01f){
			gameController.lockMouse();
		}
	}
	
	private void mouseMove(){
		float sensitivity = mouseSensitivity * 2f;
		
		mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		transform.Rotate(Vector3.up * mouseX);
	}
	
	private Vector3 getMovementInput(){
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
		
	private void playerMove(){
		float inputMagnitude = Mathf.Clamp01(getMovementInput().magnitude);

		// speed logic
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

		// horizontal movement
		Vector3 horizontalMove = getMovementInput() * playerSpeed * sensitivity;

		// gravity
		if (characterController.isGrounded && verticalVelocity < 0f){
			verticalVelocity = -2f;
		}
		verticalVelocity += gravity * Time.deltaTime;

		Vector3 finalMove = horizontalMove;
		finalMove.y = verticalVelocity;

		moveDirection = finalMove * Time.deltaTime;
		characterController.Move(moveDirection);
	}
	
	private void staminaCheck(){
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
	
	private void healthCheck(){
		float regenThreshold = maxHealthValue * 0.4f;
		
		if (!isMoving && !isRunning && healthValue < regenThreshold){
			healthValue += (healthRate * 0.5f) * Time.deltaTime;
		}
		
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
		
		if (healthBar != null){
			float target = healthValue / maxHealthValue;
			healthBar.value = Mathf.MoveTowards(healthBar.value, target, Time.deltaTime * 5f);
		}
	}
	
	public void healthAction(float value){
		healthValue += value;
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
	}
	
	private void OnTriggerEnter(Collider other){
		if (other.transform.name == "Baldi" && gameController != null && !gameController.isDebugMode){
			gameController.isGameOver = true;
		}
	}
}