Shader "Custom/Outline"{
	Properties{
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineSize ("Outline Size", Float) = 0.03
	}
	
	SubShader{
		Tags{
		"RenderType"="Opaque"
		}
		
		Pass{
			Cull Front
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f{
				float4 pos : SV_POSITION;
			};
			
			float _OutlineSize;
			fixed4 _OutlineColor;
			
			v2f vert(appdata v){
				v2f o;
				v.vertex.xyz += v.normal * _OutlineSize;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target{
				return _OutlineColor;
			}

			ENDCG
		}
	}
}