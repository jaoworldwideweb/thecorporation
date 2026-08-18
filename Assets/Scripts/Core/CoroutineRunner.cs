using System;
using System.Collections;
using UnityEngine;

public sealed class CoroutineRunner : MonoBehaviour{
	public static CoroutineRunner Instance { get; private set; }
	private void Awake(){
		Instance = this;
	}
}