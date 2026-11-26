Shader "Brotato/HEXCORE Gold VFX"
{
    Properties { [PerRendererData] _MainTex("Sprite", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Input { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct Output { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            Output vert(Input input)
            {
                Output output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            float Gold(float4 pixel)
            {
                return smoothstep(.12, .31, pixel.r - pixel.b) * smoothstep(.09, .30, pixel.g - pixel.b);
            }

            float LitGold(float2 uv)
            {
                float4 pixel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return Gold(pixel) * smoothstep(.18, .55, pixel.a);
            }

            half4 frag(Output input) : SV_Target
            {
                float4 source = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float2 texel = _MainTex_TexelSize.xy;
                float adjacent = max(LitGold(input.uv + float2(texel.x, 0)), LitGold(input.uv - float2(texel.x, 0)));
                adjacent = max(adjacent, max(LitGold(input.uv + float2(0, texel.y)), LitGold(input.uv - float2(0, texel.y))));
                // Lõi vàng giữ glow. Ánh trắng chỉ đi cùng nét vàng sáng kế bên.
                float glint = smoothstep(.28, .67, min(source.r, min(source.g, source.b))) * adjacent;
                float mask = max(Gold(source), glint);
                return half4(source.rgb * input.color.rgb, source.a * input.color.a * mask);
            }
            ENDHLSL
        }
    }
}
