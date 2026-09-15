using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using MathLibrary;
using GeneralLibrary;

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
	
	[Header("Sprites")]
	[SerializeField] private SpriteRenderer spriteOutput;
	[SerializeField] private Sprite[] movementSprite;
	
	[Header("Sounds")]
	[SerializeField] private AudioSource audioOutput;
	[SerializeField] private AudioClip[] seeSounds;
	private bool chasing;
#endregion

#region MainFunctions
	private void Awake(){
		base.Awake();
	}
	
	private void Start(){
		playerTransform = playerScript.transform;
		
		StartCoroutine(SpriteChanger());
		StartRoutine(WanderRoutine());
	}
	
	private void Update(){
		SpriteChanger();
		
		if(!chasing){
			if(canSeePlayer()){
				StartChasing();
			}

			return;
		}
		
		if(canSeePlayer()){
			return;
		}
		
		StopChasing();
	}
	
#endregion

#region CharcterFunctions
	private IEnumerator SpriteChanger(){
		while(true){
			dint randomRange;
			
			do{
				randomRange = new dint(UnityEngine.Random.Range(10, 50), UnityEngine.Random.Range(10, 50));
			}while(randomRange.a > randomRange.b);
			
			yield return WaitRandom(chasing, new dfloat(4f, 8f));
			
			if(randomRange.Subtract() % 2 == 0){
				continue;
			}
			
			spriteOutput.sprite = movementSprite[UnityEngine.Random.Range(0, movementSprite.Length)];
			yield return WaitRandom(chasing, new dfloat(0.04f, 0.08f));
			spriteOutput.sprite = movementSprite[0];
		}
	}
	
	private IEnumerator WaitRandom(bool half, dfloat values){
		float mininum = half ? range.a * 0.5f : range.a;
		float maximum = half ? range.b * 0.5f : range.b;
		
		yield return new WaitForSeconds(UnityEngine.Random.Range(mininum, maximum));
	}
	
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
		StartRoutine(WanderRoutine());
	}

	private IEnumerator ChaseRoutine(){
		ResumeMovement();
		
		while(chasing){
			agent.SetDestination(playerTransform.position);
			yield return null;
		}
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