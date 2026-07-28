using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using GeneralLibrary;

public class GameOverScript : MonoBehaviour{
#region Inspector
	[Header("Graphics")]
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private Sprite rareSprite;
	private Image image;	
	
	[Header("Values")]
	[SerializeField] private float timeToScene = 5f;	
	[SerializeField] private string sceneToLoad = "MainMenu";
	private int randomValue = 0;
	
	[Header("Sounds")]
	[SerializeField] private AudioClip[] music;
	private SoundHandler soundHandler;
#endregion
	
#region MainFunctions
	private void Start(){
		randomValue = UnityEngine.Random.Range(0, 999);
		StartCoroutine(StartCutscene());
		
		soundHandler = GetComponent<SoundHandler>();
		image = GetComponent<Image>();
	}
	
	// for now it's going to be like this
	private IEnumerator StartCutscene(){
		if(randomValue == 1){
			yield return new WaitForSeconds(timeToScene);
			image.color = Color.red;
			yield return new WaitForSeconds(timeToScene/2f);
			Application.Quit();
		}
		else{
			yield return new WaitForSeconds(timeToScene);
			SceneManager.LoadScene(sceneToLoad);
		}
		
		
	}
#endregion
}

// worst code of all time
/*
public class GameOverScript : MonoBehaviour
{
	private void Start()
	{

		delay = 5f;
		chance = Random.Range(1f, 99f);
		if (chance < 98f)
		{
			int num = Mathf.RoundToInt(Random.Range(0f, 4f));
			image.sprite = images[num];
		}
		else
		{
			image.sprite = rare;
		}
	}
	private void Update()
	{
		delay -= 1f * Time.deltaTime;
		if (delay <= 0f)
		{
			if (chance < 98f)
			{
				SceneManager.LoadScene(LoadScene);
			}
			else
			{
				image.transform.localScale = new Vector3(5f, 5f, 1f);
				image.color = Color.red;
				if (!audioDevice.isPlaying)
				{
					audioDevice.Play();
				}
				if (delay <= -5f)
				{
					Application.Quit();
				}
			}
		}
	}
	private Image image;
	private float delay;
	public Sprite[] images = new Sprite[5];
	public Sprite rare;
	private float chance;
	private AudioSource audioDevice;
	public string LoadScene;
}
*/