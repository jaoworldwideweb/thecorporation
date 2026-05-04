using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour{
	[Header("Scripts")]
	[SerializeField] private GameControllerScript gameController;
	
	[Header("Main")]
	[SerializeField] private Quaternion playerRotation;
	[SerializeField] private Vector3 moveDirection;
	[SerializeField] private CharacterController characterController;
	[SerializeField] private Vector3 frozenPosition;
	[SerializeField] private float playerHeight;
	[SerializeField] private float fliparoo;
	[SerializeField] private float flipaturn;
	[SerializeField] private bool sensitivityActive;
	[SerializeField] private float sensitivity;	
	[SerializeField] private float mouseSensitivity;
	
	[Header("Walk/Run")]
	[SerializeField] private float playerSpeed;	
	[SerializeField] private float walkSpeed;
	[SerializeField] private float runSpeed;
	[SerializeField] private float slowSpeed;
	[SerializeField] private bool isMoving;
	
	[Header("Stamina")]
	[SerializeField] public float stamina;	
	[SerializeField] public float maxStamina;
	[SerializeField] private float staminaRate;
	[SerializeField] private bool isRunning;
	[SerializeField] private Slider staminaBar;
	
	[Header("Health")]
	[SerializeField] public float healthValue;	
	[SerializeField] public float maxHealthValue;
	[SerializeField] private float healthRate;
	[SerializeField] private Slider healthBar;
	
	private void Start(){
		if (PlayerPrefs.GetInt("AnalogMove") == 1){
			sensitivityActive = true;
		}
		
		playerHeight = transform.position.y;
		stamina = maxStamina;
		playerRotation = transform.rotation;
		mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
	}
	
	private void Update(){
		transform.position = new Vector3(transform.position.x, playerHeight, transform.position.z);
		
		isMoving = characterController.velocity.sqrMagnitude > 0.01f;
		isRunning = Singleton<InputManager>.Instance.GetActionKey(InputAction.Run);
		
		mouseMove();
		playerMove();
		staminaCheck();
		
		if (characterController.velocity.magnitude > 0f){
			gameController.lockMouse();
		}
	}
	
	private void mouseMove(){
		playerRotation.eulerAngles = new Vector3(playerRotation.eulerAngles.x, playerRotation.eulerAngles.y, 0f);
		playerRotation.eulerAngles = playerRotation.eulerAngles + Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity * Time.timeScale;
		transform.rotation = playerRotation;
	}
	
	private void playerMove(){
		Vector3 movement = Vector3.zero;
		Vector3 lateralMovement = Vector3.zero;
		
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveForward)) movement = transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveBackward)) movement = -transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveLeft)) lateralMovement = -transform.right;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveRight)) lateralMovement = transform.right;
		
		if (stamina > 0f){
			if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Run)){
				playerSpeed = runSpeed;
				sensitivity = 1f;
			}
			else{
				playerSpeed = walkSpeed;
				
				if (sensitivityActive){
					sensitivity = Mathf.Clamp((movement + lateralMovement).magnitude, 0f, 1f);
				}
				else{
					sensitivity = 1f;
				}
			}
		}
		else{
			playerSpeed = walkSpeed;
			
			if (sensitivityActive){
				sensitivity = Mathf.Clamp((lateralMovement + movement).magnitude, 0f, 1f);
			}
			else{
				sensitivity = 1f;
			}
		}
		
		playerSpeed *= Time.deltaTime;
		moveDirection = (movement + lateralMovement).normalized * playerSpeed * sensitivity;
		
		characterController.Move(moveDirection);
	}
	
	private void staminaCheck(){
		if (isMoving && isRunning && stamina > 0f){
			stamina -= staminaRate * Time.deltaTime;
		}
		else if (stamina < maxStamina){
			stamina += staminaRate * Time.deltaTime;
		}

		stamina = Mathf.Clamp(stamina, 0f, maxStamina);

		float target = (stamina / maxStamina) * 100f;
		staminaBar.value = Mathf.MoveTowards(staminaBar.value, target, Time.deltaTime * 600f);
	}
	
	private void healthCheck(){
		float regenThreshold = maxHealthValue * 0.4f;
		
		if (!isMoving && !isRunning && healthValue < regenThreshold){
			healthValue += healthRate * Time.deltaTime / 2;
		}
		
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
		
		float target = (healthValue / maxHealthValue) * 100f;
		healthBar.value = Mathf.MoveTowards(healthBar.value, target, Time.deltaTime * 600f);
	}
	
	public void healthAction(float value){
		healthValue += value;
		healthValue = Mathf.Clamp(healthValue, 0f, maxHealthValue);
	}
	
	private void OnTriggerEnter(Collider other){
		if (other.transform.name == "Baldi" & !gameController.isDebugMode){
			gameController.isGameOver = true;
		}
	}
}
