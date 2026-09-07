using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using MathLibrary;
using GameLibrary;
using TMPro;

namespace GeneralLibrary{
#region Structs
	// might be useful for dialouge?
	[System.Serializable]
	public struct dstring{
		public string a;
		public string b;
		public string memory;
		
		public unsafe dstring(string a, string b){
			this.a = a; this.b = b; this.memory = null;
		}
		
		// bools
		public bool isClear() => a == null && b == null;
		public bool isMemoryClear() => memory == null;
		public bool isEqual() => a == b;
		
		// operations
		public void Store(string push) => memory = push;
		public string Get() => memory;
		
		public void Push(ref string point) => point = memory;
		public void Clear(bool full = false){
			a = null;
			b = null;
			
			if(!full){
				return;
			}
			
			memory = null;
		}
	}
	
	[System.Serializable]
	public struct Date{
		public int year;
		public int month;
		public int day;
		
		public Date(int year, int month, int day){
			this.year = year;
			this.month = month;
			this.day = day;
		}
		
		public string GetDate(){
			return $"{month}/{day}/{year}";
		}
	}
	
	[System.Serializable]
	public struct TimeData{
		public int hours;
		public int seconds;
		public int milliseconds;
		
		public TimeData(int hours, int seconds, int milliseconds){
			this.seconds = seconds;
			this.hours = hours;
			this.milliseconds = milliseconds;
		}
		
		public string Simple(){
			return $"{hours}:{seconds}";
		}
		
		public string Complex(){
			return $"{hours}:{seconds}:{milliseconds}";
		}
	}
#endregion
	
	public static class General{
	#region General
		public static T RandomEnumValue<T>() where T : Enum{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values[UnityEngine.Random.Range(0, values.Length)];
		}
		
		public static T GetComponentFromObject<T>(string name) where T : Component{
			return GameObject.Find(name).GetComponent<T>();
		}
		
		public static void LockCursor(){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
				
		public static void UnlockCursor(){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		
		public static void DoRaycastForObject(Action<RaycastHit> function, Camera camera, Transform playerTransform, float distance = 10f){
			if (!Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) || Time.timeScale == 0f){
				return;
			}
			
			Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
			
			if (!Physics.Raycast(ray, out RaycastHit hit)){
				return;				
			}
			if (hit.distance > distance){
				return;				
			}
			
			function(hit);
		}
		
		public static void DoActionFromInput(Action function, InputAction action){
			if (!Singleton<InputManager>.Instance.GetActionKeyDown(action)){
				return;
			}
			
			function();
		}
		
		public static void DoActionFromInput<T>(Action<T> function, InputAction action, T arg){
			if (!Singleton<InputManager>.Instance.GetActionKeyDown(action)){
				return;
			}
			
			function(arg);
		}	
	#endregion
	
	#region TextManipulation
		public static string ToTitleCase(string text){
			TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
			return textInfo.ToTitleCase(text.ToLower());
		}
		
		public static string ReadOutNumber(int number, bool isProperCase = false){
			string[] words = {
				"zero", "one", "two",
				"three", "four", "five",
				"six", "seven","eight",
				"nine",
			};
			
			if (number < 0 || number >= words.Length){
				return "not implemented";
			}
			
			string result = words[number];
			return isProperCase ? char.ToUpper(result[0]) + result.Substring(1) : result;
		}
		
		public static string GetFormattedColor(BoxColor color){
			string[] words = {
				"Red",
				"Green",
				"Blue",
				"Orange",
				"Light Red",
				"Light Green",
				"Light Blue",
				"Light Orange"
			};
			
			return words[(int)color];
		}
	#endregion
	}
	
	public static class UserInterface{
	#region ImageManipulation
		public static void FadeImage(Image image, dfloat alpha, float duration = 5f){
			CoroutineRunner.Instance.StartCoroutine(IFadeImage(image, alpha, duration));
		}
		
		public static void DitherImage(Image image, dfloat alpha, float duration = 5f){
			CoroutineRunner.Instance.StartCoroutine(IDitherImage(image, alpha, duration));
		}
		
		public static IEnumerator IFadeImage(Image image, dfloat alpha, float duration){
			float timeElapsed = 0f;
			Color color = image.color;
			
			while (timeElapsed < duration){
				
				timeElapsed += Time.deltaTime;
				
				color.a = Mathf.Lerp(alpha.a, alpha.b, timeElapsed / duration);
				image.color = color;
				
				yield return null;
			}
			
			color.a = alpha.b;
			image.color = color;
		}
		
		public static IEnumerator IDitherImage(Image image, dfloat fade, float duration){
			float timeElapsed = 0f;
			
			Material material = image.material;
			int fadeID = Shader.PropertyToID("_Fade");
			
			while (timeElapsed < duration){
				timeElapsed += Time.deltaTime;
				
				float value = Mathf.Lerp(fade.a, fade.b, timeElapsed / duration);
				material.SetFloat(fadeID, value);
				
				yield return null;
			}
			
			material.SetFloat(fadeID, fade.b);
		}
	#endregion
	
	#region ObjectMovement	
		public static IEnumerator MoveObject(RectTransform rectTransform, Vector2 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			float timeElapsed = 0f;		
			Vector2 startPos = rectTransform.anchoredPosition;
			
			while (timeElapsed < time){
				float t = timeElapsed / time;
				float easedT = easing(t);

				rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startPos.x, targetPosition.x, easedT), Mathf.Lerp(startPos.y, targetPosition.y, easedT));

				timeElapsed += Time.deltaTime;
				yield return null;
			}
			
			rectTransform.anchoredPosition = targetPosition;
		}
		
		public static IEnumerator Move3DObject(Transform transform, Vector3 targetPosition, CommonMath.EaseFunction easing, float time = 1f){
			float timeElapsed = 0f;			
			Vector3 startPos = transform.localPosition;
			
			while (timeElapsed < time){
				float t = timeElapsed / time;
				float easedT = easing(t);

				transform.localPosition = new Vector3(Mathf.Lerp(startPos.x, targetPosition.x, easedT), Mathf.Lerp(startPos.y, targetPosition.y, easedT), Mathf.Lerp(startPos.z, targetPosition.z, easedT));

				timeElapsed += Time.deltaTime;
				yield return null;
			}
			
			transform.localPosition = targetPosition;
		}
	#endregion
	
	#region Misc.
		public static IEnumerator PlayVideo(VideoPlayer player, VideoClip file, bool holdWhilePlaying = true){
			player.clip = file;
			player.Prepare();
			
			// wow nullchecking, really cool
			while (!player.isPrepared){
				yield return null;
			}
			
			player.Play();
			
			if(!holdWhilePlaying){
				yield break;
			}
			
			while (player.isPlaying){
				yield return null;
			}
		}
	#endregion
	}
}