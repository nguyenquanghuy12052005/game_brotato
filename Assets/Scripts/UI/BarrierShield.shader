Shader "Brotato/Barrier Shield"
{
    Properties { [PerRendererData] _MainTex("Sprite", 2D) = "white" {} _DemoTime("Demo Time", Float) = 0 }
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
            float _DemoTime;
            Output vert(Input input)
            {
                Output output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }
            half4 frag(Output input) : SV_Target
            {
                float2 p = (input.uv - .5) / float2(.39, .46);
                float radius = length(p);
                float rim = exp(-abs(radius - .92) * 80);
                float glow = exp(-abs(radius - .92) * 18);
                float inside = 1 - smoothstep(.86, .94, radius);
                float arc = pow(saturate(sin(atan2(p.y, p.x) * 3 - _DemoTime * 3)), 14);
                float pulse = .8 + .2 * sin(_DemoTime * 5);
                float alpha = (rim * .85 + glow * .22 + inside * .07 + arc * glow * .25) * pulse;
                float3 color = lerp(float3(.22, .72, .85), float3(1, .84, .39), saturate(rim + arc * glow));
                return half4(color, saturate(alpha) * input.color.a);
            }
            ENDHLSL
        }
    }
}
