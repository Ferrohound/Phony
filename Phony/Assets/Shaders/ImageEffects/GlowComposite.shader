Shader "Hidden/GlowComposite"
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _GlowPrePassTex;
			sampler2D _GlowBlurredTex;
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = v.uv;
				
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif
				
				
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//comment when fully understood
				fixed4 col = tex2D(_MainTex, i.uv);
				//subtract the original, crisp, prepass from the blurred one
				//to get a hard outline
				fixed4 glow = max(0, tex2D(_GlowBlurredTex, i.uv2) - 
					tex2D(_GlowPrePassTex, i.uv2));
				return col + glow * _Intensity;
			}
			ENDCG
		}
	}
}
