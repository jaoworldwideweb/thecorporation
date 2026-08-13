using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using GeneralLibrary;

[Serializable]
public class Logo{
	public Sprite image;
	public float displayTime = 3f;
}

public class LogoShowcaseHandler : MonoBehaviour{
#region Inspector
	[Header("Image")]
	[SerializeField] private Logo[] logos = new Logo[2];
	[SerializeField] private Image outputImage;
	[Space(3)]
	[SerializeField] private string nextScene;
#endregion

#region MainFunctions
	private void Start(){
		StartCoroutine(ShowcaseLoop());
	}
#endregion

#region UserInterface
	private IEnumerator ShowcaseLoop(){
		foreach(Logo logo in logos){
			yield return TransitionTo(logo, 1.5f);
			yield return new WaitForSeconds(logo.displayTime);
		}
		
		yield return UserInterface.IFadeImage(outputImage, new dfloat(1f, 0f), 3f);
		SceneManager.LoadScene(nextScene);
	}

	private void SetLogo(Logo logo){
		outputImage.sprite = logo.image;
	}

	private IEnumerator TransitionTo(Logo logo, float transitionTime = 1f){
		yield return UserInterface.IFadeImage(outputImage, new dfloat(1f, 0f), transitionTime);
		SetLogo(logo);
		yield return UserInterface.IFadeImage(outputImage, new dfloat(0f, 1f), transitionTime);
	}
#endregion
}

