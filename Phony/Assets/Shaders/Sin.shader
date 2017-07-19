Shader "Hidden/Sin"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off 
		ZTest Always
		Tags { "RenderType"="Opaque"
			"Queue" = "Geometry+2"
			"Sin" = "True" }
		Pass
		{
			/*Stencil
			{
				Ref 1
				Comp Equal
				Pass keep	
			}*/
			
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
			
			//original properties are all still there
			sampler2D _MainTex;
			sampler2D _SinPPTex;
			half4 _Color;
	
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 wat = tex2D(_SinPPTex, i.uv);
				// just return the normal colour for now
				//to allow for depth, have some integer that's being
				//set to 1 or 0 and multiply the whole thing by this so
				//the outline isn't overlaid on top of everything
				//return col * _Color;
				return col * _Color/* * wat*/;
			}
			ENDCG
		}
	}
	
	SubShader
	{
		// No culling or depth
		//Cull Off 
		ZWrite Off ZTest Always
		Tags { "RenderType"="Opaque"
			"Queue" = "Geometry"
			"Sin" = "False" }
		Pass
		{
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
			
			//original properties are all still there
			sampler2D _MainTex;
			half4 _Color;
	
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
