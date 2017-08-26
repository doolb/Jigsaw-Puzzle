Shader "Custom/puzzle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
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
		};

		float4 _MainTex_ST;
		v2f vert(appdata i)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}

		sampler2D _MainTex;
		fixed4  frag(v2f i) : SV_TARGET
		{
			fixed4 col = 1;
			col.rgb = tex2D(_MainTex,i.uv);
			col.rgb *= .6f;
			return col;
		}
		
		ENDCG
	}
		
	}
	FallBack "Diffuse"
}
