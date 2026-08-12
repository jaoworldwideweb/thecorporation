using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace MathLibrary{
	public static class CommonMath{
	#region Main
		public static bool IsValueNullOrZero(this float startValue){
			return startValue == 0f;
		}
		
		public static bool IsValueNullOrZero(this int startValue){
			return startValue == 0;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculatePercentage(float current, float max){
			if(max.IsValueNullOrZero()){
				return 0f;
			}
			return (current / max) * 100f;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CalculatePercentage(int current, int max){
			if(max.IsValueNullOrZero()){
				return 0;
			}
			return (int)System.Math.Round((float)current / max * 100f);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculateHypotnuse(float adjacentCathetus, float oppositeCathetus){
			return HighMath.SquareRoot(HighMath.Square(adjacentCathetus) + HighMath.Square(oppositeCathetus));
		}
	#endregion
	
	#region UI
		public delegate float EaseFunction(float t);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Linear(float t){
			return t;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutQuad(float t){
			t = 1f - t;
			return 1f - HighMath.Square(t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInQuad(float t){
			return HighMath.Square(t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInOutQuad(float t){
			return t < 0.5f ? 2f * HighMath.Square(t) : 1f - HighMath.ComplexAproximatePower(-2f * t + 2f, 2) * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutCubic(float t){
			t = 1f - t;
			return 1f - HighMath.Cube(t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInCubic(float t){
			return HighMath.Cube(t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseOutExpo(float t){
			return t >= 1f ? 1f : 1f - HighMath.ComplexAproximatePower(2f, (int)(-10f * t));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInExpo(float t){
			return t <= 0f ? 0f : HighMath.ComplexAproximatePower(2f, (int)(10f * (t - 1f)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float EaseInOutSine(float t){
			return -(HighMath.Cosine(HighMath.pi * t) - 1f) * 0.5f;
		}
	#endregion
	}
	
	public static class HighMath{
	#region Main
		// i use these a lot for my formulas :-0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Square(float startValue){
			return startValue * startValue;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cube(float startValue){
			return startValue * startValue * startValue;
		}
		
		private const float ln2 = 0.6931471805599453f;
		private const float invLn2 = 1.4426950408889634f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Exponent(float x){
			if (x == 0f){
				return 1f;
			}
			
			int whole = (int)x;
			float frac = x - whole;
			float fracExp = 1f + frac + Square(frac) * 0.5f + Cube(frac) * (1f / 6f) + CalculatePower(frac, 4) * (1f / 24f) + CalculatePower(frac, 5) * (1f / 120f); // wow

			return fracExp * CalculatePower(2f, (int)(whole * invLn2));
		}
		
		// this feels cursed for some reason... to badd!
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Log(float x){
			if (x <= 0f){
				return float.NaN;
			}
			
			float y = 0f;
			
			while (x > 2f){
				x *= 0.5f;
				y += ln2;
			}
			
			while (x < 1f){
				x *= 2f;
				y -= ln2;
			}
			
			float z = x - 1f;
			float z2 = Square(z);
			float z3 = z2 * z;
			float z4 = z3 * z;
			float z5 = z4 * z;

			return y + z - z2 * 0.5f + z3 / 3f - z4 * 0.25f + z5 * 0.2f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ComplexAproximatePower(float value, float exponent){
			if (value <= 0f){
				return 0f;
			}

			return Exponent(exponent * Log(value));
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculatePower(float startValue, int exponent){
			float result = 1f;
			
			while (exponent > 0){
				if ((exponent & 1) != 0){
					result *= startValue;					
				}
				startValue *= startValue;
				exponent >>= 1;
			}
			
			return result;
		}
		
		// quake 3 fstinvsqrt my beloved <3
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float InvSqrt(float startValue){
			float xHalf = 0.5f * startValue;
			int i = *(int*)&startValue;
			i = 0x5f3759df - (i >> 1);
			
			startValue = *(float*)&i;
			startValue *= 1.5f - xHalf * Square(startValue);
			
			return startValue;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SquareRoot(float startValue){
			return startValue * InvSqrt(startValue);
		}
	#endregion
		
	#region Trigonometry
		public const float pi = 3.14159265358979323846f;
		private const float b = 4f / pi;
		private const float c = -4f / (pi * pi);
		private const float p = 0.225f;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sine(float startValue){
			startValue %= 2f * pi;
			float y = b * startValue + c * startValue * System.Math.Abs(startValue);

			return p * (y * System.Math.Abs(y) - y) + y;
		}
		
		// cos(x) = sin(x + pi/2)
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cosine(float startValue){
			return Sine(startValue + 1.5707963267948966f);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Tangent(float startValue){
			return Sine(startValue) / Cosine(startValue);
		}
	#endregion
	}
}