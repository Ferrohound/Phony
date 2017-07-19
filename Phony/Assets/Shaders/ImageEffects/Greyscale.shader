Shader "Hidden/Greyscale"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Blend ("BlendValue", Range(0,1)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			
			//write 9 to this buffer wherever 9s don't exist and 0s where they do
			//essentially invert the buffer
			Stencil
			{
				Ref 2
				Comp notEqual
				Pass keep
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			uniform float _Blend;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float lumo = col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
				float3 grey = float3(lumo, lumo, lumo);
				
				float3 end = col;
				end.rgb = lerp(col.rgb, grey, _Blend);
				
				return fixed4(end, 1);
			}
			ENDCG
		}
	}
}
