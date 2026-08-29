Shader "Custom/NoiseDither"{
	Properties{
		_Color ("Color", Color) = (1,1,1,1)
		_NoiseScale ("Noise Scale", Float) = 8
		_NoiseSpeed ("Noise Speed", Float) = 1
		_DitherStrength ("Dither Strength", Range(0,1)) = 0.5
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
	}

	SubShader{
		Tags{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}
		
		Pass{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			fixed4 _Color;
			float _NoiseScale;
			float _NoiseSpeed;
			float _DitherStrength;
			float _Cutoff;
			
			const float bayer[16] = {0,  8,  2, 10, 12,  4, 14,  6, 3, 11,  1,  9, 15,  7, 13,  5};
			
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};
			
			v2f vert(appdata v){
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);
				
				return o;
			}
			
			float hash21(float2 p){
				p = frac(p * float2(123.34, 456.21));
				p += dot(p, p + 45.32);

				return frac(p.x * p.y);
			}
			
			float noise(float2 p){
				float2 i = floor(p);
				float2 f = frac(p);
				
				f = f * f * (3.0 - 2.0 * f);
				
				float a = hash21(i);
				float b = hash21(i + float2(1, 0));
				float c = hash21(i + float2(0, 1));
				float d = hash21(i + float2(1, 1));
				
				return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
			}
			
			float bayerFour(float2 p){
				int x = (int)p.x & 3;
				int y = (int)p.y & 3;
				return bayer[y * 4 + x] / 16.0 - 0.5;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float2 noiseUV = i.uv * _NoiseScale + _Time.y * _NoiseSpeed;
				
				float n = noise(noiseUV);
				
				float2 pixel = screenUV * _ScreenParams.xy;
				float dither = (bayerFour(pixel) - 0.5) * _DitherStrength;
				
				float value = n + dither;
				float mask = step(_Cutoff, value);

				return _Color * mask;
			}
			ENDCG
		}
	}
}