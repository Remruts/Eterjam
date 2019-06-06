// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        // Add values to determine if outlining is enabled and outline color.
        _Outline ("Outline", Float) = 1
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size", Range(0.5, 3)) = 1
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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            float _Outline;
            float _OutlineSize;
            fixed4 _OutlineColor;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D (_MainTex, IN.texcoord) * IN.color;
                if (_Outline){

                    float alpha = 8 * tex2D(_MainTex, IN.texcoord).a;

                    float coordX = _MainTex_TexelSize.x*_OutlineSize;
                    float coordY = _MainTex_TexelSize.y*_OutlineSize;

                    // arriba, abajo, derecha, izquierda
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(0, coordY)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(0, -coordY)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(coordX, 0)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(-coordX, 0)).a;
                    // diagonales
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(coordX, coordY)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(coordX, -coordY)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(-coordX, coordY)).a;
                    alpha -= tex2D(_MainTex, IN.texcoord + fixed2(-coordX, -coordY)).a;


                    //c += _OutlineColor*alpha;
                    c = min(fixed4(1, 1, 1, 1), c + _OutlineColor * alpha);
                }
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
