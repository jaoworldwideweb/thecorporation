using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using MathLibrary;

namespace GeneralLibrary{
	[Serializable]
	public class Date{
		public int year;
		public int month;
		public int day;
		
		public static string GetDate(){
			return $"{month}/{day}/{year}";
		}
	}
	
	[Serializable]
	public class Time{
		public int hours;
		public int seconds;
		public int milliseconds;
		
		public enum GetType{
			Simple,
			Complex
		}
		
		public static string GetTime(GetType type){
			switch(type){
				case GetType.Simple:
					return $"{hours}:{seconds}"
				
				case GetType.Complex:
					return $"{hours}:{seconds}:{milliseconds}"
			}
			
			return "nullpointer"
		}
	}
	
	public static class General{
		public static T RandomEnumValue<T>() where T : Enum{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values[UnityEngine.Random.Range(0, values.Length)];
		}
	}
	
	public static class Actions{
		public static void LockCursor(){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		
		public static void UnlockCursor(){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
	public static class UserInterface{
		public static IEnumerator FadeImage(float[] alpha, float duration, Image image){
			float elapsed = 0f;
			Color color = image.color;
			
			while (elapsed < duration){
				elapsed += Time.deltaTime;
				
				color.a = Mathf.Lerp(alpha[0], alpha[1], elapsed / duration);
				image.color = color;
				
				yield return null;
			}
			
			color.a = alpha[1];
			image.color = color;
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