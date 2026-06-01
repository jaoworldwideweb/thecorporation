using UnityEngine;
using System;
using MathLibrary;

namespace GeneralLibrary{
	public static class cGeneral{
		public static T RandomEnumValue<T>() where T : Enum{
			T[] values = (T[])Enum.GetValues(typeof(T));
			return values[UnityEngine.Random.Range(0, values.Length)];
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
}