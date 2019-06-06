// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/movingShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TransitionTex ("Transition Texture", 2D) = "white" {}
		_Translation("Translation", Vector) = (0.5, 0.5, 0, 0)
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
            #pragma shader_feature ETC1_EXTERNAL_ALPHA

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _TransitionTex;
			float4 _TransitionTex_TexelSize;
			float4 _Translation;

			v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;

				fixed2 newuv = i.uv;
				newuv.x *= _TransitionTex_TexelSize.x / _MainTex_TexelSize.x;
				newuv.y *= _TransitionTex_TexelSize.y / _MainTex_TexelSize.y;
				fixed4 newcol = tex2D(_TransitionTex, (newuv + _Translation.xy * _Time.y));
				newcol.a = col.a;
				newcol.rgb *= col.a;

				return newcol;
			}
			ENDCG
		}
	}
}
