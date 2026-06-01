using System;
using MathLibrary;

namespace TextLibrary{
	public static class cText{
		// part of the percentage stuff
		public static string PrintPercentage(float current, float max, string prefix = "out of"){
			return $"{cMath.CalculatePercentage(current, max)} {prefix} {max}%";
		}
		
		public static string PrintPercentage(int current, int max, string prefix = "out of"){
			return $"{cMath.CalculatePercentage(current, max)} {prefix} {max}%";
		}
		
		// regular stuff
		public static string ReadOutNum(int number, bool isProperCase = false){
			string[] words = {
				"zero", "one", "two",
				"three", "four", "five",
				"six", "seven","eight",
				"nine",
			};
			
			if (number < 0 || number >= words.Length){
				throw new ArgumentOutOfRangeException(nameof(number));				
			}
			
			string result = words[number];
			return isProperCase ? char.ToUpper(result[0]) + result.Substring(1) : result;
		}
	}
}
