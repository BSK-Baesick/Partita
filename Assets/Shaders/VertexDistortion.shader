Shader "Custom/Vertex Distortion"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color"     , Color) = (1,1,1,1)
        _AddAlphaColor("Add Alpha Color", Color) = (0,0,0,1)

        _NoiseTilingOffset("NoiseTex Tiling(x,y)/Offset(z,w)", Vector) = (0.1,0.1,0,0)
        _NoiseSizeScroll("NoiseTex Size(x,y)/Scroll(z,w)"  , Vector) = (16,16,0,0)
        _DistortionPower("Distortion Power", Float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "SasamiNoise.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 uv : TEXCOORD0;
                    float4 color : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _BaseColor;
                fixed4 _AddAlphaColor;

                fixed4 _NoiseTilingOffset;
                fixed4 _NoiseSizeScroll;
                fixed _DistortionPower;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                    o.uv.zw = TRANSFORM_NOISE_TEX(v.uv, _NoiseTilingOffset, _NoiseSizeScroll);
                    o.color = _BaseColor * v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed3 dist = normalNoise(i.uv.zw, _NoiseSizeScroll.xy);
                    dist = dist * 2 - 1;
                    dist *= _DistortionPower;

                    i.uv.xy += dist.xy;

                    fixed4 color = tex2D(_MainTex, i.uv.xy);
                    color *= i.color;
                    color.rgb += _AddAlphaColor * color.a;
                    return color;
                }
                ENDCG
            }
        }
}