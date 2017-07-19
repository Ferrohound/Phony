Shader "Hidden/SinComposite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Equal
				Pass keep	
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _SinPPTex;
			sampler2D _SinnedTex;
			sampler2D _TempTex0;
			
			float2 _MainTex_TexelSize;
			float _Intensity;

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 sin = tex2D(_SinnedTex, i.uv);
				fixed4 sinPP = tex2D(_SinPPTex, i.uv);
				// just invert the colors
				//col = 1 - col;
				return col + sin;
			}
			ENDCG
		}
	}
}
