Shader "Hidden/ChromaticAberration"
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
			uniform half2 u_redOffset;
			uniform half2 u_greenOffset;
			uniform half2 u_blueOffset;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				col.r = tex2D(_MainTex, i.uv + u_redOffset).r;
				col.g = tex2D(_MainTex, i.uv + u_greenOffset).g;
				col.b = tex2D(_MainTex, i.uv + u_blueOffset).b;
				col.a = tex2D(_MainTex, i.uv).a;
		
				return col;
			}
			ENDCG
		}
	}
}
