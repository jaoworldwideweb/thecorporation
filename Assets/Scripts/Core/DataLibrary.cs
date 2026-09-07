using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace DataLibrary{
	public unsafe struct dfloat{
		public unsafe float a;
		public unsafe float b;
		private unsafe float memory;
		
		public unsafe dfloat(float a, float b){
			this.a = a; this.b = b; this.memory = 0f;
		}
		
		// calculations
		public float Sum() => a + b;
		public float Multiply() => a * b;
		
		public float Subtract(bool inv = false) => inv ? b - a : a - b;
		public float Divide(bool inv = false) =>  inv ? b / a : a / b;
		
		public float Floor(float value) => (int)value - (value < (int)value ? 1 : 0);
		public float Round(float value) => (int)(value + (value >= 0f ? 0.5f : -0.5f));
		
		// operations
		public void Store(float push) => memory = push;
		public float Get() => memory;
		
		public void Push(ref float point) => point = memory;
		public void Clear(bool full = false){
			a = 0f;
			b = 0f;
			if(!full){
				return;
			}
			memory = 0f;
		}
	}
	
	[System.Serializable]
	public struct dint{
		public float a;
		public float b;
		private float memory;
		
		public dint(float a, float b){
			this.a = a; this.b = b; this.memory = 0;
		}
		
		// calculations
		public float Sum() => a + b;
		public float Multiply() => a * b;
		
		public float Subtract(bool inv = false) => inv ? b - a : a - b;
		public float Divide(bool inv = false) =>  inv ? b / a : a / b;
		
		// operations
		public void Store(float push) => memory = push;
		public float Get() => memory;
		
		public void Push(ref float point) => point = memory;
		public void Clear(bool full = false){
			a = 0;
			b = 0;
			if(!full){
				return;
			}
			memory = 0;
		}
	}
}