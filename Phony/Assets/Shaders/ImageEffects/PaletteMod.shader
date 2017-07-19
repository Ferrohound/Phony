// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//more like Sin City shader
Shader "Custom/PaletteMod"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_R ("Red", Range(0, 255)) = 0
		_G ("Green", Range(0, 255)) = 0
		_B ("Blue", Range(0, 255)) = 0
		_Black ("Darken", Range(1, 10)) = 1
		
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Tags { "RenderType"="Opaque" "Queue"="Geometry+2"}

		Pass
		{
			//check stencil buffer for 9, if it's there, leave it as is
			Stencil
			{
				Ref 1
				Comp equal
				Pass keep
			}
			
			/*
				write stencil like
				Stencil
				{
					Ref 9
					Comp always
					Pass replace	
				}
			*/
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform float _R;
			uniform float _G;
			uniform float _B;
			uniform float _Black;

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

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col.r += (_R/255) * (1/_Black);
				col.g += (_G/255) * (1/_Black);
				col.b += (_B/255) * (1/_Black);
				return col;
			}
			ENDCG
		}
	}
}
