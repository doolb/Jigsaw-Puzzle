Shader "Custom/puzzle" {
	Properties {
		// 主颜色值
		_Color ("Color", Color) = (1,1,1,1)

		// 拼图图像纹理
		_MainTex ("拼图图像", 2D) = "white" {}

		// 拼图形状纹理
		_MarkTex("拼图形状", 2D) = "white"{}

		// 高光颜色
		_HighLight("高光颜色", Float) = .8

		// 阴影颜色
		_Shadow("阴影颜色",Float) = 0.3

		// 特殊颜色
		_Special("特殊颜色",Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Cull off
		
	PASS
	{
		CGPROGRAM
		#include "UnityCG.cginc"

		#pragma vertex vert
		#pragma fragment frag

		// 应用程序数据
		struct appdata
		{
			float4 vertex : POSITION;	// 顶点
			float2 uv:TEXCOORD0;		// uv
		};

		// 顶点着色器数据
		struct v2f
		{
			float4 pos : SV_POSITION;	// 顶点
			float2 uv  : TEXCOORD0;		// 拼图图像纹理 uv
			float2 muv : TEXCOORD1;		// 拼图形状纹理 uv
		};

		// 顶点着色器
		float4 _MainTex_ST;
		float4 _MarkTex_ST;
		v2f vert(appdata i)
		{
			v2f o;
			
			// 计算顶点
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);

			// 把原图移动到中心
			_MainTex_ST.zw -= _MainTex_ST.xy / 4;
			
			// 计算 拼图图像纹理 uv
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			
			// 计算 拼图图像纹理 uv
			o.muv = TRANSFORM_TEX(i.uv, _MarkTex);

			return o;
		}

		// 片段着色器
		sampler2D _MainTex;	// 拼图图像纹理 uv
		sampler2D _MarkTex;	// 拼图形状纹理 uv
		fixed4 	_Color;		// 主颜色值
		float 	_HighLight;	// 高光颜色
		float 	_Shadow;	// 阴影颜色
		float4 	_Special;	// 特殊颜色
		fixed4  frag(v2f i) : SV_TARGET
		{
			fixed4 col;

			// 采样 拼图图像
			col.rgb = tex2D(_MainTex,i.uv);
			col.rgb *= _Color;

			// 采样 拼图形状
			fixed4 mark = tex2D(_MarkTex,i.muv);

			// 合并 拼图图像 和 拼图形状
			col.a = mark.a;
			
			// 计算 阴影颜色
			col.rgb -= mark.b * _Shadow;

			// 计算 高光颜色
			col.rgb += mark.g * _HighLight;
			
			// 计算 特殊颜色
			if(mark.r > 0.5)	
				col.rgb = mark.r * _Special;

			// 返回 最终的 颜色值
			return col;
		}
		
		ENDCG
	}
		
	}
	FallBack "Diffuse"
}
