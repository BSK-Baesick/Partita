Shader "Unlit/Gradient Blur"
{
    Properties
    {
        [HDR]
        _InnerColor("Inner Color", Color) = (1, 1, 1, 1)
        [HDR]
        _OuterColor ("Outer Color", Color) = (1, 1, 1, 1)
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
        _Mask ("Mask", 2D) = "white" {}
        _BlurRadius ("Blur Radius", Range(0.0, 0.1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend OneMinusSrcAlpha SrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _InnerColor;
            float4 _OuterColor;
            float _Alpha;
            sampler2D _Mask;
            float4 _Mask_ST;
            float _BlurRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Mask);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 texcol = lerp(_InnerColor, _OuterColor, distance(float2(0.5, 0.5), i.uv));
                float remaining = 1.0f;
                float coef = 1.0;
                float fI = 0;
                for (int j = 0; j < 3; j++) {
                    fI++;
                    coef *= 0.12;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x, i.uv.y - fI * _BlurRadius)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x - fI * _BlurRadius, i.uv.y)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x + fI * _BlurRadius, i.uv.y)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x, i.uv.y + fI * _BlurRadius)) * coef * _Alpha;

                    texcol.a -= tex2D(_Mask, float2(i.uv.x - fI * _BlurRadius, i.uv.y - fI * _BlurRadius)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x - fI * _BlurRadius, i.uv.y + fI * _BlurRadius)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x + fI * _BlurRadius, i.uv.y - fI * _BlurRadius)) * coef * _Alpha;
                    texcol.a -= tex2D(_Mask, float2(i.uv.x + fI * _BlurRadius, i.uv.y + fI * _BlurRadius)) * coef * _Alpha;

                    remaining -= 8 * coef;
                }
                texcol.a -= tex2D(_Mask, float2(i.uv.x, i.uv.y)) * remaining * _Alpha;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, texcol);
                return texcol;
            }
            ENDCG
        }
    }
}
