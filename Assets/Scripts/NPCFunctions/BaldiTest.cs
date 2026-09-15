using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using MathLibrary;
using GeneralLibrary;

public class BaldiTest : Character{
/*#region Inspector
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;

	[Header("Movement")]
	[SerializeField] private SpriteRenderer spriteOutput;
	[SerializeField] private Sprite[] movementSprite;
	[SerializeField] private float moveDelay = 3f;
	[SerializeField] private float moveWaitTime = 3f;
	[SerializeField] private float moveSpeed = 75f;
	[SerializeField] private float speedScale = 0.65f;
	[SerializeField] private float moveFrames = 10f;
	[SerializeField] private Transform player;
	private float coolDown;
	private Vector3 previous;
	
	[Header("AI")]
	[SerializeField] private float angerRate = 0.01f;
	[SerializeField] private float angerRateRate = 0.00025f;
	[SerializeField] private float angerFrequency = 1f;
	[SerializeField] private LayerMask targetMask;
	private float anger;
	private float temporaryAnger;
	private int currentPriority;
	private float antiHearingTime;
	[HideInInspector] public bool antiHearing;
	
	[Header("Audio")]
	[SerializeField] private AudioSource audioOutput;
	[SerializeField] private AudioClip[] seeSounds;
	private Coroutine moveRoutine;
#endregion

#region MainFunctions
	private void Start(){
		Wander();
		
		RestartMoveRoutine(moveDelay);
		
		StartCoroutine(CooldownRoutine());
		StartCoroutine(TempAngerRoutine());
		StartCoroutine(AntiHearingRoutine());
		StartCoroutine(EndlessRoutine());
	}

	private void FixedUpdate(){
		UpdateMovement();
		
		Vector3 origin = transform.position + Vector3.up * 2f;
		Vector3 direction = player.position - origin;
		
		GenericRaycastForObject(OnPlayerSighted, new dVector3(origin, direction), targetMask, direction.magnitude);
	}
#endregion

#region MovementFunctions
	private void UpdateMovement(){
		if (moveFrames > 0f){
			moveFrames--;
			agent.speed = moveSpeed;
		}
		else{
			agent.speed = 0f;
		}
	}

	private IEnumerator SlapRoutine(float delay){
		while (true){
			yield return new WaitForSeconds(delay);
			
			Move();
			delay = Mathf.Max(0.05f, moveWaitTime - temporaryAnger);
		}
	}

	private void RestartMoveRoutine(float delay){
		if (moveRoutine != null){
			StopCoroutine(moveRoutine);			
		}
		
		moveRoutine = StartCoroutine(SlapRoutine(delay));
	}

	private void Move(){
		if (transform.position == previous && coolDown <= 0f){
			Wander();			
		}
		
		moveFrames = 10f;
		previous = transform.position;

		if (movementSprite.Length > 0){
			spriteOutput.sprite = movementSprite[0];			
		}
	}
#endregion

#region AIFunctions
	private void OnPlayerSighted(RaycastHit hit){
		if (!hit.transform.CompareTag("Player")){
			return;			
		}
		
		TargetPlayer();

		if (audioOutput.isPlaying || seeSounds.Length == 0){
			return;			
		}
		
		audioOutput.PlayOneShot(seeSounds[UnityEngine.Random.Range(0, seeSounds.Length)]);
	}

	private void Wander(){
		StartRoutine(WanderRoutine());
		
		coolDown = 1f;
		currentPriority = 0;
	}

	public void TargetPlayer(){
		Follow(player);
		
		coolDown = 1f;
		currentPriority = 0;
	}

	public void GetAngry(float value){
		anger = Mathf.Max(0.5f, anger + value);
		moveWaitTime = -3f * anger / (anger + 2f / speedScale) + 3f;
		
		RestartMoveRoutine(Mathf.Max(0.05f, moveWaitTime - temporaryAnger));
	}

	public void GetTempAngry(float value){
		temporaryAnger += value;
		
		RestartMoveRoutine(Mathf.Max(0.05f, moveWaitTime - temporaryAnger));
	}

	public void Hear(Vector3 soundLocation, int priority){
		if (antiHearing || priority < currentPriority){
			return;			
		}
		
		MoveTo(soundLocation);
		currentPriority = priority;
	}

	public void ActivateAntiHearing(float time){
		Wander();
		
		antiHearing = true;
		antiHearingTime = time;
	}
#endregion

#region CoroutineFunctions
	private IEnumerator CooldownRoutine(){
		while (true){
			coolDown = Mathf.Max(0f, coolDown - Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator TempAngerRoutine(){
		while (true){
			temporaryAnger = Mathf.Max(0f, temporaryAnger - 0.02f * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator AntiHearingRoutine(){
		while (true){
			if (antiHearing){
				antiHearingTime -= Time.deltaTime;

				if (antiHearingTime <= 0f){
					antiHearing = false;
					antiHearingTime = 0f;
				}
			}

			yield return null;
		}
	}

	private IEnumerator EndlessRoutine(){
		while (true){
			yield return new WaitForSeconds(angerFrequency);

			GetAngry(angerRate);
			angerRate += angerRateRate;
		}
	}
#endregion*/
}