using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using MathLibrary;
using TMPro;

namespace GeneralLibrary{
	[Serializable]
	public class Date{
		public static int year;
		public static int month;
		public static int day;
		
		public static string GetDate(){
			return $"{month}/{day}/{year}";
		}
	}
	
	[Serializable]
	public class TimeData{
		public static int hours;
		public static int seconds;
		public static int milliseconds;
		
		public enum GetType{
			Simple,
			Complex
		}
		
		public static string GetTime(GetType type){
			switch(type){
				case GetType.Simple:
					return $"{hours}:{seconds}";
				
				case GetType.Complex:
					return $"{hours}:{seconds}:{milliseconds}";
			}
			
			return "nullpointer";
		}
	}
	
	public static class General{
		public static T RandomEnumValue<T>() where T : Enum{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values[UnityEngine.Random.Range(0, values.Length)];
		}
		
		public static string ToTitleCase(string text){
			TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
			return textInfo.ToTitleCase(text.ToLower());
		}
		
		public static void LockCursor(){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		
		public static void UnlockCursor(){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		
		public static void DoActionFromInput(Action function, InputAction action){
			if(!Singleton<InputManager>.Instance.GetActionKeyDown(action)){
				return;
			}
			
			function();
		}
	}
	
	public static class UserInterface{
		public static IEnumerator FadeImage(float[] alpha, float duration, Image image){
			float timeElapsed = 0f;
			Color color = image.color;
			
			while (timeElapsed < duration){
				timeElapsed += Time.deltaTime;
				
				color.a = Mathf.Lerp(alpha[0], alpha[1], timeElapsed / duration);
				image.color = color;
				
				yield return null;
			}
			
			color.a = alpha[1];
			image.color = color;
		}
	
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
	}
	
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	public static class DebugConsole{
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();
		
		public static void StartConsole(){
			AllocConsole();
		}
		
		public static void EndConsole(){
			FreeConsole();
		}
		
		public static void ThrowError(string text = "foobar"){
			Console.WriteLine($"[{DateTime.UtcNow}] => I_Error: {text}");
		}
		public static void SendLog(string text = "foobar"){
			Console.WriteLine($"[{DateTime.UtcNow}] => I_Log: {text}");
		}
	}
	#endif
}