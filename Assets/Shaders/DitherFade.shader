Shader "Custom/DitherFade" {
	Properties{
		_MainTex ("Source", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Fade ("Fade", Range(0, 1)) = 1
		_DitherSize ("Dither Size", Range(1, 8)) = 1
	}
	
	SubShader{
		Tags{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"CanUseSpriteAtlas"="True"
		}
		
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _Color;
			float _Fade;
			float _DitherSize;
			
			const float bayer[16] = {
				0,  8,  2, 10,
				12, 4, 14, 6,
				3, 11, 1,  9,
				15, 7, 13, 5
			};
			
			struct appdata{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			float bayerFour(float2 p){
				int x = (int)p.x & 3;
				int y = (int)p.y & 3;
				return bayer[y * 4 + x] / 16.0;
			}
			
			fixed4 frag(v2f i) : SV_Target{
				float4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
				
				// make the pattern screen-space so it doesn't stretch with the image
				float2 screenPixel = floor(i.uv * _MainTex_TexelSize.zw / _DitherSize);
				float threshold = bayerFour(screenPixel);
				
				clip(col.a - threshold);
				
				return col;
			}
			ENDCG
		}
	}
}