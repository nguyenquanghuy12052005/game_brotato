Shader "Hexcore/Sprite Without Low Alpha Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GlowCutoff ("Remove background glow below alpha", Range(0,1)) = 0.25
        _HealPalette ("Healing palette", Range(0,1)) = 0
        _PoisonInk ("Isolate poison sparks", Range(0,1)) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment NoGlowFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            fixed _GlowCutoff;
            fixed _HealPalette;
            fixed _PoisonInk;
            fixed4 NoGlowFrag(v2f input) : SV_Target
            {
                fixed4 color = SampleSpriteTexture(input.texcoord);
                // Test the source alpha before renderer tint/fade so ending the skill stays smooth.
                clip(color.a - _GlowCutoff);
                if (_PoisonInk > 0.5)
                {
                    // Keep vivid lime from poisoned.png and discard the sample's muted body pixels.
                    clip(min(color.g - 0.44, min(color.g - color.r - 0.04, color.g - color.b - 0.22)));
                }
                if (_HealPalette > 0)
                {
                    fixed light = max(color.r, max(color.g, color.b));
                    fixed3 leaf = fixed3(0.62, 0.88, 0.33) * light;
                    fixed3 ivory = fixed3(0.94, 1.0, 0.74) * light;
                    color.rgb = lerp(color.rgb, lerp(leaf, ivory, smoothstep(0.52, 1.0, light)), _HealPalette);
                }
                color *= input.color;
                color.rgb *= color.a;
                return color;
            }
            ENDCG
        }
    }
}
