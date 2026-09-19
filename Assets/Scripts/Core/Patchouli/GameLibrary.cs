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
	
	public enum ObjectState{
		Showing,
		Hidden
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
	public class UserInterfaceObject{
		[HideInInspector] public RectTransform rectTransform;
		public GameObject obj;
		public Vector2 cachedPosition = new Vector3(0f, 0f, 0f);
		public DirectionVector2 directions;
		public ObjectState state;
		public bool isMoving = false;
		
		public void Initiate(){
			SetCachedPosition();
			SetRectTransform();
		}
		
		public void SetCachedPosition(){
			cachedPosition = obj.transform.localPosition;
		}
		
		public void SetRectTransform(){
			rectTransform = obj.GetComponent<RectTransform>();
		}
		
		public Vector2 GetDirection(Direction direction) => directions.GetDirection(direction);
		
		public IEnumerator MoveObject(Vector2 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			if(isMoving){
				yield break;
			}
			
			isMoving = true;
			yield return UserInterface.MoveObject(rectTransform, targetPosition, easing, time);
			isMoving = false;
		}
	}
	
	[System.Serializable]
	public class UserInterfaceTextObject : UserInterfaceObject{
		public TMP_Text tmpText;
	}

	[System.Serializable]
	public class FullObject{
		public GameObject obj;
		public Vector3 cachedPosition = new Vector3(0f, 0f, 0f);
		public DirectionVector3 directions;
		public ObjectState state;
		public bool isMoving = false;
		
		private Coroutine currentAction = null;
		
		public void SetCachedPosition(){
			cachedPosition = obj.transform.localPosition;
		}
		
		public Vector3 GetDirection(Direction direction) => directions.GetDirection(direction);
		
		private IEnumerator MoveObject(Vector3 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			if(isMoving){
				yield break;
			}
			
			isMoving = true;
			yield return UserInterface.Move3DObject(obj.transform, targetPosition, easing, time);
			isMoving = false;
			
			currentAction = null;
		}
	}
	
	[System.Serializable]
	public struct MenuTransition{
		public GameObject currentMenu;
		public GameObject nextMenu;

		public MenuTransition(GameObject currentMenu, GameObject nextMenu){
			this.currentMenu = currentMenu;
			this.nextMenu = nextMenu;
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
		
		public Vector2 GetDirection(Direction direction){
			switch(direction){
				case Direction.Up: return up;
				case Direction.Down: return down;
				case Direction.Left: return left;
				case Direction.Right: return right; 
			}
			
			return Vector2.zero;
		}
	}
	
	[System.Serializable]
	public struct DirectionVector3{
		public Vector3 up;
		public Vector3 down;
		public Vector3 left;
		public Vector3 right;
		
		public DirectionVector3(Vector3 up, Vector3 down, Vector3 left, Vector3 right){
			this.up = up;
			this.down = down;
			this.left = left;
			this.right = right;
		}
		
		public Vector3 GetDirection(Direction direction){
			switch(direction){
				case Direction.Up: return up;
				case Direction.Down: return down;
				case Direction.Left: return left;
				case Direction.Right: return right; 
			}
			
			return Vector3.zero;
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
	public class Box{
		private const BoxColor COLOR = BoxColor.Red;
		private const string ID = "0000";
		
		[SerializeField] private BoxColor color = COLOR;
		[SerializeField] private string id = ID;
		
		public BoxColor GetColor() => color;
		public string GetFormatted() => $"{General.GetFormattedColor(color).ToUpper()}\n{id}";
		public string GetID() => string.IsNullOrEmpty(id) ? ID : id;
		
		public void SetID(string newID){
			id = newID;
		}		
		
		public void Clear(){
			color = COLOR;
			id = ID;
		}
		
		public void Transfer(Box box){
			color = box.color;
			id = box.id;
		}	
	}
	
	[System.Serializable]
	public class FootstepSound{
		public PhysicMaterial material;
		public AudioClip[] sounds;
	}
	
	[System.Serializable]
	public class CharacterOutput{
		public Image image;
		public TMP_Text name;
		public TMP_Text description;
	}
	
	[System.Serializable]
	public class Employee{
		private const string WORKPLACE = "Warehouse A-00";
		private const string ID = "9000";
		
		[SerializeField] private string workplace = WORKPLACE;
		[SerializeField] private string id = ID;
		
		public string GetWorkplace() => string.IsNullOrEmpty(workplace) ? WORKPLACE : workplace;
		public string GetID() => string.IsNullOrEmpty(id) ? ID : id;
	}

	[System.Serializable]
	public class Job{
		private const string NAME = "Warehouse Caretaker";
		private const string ID = "9000";
		
		[SerializeField] private string name = NAME;
		[SerializeField] private string id = ID;
		
		public string GetName() => string.IsNullOrEmpty(name) ? NAME : name;
		public string GetID() => string.IsNullOrEmpty(id) ? ID : id;
	}

	[System.Serializable]
	public class CharacterData{
		private const string NAME = "Jayden Doe";
		private const string GENDER = "Unknown";
		private const string DESCRIPTION = "Seja marginal, seja herói.";
		private static readonly Date BIRTHDAY = new Date(1999, 1, 1);
		
		[Header("Information")]
		[SerializeField] private Sprite photo;
		[SerializeField] private string name = NAME;
		[SerializeField] private string gender = GENDER;
		[SerializeField, TextArea(5, 10)] public string description = DESCRIPTION;
		[SerializeField] private Date birthday = BIRTHDAY;
		
		[Header("Job Information")]
		[SerializeField] private Job job;
		[SerializeField] private Employee employee;
		
		public string GetName() => string.IsNullOrEmpty(name) ? NAME : name;
		public string GetGender() => string.IsNullOrEmpty(gender) ? GENDER : gender;
		public string GetDescription() => string.IsNullOrEmpty(description) ? DESCRIPTION : description;
		
		public Date GetBirthday() => birthday.isNull() ? BIRTHDAY : birthday;
		public string GetFormattedBirthday() => GetBirthday().GetFormatted();
		public int GetAge(Date currentDate) => currentDate.year - birthday.year;
		
		public Sprite GetPhoto() => photo;
		
		public string GetBasicFormatted(Date currentDate){
			return
				$"Name: {GetName()}\tAge: ≈{GetAge(currentDate)}\n" +
				$"Gender: {GetGender()}";
		}

		public string GetFullFormatted(){
			return
				$"Gender: {GetGender()}\n" +
				$"Date of birth: {GetFormattedBirthday()}\n" +
				$"Current Job: {job?.GetName() ?? "None"} ({job?.GetID() ?? "N/A"})\n" +
				$"Workplace: {employee?.GetWorkplace() ?? "N/A"}";
		}
	}

	[System.Serializable]
	public class InteractableCharacter{
		private const string NAME = "Jayden Doe";
		private const float CHARACTERS_PER_SECOND = 5f;
		
		[Header("Information")]
		[SerializeField] private string name = NAME;
		[SerializeField] private float charactersPerSecond = CHARACTERS_PER_SECOND;
		[SerializeField] private Sprite photo;
		
		[Header("Dialogue")]
		[SerializeField, TextArea(2, 5)] public string[] smallTalk;

		public string GetName() => string.IsNullOrEmpty(name) ? NAME : name;
		public float GetCharactersPerSecond() => charactersPerSecond <= 0f ? CHARACTERS_PER_SECOND : charactersPerSecond;
		public Sprite GetPhoto() => photo;

		public string GetSmallTalk(){
			if (smallTalk == null || smallTalk.Length == 0){
				return string.Empty;				
			}
			
			return smallTalk[UnityEngine.Random.Range(0, smallTalk.Length)];
		}
	}
#endregion
}