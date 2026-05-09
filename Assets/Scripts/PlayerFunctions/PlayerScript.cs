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
	[SerializeField] private bool isMoving;
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
		
		stamina = maxStamina;
		mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
	}
	
	private void Update(){
		isRunning = Singleton<InputManager>.Instance.GetActionKey(InputAction.Run);
		
		mouseMove();
		playerMove();
		healthCheck();
		staminaCheck();
		
		isMoving = moveDirection.sqrMagnitude > 0.0001f;

		if (characterController.velocity.sqrMagnitude > 0.01f){
			gameController.lockMouse();
		}
	}
	
	private void mouseMove(){
		float sensitivity = mouseSensitivity * 2f;
		
		mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		transform.Rotate(Vector3.up * mouseX);
	}
		
	private void playerMove(){
		Vector3 input = Vector3.zero;
		
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveForward)) input += transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveBackward)) input -= transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveLeft)) input -= transform.right;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveRight)) input += transform.right;
		
		float inputMagnitude = Mathf.Clamp01(input.magnitude);
		Vector3 direction = input.normalized;

		// speed logic
		if (stamina > 0.1f && isRunning){
			playerSpeed = runSpeed;
			sensitivity = 1f;
		}
		else{
			playerSpeed = walkSpeed;
			sensitivity = sensitivityActive ? inputMagnitude : 1f;
		}

		// horizontal movement
		Vector3 horizontalMove = direction * playerSpeed * sensitivity;

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
		if (isMoving && isRunning && stamina > 0.1f){
			stamina -= staminaRate * Time.deltaTime;
		}
		else if (stamina < maxStamina){
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