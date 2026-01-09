Shader "Custom/URP2D/YellowSilhouette"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _SilhouetteColor("Silhouette Color", Color) = (1,1,0,1) // 노랑 기본
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _SilhouetteColor;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * i.color;

                // RGB 값이 있으면 노란색으로 변경
                half mask = step(0.001, max(col.r, max(col.g, col.b)));
                col.rgb = lerp(col.rgb, _SilhouetteColor.rgb, mask);

                return col;
            }
            ENDHLSL
        }
    }
}
