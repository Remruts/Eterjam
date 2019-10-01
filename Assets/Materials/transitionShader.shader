// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/transitionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_TransitionTex ("Transition Texture", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0.0, 1.0)) = 0.0
		_Translation("Translation", Vector) = (0.5, 0.5, 0, 0)
		_Fade ("Fade", Range(0, 1)) = 1
        _Scale ("Scale", Range(0.5, 3)) = 1		
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

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _TransitionTex;
			float4 _TransitionTex_TexelSize;
			float4 _Translation;
			sampler2D _MaskTex;
			float _Cutoff;
			float _Fade;
            float _Scale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 transit = tex2D(_MaskTex, i.uv);

				if (transit.b < _Cutoff){
					fixed2 newuv = i.uv;
					newuv.x *= _TransitionTex_TexelSize.x / _MainTex_TexelSize.x / _Scale;
					newuv.y *= _TransitionTex_TexelSize.y / _MainTex_TexelSize.y / _Scale;
					return lerp(col, tex2D(_TransitionTex, newuv + _Translation.xy * _Time.y), _Fade);
				}
				return col;
			}
			ENDCG
		}
	}
}
