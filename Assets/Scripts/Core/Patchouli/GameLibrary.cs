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
	public enum Direction{
		Up,
		Down,
		Left,
		Right
	}
	
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
	
	public enum FootstepType{
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
	[System.Serializable]
	public class UIObject{
		public GameObject obj;
		public RectTransform rectTransform;
		public Vector2 oldRectTransform = new Vector2(0f, 0f);
		public DirectionVector2 directions;
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
		
		public Vector2 GetDirectionVector(Direction direction){
			switch(direction){
				case Direction.Up:
					return directions.up;
				
				case Direction.Down:
					return directions.down;
				
				case Direction.Left:
					return directions.left;
				
				case Direction.Right:
					return directions.right;
			}
			
			return Vector2.zero;
		}
	}
	
	[System.Serializable]
	public class UITextObject : UIObject{
		public TMP_Text objText;
	}

	[System.Serializable]
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
	
	// this is so bad.
	[System.Serializable]
	public struct DirectionVector2{
		public Vector2 up;
		public Vector2 down;
		public Vector2 left;
		public Vector2 right;
		
		public DirectionVector2(Vector2 up, Vector2 down, Vector2 left, Vector2 right){
			this.up = up;
			this.down = down;
			this.left = left;
			this.right = right;
		}
	}
#endregion

#region GameplayData
	[System.Serializable]
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
	
	[System.Serializable]
	public struct ItemSlot{
		public Image outputTexture;
		public Item item;
		
		public ItemSlot(Image outputTexture, Item item){
			this.outputTexture = outputTexture;
			this.item = item;
		}
		
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
	
	[System.Serializable]
	public struct Box{
		public BoxColor color;
		public int id;
		
		public Box(BoxColor color, int id){
			this.color = color;
			this.id = id;
		}
		
		// functions
		public void ClearData(){
			color = BoxColor.Red;
			id = 0;
		}
		
		public void Transfer(Box box){
			ClearData();
			color = box.color;
			id = box.id;
		}
		
		public string GetFormatted(){
			return 
				$"{General.GetFormattedColor(color).ToUpper()}\n" +
				$"{id}";
		}
	}
	
	[System.Serializable]
	public class FootstepSound{
		public FootstepType type;
		public PhysicMaterial material;
		public AudioClip[] sounds;
	}
	
	[System.Serializable]
	public struct Job{
		public string name;
		public string id;
		
		public Job(string name, string id){
			this.name = name;
			this.id = id;
		}
	}

	[System.Serializable]
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
		
		public string GetFormatted(){
			return
				$"Gender: {gender}\n" +
				$"Date of birth: {birthday.GetDate()}\n" +
				$"Current Job: {job.name} ({job.id})\n" +
				$"Workplace: {workplace}\n";
		}
	}
	
	[System.Serializable]
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