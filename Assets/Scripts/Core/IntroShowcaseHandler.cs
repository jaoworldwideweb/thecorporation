using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using GeneralLibrary;
using MathLibrary;
using GameLibrary;

[Serializable]
public class Logo{
	public Sprite image;
	public float displayTime = 3f;
}

public class IntroShowcaseHandler : MonoBehaviour{
#region Program
	[SerializeField] private bool doEasterEgg = false;
	[SerializeField] private SoundHandler soundHandler;
	
	[Header("Image")]
	[SerializeField] private AudioClip logoSound;
	[SerializeField] private Logo[] logos = new Logo[2];
	[SerializeField] private Image outputImage;
	[SerializeField] private string nextScene;
	
	[Header("Easter Egg")]
	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private VideoClip[] easterEggVideos;
#endregion

#region MainFunctions
	private void Start(){
		int gamesStarted = PlayerPrefs.GetInt("GamesStarted", 0) + 1;
		
		PlayerPrefs.SetInt("GamesStarted", gamesStarted);
		PlayerPrefs.Save();
		
		General.LockCursor();
		
		#if UNITY_EDITOR
			StartCoroutine(doEasterEgg ? DoEasterEgg() : DoLogoShowcase());
			return;
		#endif
		
		if(!(PlayerPrefs.GetInt("GamesStarted") > 3)){
			return;
		}
		
		dint randomNumber = new dint(CommonMath.GetRandom(), CommonMath.GetRandom());
		StartCoroutine(randomNumber.isEqual() ? DoEasterEgg() : DoLogoShowcase());
	}
	
	private void Update(){}
#endregion

#region EasterEggFunctions
	private IEnumerator DoEasterEgg(){
		outputImage.gameObject.SetActive(false);
		yield return UserInterface.PlayVideo(videoPlayer, easterEggVideos[UnityEngine.Random.Range(0, easterEggVideos.Length)]);
		Application.Quit();
	}
#endregion

#region LogoFunctions
	private IEnumerator DoLogoShowcase(){
		videoPlayer.gameObject.SetActive(false);
		soundHandler.PlaySound(logoSound, SoundOutput.PlayerSounds);
		
		yield return ShowcaseLoop();
		
		General.UnlockCursor();
		SceneManager.LoadScene(nextScene);		
	}
	
	private IEnumerator ShowcaseLoop(){
		outputImage.material.SetFloat("_Fade", 0f);
		
		foreach(Logo logo in logos){
			yield return TransitionTo(logo, 1.45f);
			yield return new WaitForSeconds(logo.displayTime);
		}
		
		yield return UserInterface.IDitherImage(outputImage, new dfloat(1f, 0f), 2f);
	}

	private void SetLogo(Logo logo){
		outputImage.sprite = logo.image;
	}

	private IEnumerator TransitionTo(Logo logo, float transitionTime = 1f){
		yield return UserInterface.IDitherImage(outputImage, new dfloat(1f, 0f), transitionTime);
		SetLogo(logo);
		yield return UserInterface.IDitherImage(outputImage, new dfloat(0f, 1f), transitionTime);
	}
#endregion
}
