using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using MathLibrary;
using GeneralLibrary;
using System.Runtime.CompilerServices;

namespace GameLibrary{
#region Enumeration
	public enum HealthAction{
		Damage,
		Regeneration
	}
	
	public enum CreatureType{
		None,
		Skinwalker,
		StarCreature
	}
	
	public enum DeathReason{
		Stabbing,
		Choking,
		Bleeding
	}
	
	public enum FootstepSoundType{
		Wood,
		Vent,		
		Metal,
		MetalGrate,
		Tile,
		Concrete,
		Water
	}
	
	public enum SoundOutput{
		PlayerSounds,
		GameSounds
	}
	
	public enum MusicOutput{
		MainSong,
		Ambience
	}
	
	public enum BoxColor{
		Red,
		Green,
		Blue,
		Orange,
		LightRed,
		LightGreen,
		LightBlue,
		LightOrange
	}
	
	public enum ItemType{
		Nothing,
		ChocolateBar,
		DrinkableSoda,
		SprayableSoda,
		HealthPack
	}
#endregion

#region GameObjects
	[Serializable]
	public class UIObject{
		public GameObject obj;
		public RectTransform rectTransform;
		public Vector2 oldRectTransform = new Vector2(0f, 0f);
		public bool isMoving = false;
		public bool isInState; // generic variable :-)
		
		public void SetOldTransform(){
			oldRectTransform = rectTransform.anchoredPosition;
		}
		
		public IEnumerator MoveObject(Vector2 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			if(isMoving){
				yield break;
			}
			
			isMoving = true;
			yield return UserInterface.MoveObject(rectTransform, targetPosition, easing, time);
			isMoving = false;
		}
	}
	
	[Serializable]
	public class UITextObject : UIObject{
		public TMP_Text objText;
	}

	[Serializable]
	public class FullObject{
		public GameObject obj;
		public Vector3 oldTransform = new Vector3(0f, 0f, 0f);
		public bool isMoving = false;
		public bool isInState; // generic variable :-)
		
		private Coroutine currentAction = null;
		
		public void SetOldTransform(){
			oldTransform = obj.transform.localPosition;
		}
		
		public void MoveObject(Vector3 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			if(isMoving){
				return;
			}
			
			currentAction = CoroutineRunner.Instance.StartCoroutine(IEnumeratorMoveObject(targetPosition, easing, time));
		}
		
		private IEnumerator IEnumeratorMoveObject(Vector3 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			isMoving = true;
			yield return UserInterface.Move3DObject(obj.transform, targetPosition, easing, time);
			isMoving = false;
			
			currentAction = null;
		}
	}
#endregion

#region GameplayData
	[Serializable]
	public class Item{
		public string name = "Nothing";
		public ItemType type = ItemType.Nothing;
		public Sprite sprite;
		
		public void Clear(){
			name = "Nothing";
			type = ItemType.Nothing;
			sprite = null;
		}
		
		public void Transfer(Item item){
			Clear();
			name = item.name;
			type = item.type;
			sprite = item.sprite;
		}
	}
	
	[Serializable]
	public class ItemSlot{
		public Image outputTexture;
		public Item item;
		
		public void Set(Item item){
			this.item.Transfer(item);
			outputTexture.sprite = item.sprite;
		}
	}
	
	public struct ItemDefinition{
		public ItemType type;
		public Func<bool> function;
		
		public ItemDefinition(ItemType type, Func<bool> function){
			this.type = type;
			this.function = function;
		}
	}
	
	[Serializable]
	public class BoxData{
		public BoxColor currentBoxColor = BoxColor.Red;
		public int boxCode = 0;
		
		public void ClearData(){
			currentBoxColor = BoxColor.Red;
			boxCode = 0;
		}
		
		public void Transfer(BoxData data){
			ClearData();
			currentBoxColor = data.currentBoxColor;
			boxCode = data.boxCode;
		}
		
		public string GetFormatted(){
			return 	$"{General.GetFormattedColor(currentBoxColor).ToUpper()}\n" +
					$"{boxCode}";
		}
	}
	
	[Serializable]
	public class FootstepSound{
		public FootstepSoundType soundType = FootstepSoundType.Concrete;
		public PhysicMaterial material;
		public AudioClip[] sounds;
	}

	[Serializable]
	public class Creature{
		public CreatureType type;
		public AudioClip music;
		public Sprite deathImage;
		public Sprite[] autopsyImage;
		public bool hasDeathImage = false;
	}
	
	[Serializable]
	public class Job{
		public string name = "Worker";
		public string id = "0000";
	}

	[Serializable]
	public class EmployeeData{
		[Header("Main Information")]
		public Sprite photo;
		public string name = "Jayden Doe";
		public string gender = "Unknown";
		public Date birthday;
		
		[Header("Workplace Information")]
		public string workplace = "A00";
		public string id = "0000";
		public Job job;
		
		public string GetFormattedData(){
			return
				$"Gender: {gender}\n" +
				$"Date of birth: {birthday.GetDate()}\n" +
				$"Current Job: {job.name} ({job.id})\n" +
				$"Workplace: {workplace}\n";
		}
	}
	
	[Serializable]
	public class CharacterDescription{
		[Header("Main Information")]
		public Sprite photo;
		public string name = "Jayden Doe";
		public string gender = "Unknown";
		[TextArea(3, 10)] public string description;
		
		public string GetFormatted(){
			return
				$"Name: {name}\n" +
				$"Gender: {gender}\n\n";
		}
	}
#endregion
}