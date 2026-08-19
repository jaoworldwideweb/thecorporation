Shader "Custom/RealShader" {
	Properties{
		_MainTex ("Source", 2D) = "white" {}
		_PixelSize ("Pixel Size", Range(1, 16)) = 3
		_ColorSteps ("Color Steps", Range(2, 32)) = 8
		_Dither ("Dither", Range(0, 1)) = 0.35
		_Grain ("Grain", Range(0, 1)) = 0.08
		_Contrast ("Contrast", Range(0.5, 2)) = 1.15
		_Saturation ("Saturation", Range(0, 2)) = 0.8
		_Vignette ("Vignette", Range(0, 1)) = 0.35
		_Warp ("Screen Warp", Range(0, 0.05)) = 0.008
		_Chromatic ("Chromatic Aberration", Range(0, 0.02)) = 0.002
	}

	SubShader{
		Cull Off
		ZWrite Off
		ZTest Always

		Pass{
			CGPROGRAM // :eyes: lowk fun asf

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _PixelSize;
			float _ColorSteps;
			float _Dither;
			float _Grain;
			float _Contrast;
			float _Saturation;
			float _Vignette;
			float _Warp;
			float _Chromatic;

			const float bayer[16] = {0,  8,  2, 10, 12,  4, 14,  6, 3, 11,  1,  9, 15,  7, 13,  5};

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex); // me when apis
				o.uv = v.uv;
				return o;
			}

			// alldat just to save 2 cpu cycles :joy:
			float hash21(float2 p){
				p = frac(p * float2(123.34, 456.21));
				p += dot(p, p + 45.32);

				return frac(p.x * p.y);
			}

			float bayerFour(float2 p){
				int x = (int)p.x & 3;
				int y = (int)p.y & 3;
				return bayer[y * 4 + x] / 16.0 - 0.5;
			}

			float3 Sample(float2 uv){
				return tex2D(_MainTex, uv).rgb;
			}

			// almost called this 'fag' in code but remembered one of the rules of clean code
			// ^ this itself breaks the rules of clean code
			fixed4 frag(v2f i) : SV_Target{ 
				float2 uv = i.uv;
				float2 centered = uv - 0.5;
				float r2 = dot(centered, centered);

				uv += centered * r2 * _Warp;
				uv = saturate(uv); // dont you DARE sample outside.

				float2 resolution = _MainTex_TexelSize.zw;
				float2 pixelCount = resolution / _PixelSize;

				uv = floor(uv * pixelCount) / pixelCount;

				float2 ca = centered * _Chromatic;

				// get colors from uv
				float red = Sample(uv + ca).r;
				float green = Sample(uv).g;
				float blue = Sample(uv - ca).b;
				float3 col = float3(red, green, blue);

				col = (col - 0.5) * _Contrast + 0.5;
				float luminance = dot(col, float3(0.299, 0.587, 0.114));
				col = lerp(luminance.xxx, col, _Saturation);

				// here comes the fun part!
				float2 screenPixel = floor(uv * resolution);

				float dither = bayerFour(screenPixel) * _Dither;
				col += dither / _ColorSteps;

				// quantization
				col = floor(col * _ColorSteps + 0.5)	/ _ColorSteps;
				col = saturate(col);

				float noise = hash21(screenPixel + _Time.y * 17.0);
				col += (noise - 0.5) * _Grain;

				// i dont know if i keep this
				float2 vignetteUV = i.uv - 0.5;
				float vignette = 1.0 - dot(vignetteUV, vignetteUV) * 1.8;
				vignette = lerp(1.0, saturate(vignette), _Vignette);
				col *= vignette;

				return float4(saturate(col), 1.0);
			}
			ENDCG
		}
	}
}