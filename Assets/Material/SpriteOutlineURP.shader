Shader "Custom/SpriteOutline2D"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 투명 픽셀은 제외
                if (col.a == 0)
                {
                    // 주변 픽셀 검사
                    float2 offset = _MainTex_TexelSize.xy * _OutlineWidth;

                    float alpha =
                        tex2D(_MainTex, i.uv + float2( offset.x, 0)).a +
                        tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a +
                        tex2D(_MainTex, i.uv + float2(0,  offset.y)).a +
                        tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                    if (alpha > 0)
                        return _OutlineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
