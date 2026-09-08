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
				
				// had to do this since pre-calculation doesn't look good
				// with this type of shader.
				
				if(x == 0){
					if(y == 0) return 0.5 / 16.0;
					if(y == 1) return 12.5 / 16.0;
					if(y == 2) return 3.5 / 16.0;
					return 15.5 / 16.0;
				}
				
				if(x == 1){
					if(y == 0) return 8.5 / 16.0;
					if(y == 1) return 4.5 / 16.0;
					if(y == 2) return 11.5 / 16.0;
					return 7.5 / 16.0;
				}
				
				if(x == 2){
					if(y == 0) return 2.5 / 16.0;
					if(y == 1) return 14.5 / 16.0;
					if(y == 2) return 1.5 / 16.0;
					return 13.5 / 16.0;
				}
				
				if(y == 0) return 10.5 / 16.0;
				if(y == 1) return 6.5 / 16.0;
				if(y == 2) return 9.5 / 16.0;
				return 5.5 / 16.0;
			}
			
			fixed4 frag(v2f i) : SV_Target{
				float4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
				
				float2 pixel = floor(i.uv * 512.0 / _DitherSize);
				float threshold = bayerFour(pixel);
				
				clip(_Fade - threshold);
				
				return col;
			}
			ENDCG
		}
	}
}
