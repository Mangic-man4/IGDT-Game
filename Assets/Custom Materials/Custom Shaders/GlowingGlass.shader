Shader"Custom/GlowingGlass"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1, 1, 1, 0.4)
        _GlowColor ("Glow Color", Color) = (0.2, 1, 0.5, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeWidth ("Edge Width", Range(0.001, 0.3)) = 0.02
        _GlowStrength ("Glow Strength", Range(0, 5)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _GlowColor;
            float _EdgeWidth;
            float _GlowStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenUV = o.vertex.xy / o.vertex.w;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 texCol = tex2D(_MainTex, uv);
                fixed4 col = _Color * texCol;

                            // Distance from edge: darker center, glow at edge
                float2 centeredUV = abs(uv - 0.5);
                float edgeFactor = smoothstep(0.5, 0.5 - _EdgeWidth, max(centeredUV.x, centeredUV.y));

                fixed4 glow = _GlowColor * edgeFactor * _GlowStrength;
                col.rgb += glow.rgb;

                return col;
            }
                        ENDCG
                    }
                }
            FallBack"Transparent/Diffuse"
         }
