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
		
		// calculate percentage fucntions
		// p = (n / l) * p₁
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
		
		// regular math stuff
		// good for projectiles from heights and calculating distance
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CalculateHypotnuse(float adjacentCathetus, float oppositeCathetus){
			// h = √(c₁²+c₂²)
			return HighMath.SquareRoot(HighMath.Square(adjacentCathetus) + HighMath.Square(oppositeCathetus));
		}
	#endregion
	
	#region UI
		// it's cool that ease out functions are similar to eachother (well a few)
		public static float EaseOutQuad(float startValue){
			float t = 1f - startValue;
			return 1f - HighMath.Square(t);
		}
		
		public static float EaseOutCubic(float startValue){
			float t = 1f - startValue;
			return 1f - HighMath.Cube(t);
		}
	#endregion
	}
	
	public static class HighMath{
	#region Main
		// i use these a lot for my formulas :-0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Square(float startValue){
			// x²
			return startValue * startValue;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cube(float startValue){
			// x³
			return startValue * startValue * startValue;
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
			startValue *= 1.5f - xHalf * startValue * startValue;
			
			return startValue;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SquareRoot(float startValue){
			return startValue * InvSqrt(startValue);
		}
	#endregion
		
	#region Trigonometry
		private const float pi = 3.14159265358979323846f;
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