using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class SodaSprayEffects : MonoBehaviour{
#region Inspector
	private NavMeshAgent agent;
	private Coroutine sodaCoroutine;
#endregion

#region MainFunctions
	private void Start(){
		agent = GetComponent<NavMeshAgent>();
	}
	
	private void Update(){}

	private void OnTriggerEnter(Collider other){
		if (other.CompareTag("SodaSpray")){
			sodaCoroutine = StartCoroutine(SodaSpray(other));
		}
	}

	private void OnTriggerExit(Collider other){
		if (other.CompareTag("SodaSpray") && sodaCoroutine != null){
			StopCoroutine(sodaCoroutine);
			sodaCoroutine = null;
		}
	}
#endregion

#region SodaFunctions
	private IEnumerator SodaSpray(Collider other){
		Rigidbody rb = other.attachedRigidbody;

		while (true){
			agent.velocity = rb.velocity;
			yield return null;
		}
	}
#endregion
}
