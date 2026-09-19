using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StarHumanoid : Character{
#region Inspector
	[Header("Player")]
	[SerializeField] private PlayerScript playerScript;
	private Transform playerTransform;
	
	[Header("Vision")]
	[SerializeField] private float sightRange = 50f;
	[SerializeField] private float sightAngle = 90f;
	[SerializeField] private float eyeHeight = 2f;
	[SerializeField] private LayerMask targetMask;
	
	[Header("Sounds")]
	[SerializeField] private AudioSource audioOutput;
	[SerializeField] private AudioClip[] seeSounds;
	
	[Header("Ambush")]
	[SerializeField] private float cornerZoneRadius = 3f;
	[SerializeField] private float ambushTriggerDistance = 20f;
	[SerializeField] private float fleeHealthThreshold = 50f;
	[SerializeField] private float fleeDistance = 15f;
	[SerializeField] private float flankDistance = 4f;
	[SerializeField] private float flankSampleRadius = 5f;
	[SerializeField] private float lungeSpeedMultiplier = 1.5f;
	
	private bool chasing;
	private bool ambushing;
	private float ambushTriggerDistanceSqr;
	private float baseSpeed;
	private float baseFootstepVolume;
#endregion

#region MainFunctions
#pragma warning disable CS0414
	private void Awake(){
		base.Awake();
		
		ambushTriggerDistanceSqr = ambushTriggerDistance * ambushTriggerDistance;
		baseSpeed = agent.speed;
	}
#pragma warning restore CS0414
	
	private void Start(){
		playerTransform = playerScript.transform;
		StartRoutine(WanderRoutine());
	}
	
	private void Update(){
		if(!chasing){
			if(canSeePlayer()){
				StartChasing();
			}
			return;
		}
		
		if (!ambushing && wanderer.IsNearHallwayCorner(transform.position, cornerZoneRadius)){
			Vector3 toPlayer = playerTransform.position - transform.position;
			if (toPlayer.sqrMagnitude <= ambushTriggerDistanceSqr){
				DecideAmbush();
			}
		}

		if(canSeePlayer()){
			return;
		}
		
		StopChasing();
	}
#endregion

#region AIFunctions
	private void StartChasing(){
		if (chasing){
			return;
		}
		
		chasing = true;
		
		PlaySeeSound();
		StartRoutine(ChaseRoutine());
	}
	
	private void StopChasing(){
		if (!chasing){
			return;
		}
		
		chasing = false;
		ambushing = false;
		agent.speed = baseSpeed;

		StartRoutine(WanderRoutine());
	}

	private IEnumerator ChaseRoutine(){
		ResumeMovement();
		
		while(chasing){
			agent.SetDestination(playerTransform.position);
			yield return null;
		}
	}
	
	private void DecideAmbush(){
		ambushing = true;
		
		bool shouldFlee = playerScript.health < fleeHealthThreshold;
		StartRoutine(AmbushRoutine(shouldFlee));
	}

	private IEnumerator AmbushRoutine(bool shouldFlee){
		if(shouldFlee){
			ResumeMovement();
			agent.SetDestination(GetFleePoint());
			yield return new WaitUntil(HasReachedDestination);
		}

		ResumeMovement();
		agent.SetDestination(GetFlankPoint());
		yield return new WaitUntil(HasReachedDestination);

		agent.speed = baseSpeed * lungeSpeedMultiplier;
		agent.SetDestination(playerTransform.position);
		yield return new WaitUntil(HasReachedDestination);

		agent.speed = baseSpeed;
		ambushing = false;
	}
	
	private Vector3 GetFlankPoint(){
		Vector3 behind = playerTransform.position - playerTransform.forward * flankDistance;

		if (NavMesh.SamplePosition(behind, out NavMeshHit hit, flankSampleRadius, NavMesh.AllAreas)){
			return hit.position;
		}

		return playerTransform.position;
	}
	
	private Vector3 GetFleePoint(){
		Vector3 awayDir = (transform.position - playerTransform.position).normalized;
		Vector3 candidate = transform.position + awayDir * fleeDistance;

		if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas)){
			return hit.position;
		}

		return transform.position;
	}
	
	private void PlaySeeSound(){
		if(audioOutput.isPlaying){
			return;
		}
		
		audioOutput.PlayOneShot(seeSounds[Random.Range(0, seeSounds.Length)]);
	}
	
	private bool canSeePlayer(){
		Vector3 origin = transform.position + Vector3.up * eyeHeight;
		Vector3 target = playerTransform.position + Vector3.up * eyeHeight;

		Vector3 direction = target - origin;
		float distance = direction.magnitude;

		if (distance > sightRange){
			return false;
		}

		if (Vector3.Angle(transform.forward, direction) > sightAngle * 0.5f){
			return false;
		}

		if (!Physics.Raycast(origin,direction.normalized, out RaycastHit hit, distance, targetMask)){
			return false;
		}

		return hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform);
	}
#endregion
}