using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace MathLibrary{
#region Structs
	[System.Serializable]
	public unsafe struct dfloat{
		public unsafe float a;
		public unsafe float b;
		private unsafe float memory;
		
		public unsafe dfloat(float a, float b){
			this.a = a; this.b = b; this.memory = 0f;
		}
		
		// bools
		public bool isClear() => a == 0f && b == 0f;
		public bool isMemoryClear() => memory == 0f;
		public bool isEqual() => a == b;
		
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
		
		// bools
		public bool isClear() => a == 0 && b == 0;
		public bool isMemoryClear() => memory == 0;
		public bool isEqual() => a == b;
		
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
#endregion

	public static class CommonMath{
	#region BasicMethods
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CalculatePercentage(int current, int max) => (int)System.Math.Round((float)current / max * 100f);
		
		public static int GetRandom() => UnityEngine.Random.Range(int.MinValue , int.MaxValue);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Absolute(float input) => input < 0f ? input + (input * 2) : input;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Negative(float input) => input > 0f ? input - (input * 2) : input;
	#endregion
	
	#region BitOperations
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float Int32BitsToSingle(int bits){
			return *(float*)&bits;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe int SingleToInt32Bits(float value){
			return *(int*)&value;
		}
	#endregion
	
	#region EaseFunctions
		public delegate float EaseFunction(float t);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Linear(float t) => t;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutQuad(float t) => 1f - HighMath.Square(1f - t);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInQuad(float t) => HighMath.Square(t);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInOutQuad(float t) => t < 0.5f ? 2f * HighMath.Square(t) : 1f - HighMath.ComplexAproximatePower(-2f * t + 2f, 2) * 0.5f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutCubic(float t) => 1f - HighMath.Cube(1f - t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInCubic(float t) => HighMath.Cube(t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutExpo(float t) => t >= 1f ? 1f : 1f - HighMath.ComplexAproximatePower(2f, (int)(-10f * t));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInExpo(float t) => t <= 0f ? 0f : HighMath.ComplexAproximatePower(2f, (int)(10f * (t - 1f)));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInOutSine(float t) => -(HighMath.Cosine(HighMath.PI * t) - 1f) * 0.5f;
	#endregion
	}
	
	public static class HighMath{
	#region MathConstants
		private const float LN2 = 0.6931471805599453f;
		private const float INV_LN2 = 1.4426950408889634f;
		public const float PI = 3.14159265358979323846f;
		
		private const float B = 4f / PI;
		private const float C = -4f / (PI * PI);
		private const float P = 0.225f;
	#endregion
	
	#region BasicFunctions
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Square(float input) => input * input;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cube(float input) => input * input * input;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculatePower(float input, int exponent){
			float result = 1f;
			
			while (exponent > 0){
				if((exponent & 1) != 0){
					result *= input;					
				}
				input *= input;
				exponent >>= 1;
			}
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Root(float input) => input * InvSqrt(input);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float InvSqrt(float input){
			float xHalf = 0.5f * input;
			int i = *(int*) & input;
			
			i = 0x5f3759df - (i >> 1);
			
			input = *(float*)&i;
			input *= 1.5f - xHalf * Square(input);
			
			return input;
		}
	#endregion
	
	#region HighMath
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Exponent(float input){
			if (input == 0f){
				return 1f;
			}
			
			int k = (int)(input * INV_LN2 + (input >= 0f ? 0.5f : -0.5f));
			float r = input - k * LN2;
			
			float rExp = 1f + r * (1f + r * (0.5f + r * (0.16666667f + r * (0.04166667f + r * 0.00833333f))));
			
			if (k < -126){
				return 0f;
			}
			
			if (k > 127){
				return float.PositiveInfinity;
			}
			
			int bits = (k + 127) << 23;
			float twoToK = CommonMath.Int32BitsToSingle(bits);
			return rExp * twoToK;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Log(float input){
			if (input <= 0f){
				return float.NaN;
			}
			
			int bits = CommonMath.SingleToInt32Bits(input);
			int exponent = ((bits >> 23) & 255) - 127;
			
			int mantissaBits = (bits & 0x007FFFFF) | (127 << 23);
			float mantissa = CommonMath.Int32BitsToSingle(mantissaBits);
			
			if (mantissa > 1.41421356f){
				mantissa *= 0.5f;
				exponent++;
			}

			float z = mantissa - 1f;
			float logMantissa = z * (1f - z * (0.5f - z * (0.33333333f - z * (0.25f - z * 0.2f))));

			const float LN2 = 0.69314718f;
			return logMantissa + exponent * LN2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ComplexAproximatePower(float value, float exponent){
			if (value <= 0f){
				return 0f;
			}

			return Exponent(exponent * Log(value));
		}
	#endregion
	
	#region Trig 
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sine(float input){
			input %= 2f * PI;
			float y = B * input + C * input * MathLibrary.CommonMath.Absolute(input);

			return P * (y * MathLibrary.CommonMath.Absolute(y) - y) + y;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cosine(float input){
			return Sine(input + 1.5707963267948966f);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Tangent(float input){
			return Sine(input) / Cosine(input);
		}
	#endregion
	}
}