using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using MathLibrary;
using TMPro;

namespace GeneralLibrary{
	// kind of like a vector2 but with some other goodies!
	[Serializable]
	public struct dfloat{
		public float a;
		public float b;
		private float memory;
		
		public dfloat(float a, float b){
			this.a = a;
			this.b = b;
			this.memory = 0f;
		}
		
		// calculations
		public float Sum(){
			return a + b;
		}
		
		public float Subtract(bool isInverted = false){
			return isInverted ? b - a : a - b;
		}
		
		public float Divide(bool isInverted = false){
			return isInverted ? b / a : a / b;
		}
		
		public float Multiply(){
			return a * b;
		}
		
		// operations
		public void Store(float push){
			memory = push;
		}
		
		public void Push(ref float point){
			point = memory;
		}
		
		public float Get(){
			return memory;
		}
		
		public void Clear(bool full = false){
			a = 0f;
			b = 0f;
			
			if(!full){
				return;
			}
			
			memory = 0f;
		}
	}
	
	[Serializable]
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
	
	[Serializable]
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
		// void wrapper
		public static void FadeImage(Image image, dfloat alpha, float duration = 5f){
			CoroutineRunner.Instance.StartCoroutine(IFadeImage(image, alpha, duration));
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
		
		// shit so long i was forced to snip it :sob:
		public static IEnumerator IDither(GameObject previous, GameObject next,
			Animator previousAnimator, Animator nextAnimator,
			GraphicRaycaster previousRaycaster, GraphicRaycaster nextRaycaster, float duration = 1f){
			next.SetActive(true);

			previousAnimator.SetBool("Transition", false);
			nextAnimator.SetBool("Transition", true);

			previousRaycaster.enabled = false;
			nextRaycaster.enabled = false;

			yield return new WaitForSeconds(duration);

			previous.SetActive(false);
			nextRaycaster.enabled = true;
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