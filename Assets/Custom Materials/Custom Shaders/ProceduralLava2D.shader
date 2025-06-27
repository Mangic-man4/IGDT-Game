Shader"Custom/ProceduralLava2D"
{
    Properties
    {
        _ColorCool   ("Rock Color",   Color) = (0.08,0.02,0.01,1)
        _ColorHot    ("Lava Color",   Color) = (1,0.4,0,1)
        _CrackSharp  ("Crack Sharpness", Range(0.1,5)) = 3
        _ScrollSpeed ("Flow Speed", Float) = 0.15
        _GlowPulse   ("Glow Pulse", Range(0,1)) = 0.3
        _GlowMult    ("Glow Multiplier", Range(0,10)) = 3 
        _Alpha       ("Alpha", Range(0,1)) = 1
        _TileFactor  ("Tiling Factor", Float) = 1
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

            fixed4 _ColorCool, _ColorHot;
            float _CrackSharp, _ScrollSpeed, _GlowPulse, _Alpha, _TileFactor, _GlowMult;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uvWorld : TEXCOORD0;
            };

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

            float fbm(float2 p)
            {
                float v = 0;
                float a = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    v += noise(p) * a;
                    p *= 2;
                    a *= 0.5;
                }
                return v;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                            // Use world position to build UV
                float2 worldPos = mul(unity_ObjectToWorld, v.vertex).xy;

                            // Scale it down by tiling factor to control how big the pattern is
                o.uvWorld = worldPos * _TileFactor;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uvWorld;
                uv.y += _Time.y * _ScrollSpeed;

                float height = fbm(uv * 3);
                float cracks = pow(saturate(height), _CrackSharp);

                float pulse = 1 + sin(_Time.y * 4) * _GlowPulse;
    
                //fixed3 col = lerp(_ColorCool.rgb, _ColorHot.rgb * pulse, cracks); // Without emission
                fixed3 baseCol = lerp(_ColorCool.rgb, _ColorHot.rgb * pulse, cracks);

                // Add a bloom-eligible emissive component
                fixed3 glowCol = _ColorHot.rgb * cracks * _GlowMult; // Use a strong multiplier for bloom

                // Final color = base + glow (the bloom will catch the overbright parts)
                fixed3 finalColor = baseCol + glowCol;
    
    
                //return fixed4(col, _Alpha); // Without emission
                return fixed4(finalColor, _Alpha);

            }
                        ENDCG
                    }
                }
FallBack Off
}
