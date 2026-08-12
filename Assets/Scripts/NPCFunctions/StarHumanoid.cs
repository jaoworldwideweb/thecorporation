using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class StarHumanoid : Character{
#region Inspector
	[Header("Scripts")]
	[SerializeField] private PlayerScript playerScript;
	
	[Header("Movement")]
	[SerializeField] private float moveDelay = 3f;
	[SerializeField] private float moveWaitTime = 3f;
	[SerializeField] private float moveSpeed = 75f;
	[SerializeField] private float speedScale = 0.65f;
	[SerializeField] private float coolDown = 0f;
	[SerializeField] private float moveFrames = 10f;
	[SerializeField] private Vector3 previous;
	[SerializeField] private Transform player;

	[Header("AI")]
	[SerializeField] private float anger = 0f;
	[SerializeField] private float angerRate = 0.01f;
	[SerializeField] private float angerRateRate = 0.00025f;
	[SerializeField] private float angerFrequency = 1f;
	[SerializeField] private float temporaryAnger = 0f;
	[SerializeField] private int currentPriority = 0;
	[SerializeField] private bool antiHearing = false;
	[SerializeField] private float antiHearingTime = 0f;

	[Header("Audio")]
	[SerializeField] private AudioSource audioOutput;
	[SerializeField] private AudioClip moveSound;
	
	private Coroutine moveRoutine;
#endregion

#region MainFunctions
	protected override void Awake(){
		base.Awake();
		audioOutput = GetComponent<AudioSource>();
	}

	private void Start(){
		Wander();

		moveRoutine = StartCoroutine(SlapRoutine(moveDelay));

		StartCoroutine(CooldownRoutine());
		StartCoroutine(TempAngerRoutine());
		StartCoroutine(AntiHearingRoutine());
	}

	private void FixedUpdate(){
		if (moveFrames > 0f){
			moveFrames--;
			agent.speed = moveSpeed;
		}
		else{
			agent.speed = 0f;
		}

		Vector3 direction = player.position - transform.position;
		RaycastHit hit;

		if (Physics.Raycast(transform.position + Vector3.up * 2f, direction, out hit, Mathf.Infinity, 769,QueryTriggerInteraction.Ignore) && hit.transform.CompareTag("Player")){ // worst line of code of all time
			TargetPlayer();
		}
	}
#endregion

	private IEnumerator SlapRoutine(float delay){
		while(true){
			yield return new WaitForSeconds(delay);
			Move();
			delay = Mathf.Max(0.05f, moveWaitTime - temporaryAnger);
		}
	}

	private IEnumerator CooldownRoutine(){
		while(true){
			if (coolDown > 0f){
				coolDown -= Time.deltaTime;				
			}
			yield return null;
		}
	}

	private IEnumerator TempAngerRoutine(){
		while(true){
			if (temporaryAnger > 0f){
				temporaryAnger = Mathf.Max(0f, temporaryAnger - 0.02f * Time.deltaTime);				
			}
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
		while(true){
			yield return new WaitForSeconds(angerFrequency);
			GetAngry(angerRate);
			angerRate += angerRateRate;
		}
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

	private void Move(){
		if (transform.position == previous && coolDown < 0f){
			Wander();
		}

		moveFrames = 10f;
		previous = transform.position;
		audioOutput.PlayOneShot(moveSound);
	}

	public void GetAngry(float value){
		anger += value;
		if (anger < 0.5f){
			anger = 0.5f;
		}
		
		moveWaitTime = -3f * anger / (anger + 2f / speedScale) + 3f;

		if (moveRoutine != null){
			StopCoroutine(moveRoutine);
			moveRoutine = StartCoroutine(SlapRoutine(Mathf.Max(0.05f, moveWaitTime - temporaryAnger)));
		}
	}

	public void GetTempAngry(float value){
		temporaryAnger += value;

		if (moveRoutine != null){
			StopCoroutine(moveRoutine);
			moveRoutine = StartCoroutine(SlapRoutine(Mathf.Max(0.05f, moveWaitTime - temporaryAnger)));
		}
	}

	public void Hear(Vector3 soundLocation, int priority){
		if (!antiHearing && priority >= currentPriority){
			MoveTo(soundLocation);
			currentPriority = priority;
		}
	}

	public void ActivateAntiHearing(float time){
		Wander();
		antiHearing = true;
		antiHearingTime = time;
	}
}