Shader "Custom/TextureNoiseDither"{
	Properties{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_NoiseScale ("Noise Scale", Float) = 8
		_NoiseSpeed ("Noise Speed", Float) = 1
		_NoiseAmount ("Noise Amount", Range(0,1)) = 0.25
		_DitherStrength ("Dither Strength", Range(0,1)) = 0.5
		_Cutoff ("Cutoff", Range(0,1)) = 0.5
	}
	
	SubShader{
		Tags{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
			"CanUseSpriteAtlas" = "True"
			"PreviewType" = "Plane"
		}
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off
		
		Pass{
			Tags{
				"LightMode" = "ForwardBase"
			}
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _NoiseScale;
			float _NoiseSpeed;
			float _NoiseAmount;
			float _DitherStrength;
			float _Cutoff;
			
			const float bayer[16] = {
				0, 8, 2, 10,
				12, 4, 14, 6,
				3, 11, 1, 9,
				15, 7, 13, 5
			};
			
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};
			
			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 screenPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
			};
			
			v2f vert(appdata v){
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color * _Color;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
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
				return bayer[y * 4 + x] / 16.0;
			}
			
			fixed4 frag(v2f i) : SV_Target{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 color = tex * i.color;
				
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float2 pixel = screenUV * _ScreenParams.xy;
				
				float2 noiseUV = i.uv * _NoiseScale + _Time.y * _NoiseSpeed;
				
				float n = noise(noiseUV);
				
				float dither = bayerFour(pixel) * _DitherStrength;
				
				float value = n + dither;
				float mask = step(_Cutoff, value);
				
				float noiseFactor = lerp(1.0 - _NoiseAmount, 1.0, mask);
				
				color.rgb *= noiseFactor;
				
				float3 normal = normalize(i.worldNormal);
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				float light = max(0.0, dot(normal, lightDir));
				
				color.rgb *= _LightColor0.rgb * light;
				
				return color;
			}
			
			ENDCG
		}
	}
}