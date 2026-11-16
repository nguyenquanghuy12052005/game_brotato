Shader "Brotato/Clean VFX"
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
            float Accent(float2 uv)
            {
                float4 sample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float maximum = max(sample.r, max(sample.g, sample.b));
                float minimum = min(sample.r, min(sample.g, sample.b));
                return smoothstep(.10, .32, (maximum - minimum) / max(maximum, .001)) * sample.a;
            }
            half4 frag(Output input) : SV_Target
            {
                float4 source = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float2 offset = _MainTex_TexelSize.xy * 2;
                float mask = Accent(input.uv);
                mask = max(mask, max(Accent(input.uv + float2(offset.x, 0)), Accent(input.uv - float2(offset.x, 0))));
                mask = max(mask, max(Accent(input.uv + float2(0, offset.y)), Accent(input.uv - float2(0, offset.y))));
                // Giữ thép tối/đá đặc; ánh trắng chỉ giữ gần nét màu của hiệu ứng.
                float brightness = max(source.r, max(source.g, source.b));
                float darkSolid = saturate((.48 - brightness) * 6) * smoothstep(.94, .995, source.a);
                mask = max(mask, darkSolid);
                return half4(source.rgb * input.color.rgb, source.a * input.color.a * mask);
            }
            ENDHLSL
        }
    }
}
