Shader "Custom/Color"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _FlashAlpha("Flash Alpha", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _FlashAlpha;
            CBUFFER_END
            
            float3 RGBtoHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = (c.g < c.b) ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
                float4 q = (c.r < p.x) ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

                float d = q.x - min(q.w, q.y);
                float e = 1e-10;

                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)),d / (q.x + e),q.x);
            }
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }
            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }
            float3 SRGBToLinear(float3 c)
            {
                return pow(c, 2.2);
            }
            float3 LinearToSRGB(float3 c)
            {
                return pow(c, 1.0 / 2.2);
            }
            half4 frag(Varyings IN) : SV_Target
{
    half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

    // 透明部分はそのまま返す
    if (tex.a == 0) return tex;

    // ★ sRGB → Linear
    float3 rgb = SRGBToLinear(tex.rgb);
    float3 baseRgb = SRGBToLinear(_BaseColor.rgb);

    // ★ Linear で HSV 変換
    float3 hsv = RGBtoHSV(rgb);
    float3 target = RGBtoHSV(baseRgb);

    // 色相・彩度を上書き
    hsv.x = target.x;
    hsv.y = target.y;

    // ★ Linear 空間で RGB に戻す
    float3 result = HSVtoRGB(hsv);

    // ★ Linear → sRGB
    result = LinearToSRGB(result);

    return float4(result, tex.a*_FlashAlpha);
}
            ENDHLSL
        }
    }
}
