using System;
using System.Collections;
using UnityEngine;

public class SodaSprayObject : MonoBehaviour{
#region MainFunctions
	private void Awake(){
		StartCoroutine(PushSoda(12f, 30f));
	}
#endregion

#region SodaFunctions
	private IEnumerator PushSoda(float lifespan, float speed = 10f){
		float timer = 0f;
		
		while (timer < lifespan){
			transform.position += transform.forward * speed * Time.deltaTime;
			timer += Time.deltaTime;
			yield return null;
		}
		
		Destroy(gameObject);
	}
#endregion
}
