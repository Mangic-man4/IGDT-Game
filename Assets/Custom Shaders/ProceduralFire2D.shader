Shader"Custom/ProceduralFire2D"
{
    Properties
    {
        _Speed          ("Scroll Speed", Range(0,1)) = 0.2 // Slower default
        _DistortAmt     ("Distortion Amount", Range(0,1)) = 0.2

        _GradientTop    ("Top Color",  Color) = (1,1,0.5,1)
        _GradientMid    ("Mid Color",  Color) = (1,0.4,0,1)
        _GradientBot    ("Bottom Color", Color) = (0.1,0,0,1)

        _FlickerColor   ("Flicker Color", Color) = (1,0.6,0.2,1)
        _FlickerStrength("Flicker Strength", Range(0,1)) = 0.3

        _GlowMult       ("Glow Multiplier", Range(0,10)) = 3   // <— new slider
        _Alpha          ("Global Alpha", Range(0,1)) = 1
        _MainTex        ("Main Texture", 2D) = "white" {}   // Required for SpriteRenderer
        _MaskTex        ("Shape Mask (white = visible)", 2D) = "white" {} // optional mask
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            float _Speed, _DistortAmt, _Alpha, _FlickerStrength, _GlowMult;
            fixed4 _GradientTop, _GradientMid, _GradientBot, _FlickerColor;

            sampler2D _MainTex;
            sampler2D _MaskTex;

            //----------------------------------------
            // Utility noise (hash + value noise)
            //----------------------------------------
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3 - 2 * f);

                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

                        //----------------------------------------
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //----------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
    
                            /* -------- UV animation -------- */
                float2 uv = i.uv;
                uv.y += frac(_Time.y * _Speed); // Ensures wrapping, avoids float precision drift
                float n = noise(uv * 2 + _Time.y);
                uv.x += (n - 0.5) * _DistortAmt;

                            /* -------- base gradient -------- */
                float t = saturate(uv.y);
                fixed3 baseCol = lerp(_GradientBot.rgb, _GradientMid.rgb, t);
                baseCol = lerp(baseCol, _GradientTop.rgb, smoothstep(0.6, 0.9, t));

                            /* -------- coloured flicker -------- */
                float flick = noise(uv * 8 + _Time.y * 10);
                baseCol += _FlickerColor.rgb * flick * _FlickerStrength;

                            /* -------- emissive glow -------- */
                            // Hotter near bottom (lower t) and with flicker
                float glowFactor = (1 - t) * flick;
                fixed3 glowCol = _FlickerColor.rgb * glowFactor * _GlowMult;

                fixed3 finalCol = baseCol + glowCol;

                            /* -------- mask / alpha -------- */
                float2 maskUV = i.uv;
                maskUV.x += (noise(maskUV * 4 + _Time.y) - 0.5) * _DistortAmt * 0.5; // Only slight horizontal distortion, no scroll
                float mask = tex2D(_MaskTex, maskUV).r;

                return fixed4(finalCol, _Alpha * mask);
            }
            ENDCG
        }
    }
    FallBack Off
}
