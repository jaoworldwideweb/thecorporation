using System;
using System.Collections;
using UnityEngine;

public sealed class CoroutineRunner : MonoBehaviour{
	public static CoroutineRunner Instance { get; private set; }
	private void Awake(){
		Debug.Log("CoroutineRunner Awake");
		Instance = this;
	}
}