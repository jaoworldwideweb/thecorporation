using System.Collections;
using UnityEngine;

public class BaldiScript : Character{
	public bool db;
	public float baseTime;
	public float speed;
	public float baldiAnger;
	public float baldiTempAnger;
	public float baldiWait;
	public float baldiSpeedScale;

	private float moveFrames;
	private int currentPriority;

	public bool antiHearing;
	public float antiHearingTime;

	public float vibrationDistance;

	public float angerRate;
	public float angerRateRate;
	public float angerFrequency;
	public bool endless;

	public Transform player;

	private AudioSource baldiAudio;

	public AudioClip slap;
	public Animator baldiAnimator;

	public float coolDown;

	private Vector3 previous;
	private bool rumble;

	private Coroutine slapRoutine;

	protected override void Awake(){
		base.Awake();
		baldiAudio = GetComponent<AudioSource>();
	}

	private void Start(){
		Wander();

		slapRoutine = StartCoroutine(SlapRoutine(baseTime));

		StartCoroutine(CooldownRoutine());
		StartCoroutine(TempAngerRoutine());
		StartCoroutine(AntiHearingRoutine());

		if (endless){
			StartCoroutine(EndlessRoutine());			
		}
	}

	private void FixedUpdate(){
		if (moveFrames > 0f){
			moveFrames--;
			agent.speed = speed;
		}
		else{
			agent.speed = 0f;
		}

		Vector3 direction = player.position - transform.position;
		RaycastHit hit;

		if (Physics.Raycast(transform.position + Vector3.up * 2f, direction, out hit, Mathf.Infinity, 769,QueryTriggerInteraction.Ignore) && hit.transform.CompareTag("Player")){ // worst line of code of all time
			db = true;
			TargetPlayer();
		}
		else{
			db = false;
		}
	}

	private IEnumerator SlapRoutine(float delay){
		while(true){
			yield return new WaitForSeconds(delay);
			Move();
			delay = Mathf.Max(0.05f, baldiWait - baldiTempAnger);
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
			if (baldiTempAnger > 0f){
				baldiTempAnger = Mathf.Max(0f, baldiTempAnger - 0.02f * Time.deltaTime);				
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

	public void TargetPlayer()
	{
		Follow(player);

		coolDown = 1f;
		currentPriority = 0;
	}

	private void Move()
	{
		if (transform.position == previous && coolDown < 0f)
		{
			Wander();
		}

		moveFrames = 10f;
		previous = transform.position;

		baldiAudio.PlayOneShot(slap);

		if (rumble)
		{
			float distance = Vector3.Distance(transform.position, player.position);

			if (distance < vibrationDistance)
			{
				float motorLevel = 1f - distance / vibrationDistance;
				// Use motorLevel if you have rumble support.
			}
		}
	}

	public void GetAngry(float value)
	{
		baldiAnger += value;

		if (baldiAnger < 0.5f)
			baldiAnger = 0.5f;

		baldiWait = -3f * baldiAnger / (baldiAnger + 2f / baldiSpeedScale) + 3f;

		if (slapRoutine != null)
		{
			StopCoroutine(slapRoutine);
			slapRoutine = StartCoroutine(SlapRoutine(Mathf.Max(0.05f, baldiWait - baldiTempAnger)));
		}
	}

	public void GetTempAngry(float value)
	{
		baldiTempAnger += value;

		if (slapRoutine != null)
		{
			StopCoroutine(slapRoutine);
			slapRoutine = StartCoroutine(SlapRoutine(Mathf.Max(0.05f, baldiWait - baldiTempAnger)));
		}
	}

	public void Hear(Vector3 soundLocation, int priority){
		if (!antiHearing && priority >= currentPriority)
		{
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