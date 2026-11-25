Shader "Brotato/Gold VFX Only"
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
            float Warm(float2 uv)
            {
                float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
                return smoothstep(.16, .32, color.r - color.b);
            }
            half4 frag(Output input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                float2 step = _MainTex_TexelSize.xy * 2;
                float mask = max(Warm(input.uv), max(Warm(input.uv + float2(step.x, 0)), Warm(input.uv - float2(step.x, 0))));
                mask = max(mask, max(Warm(input.uv + float2(0, step.y)), Warm(input.uv - float2(0, step.y))));
                color.a *= mask;
                return color;
            }
            ENDHLSL
        }
    }
}
