Shader "Custom/puzzle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}

		_MarkTex("Puzzle Mark Texture", 2D) = "white"{}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
	PASS
	{
		CGPROGRAM
		#include "UnityCG.cginc"

		#pragma vertex vert
		#pragma fragment frag

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv  : TEXCOORD0;
			float2 muv : TEXCOORD1;
		};

		float4 _MainTex_ST;
		float4 _MarkTex_ST;
		v2f vert(appdata i)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			o.muv = TRANSFORM_TEX(i.uv, _MarkTex);
			return o;
		}

		sampler2D _MainTex;
		sampler2D _MarkTex;
		fixed4 _Color;
		fixed4  frag(v2f i) : SV_TARGET
		{
			fixed4 col;
			col.rgb = tex2D(_MainTex,i.uv);
			col.rgb *= _Color;

			fixed4 mark = tex2D(_MarkTex,i.muv);
			col.a = mark.a;

			return col;
		}
		
		ENDCG
	}
		
	}
	FallBack "Diffuse"
}
