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
	}
	
	public static class cGeneral{
		public static T RandomEnumValue<T>() where T : Enum{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values[UnityEngine.Random.Range(0, values.Length)];
		}
		
		public static string GetDate(Date date){
			return $"{date.month}, {date.day}, {date.year}";
		}
	}
	
	public static class gActions{
		public static void LockCursor(){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		public static void UnlockCursor(){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
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