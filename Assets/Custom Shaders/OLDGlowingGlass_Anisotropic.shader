Shader"Custom/OLDGlowingGlass_Anisotropic_Unlit"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1, 1, 1, 0.4)
        _GlowColor ("Glow Color", Color) = (0.2, 1, 0.5, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeWidthX ("Edge Width X", Range(0.001, 0.3)) = 0.02
        _EdgeWidthY ("Edge Width Y", Range(0.001, 0.3)) = 0.02
        _GlowStrength ("Glow Strength", Range(0, 5)) = 2
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _GlowColor;
            float _EdgeWidthX;
            float _EdgeWidthY;
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
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 texCol = tex2D(_MainTex, uv);
                fixed4 col = _Color * texCol;

                float2 centeredUV = abs(uv - 0.5);
                float edgeX = smoothstep(0.5, 0.5 - _EdgeWidthX, centeredUV.x);
                float edgeY = smoothstep(0.5, 0.5 - _EdgeWidthY, centeredUV.y);
                float edgeFactor = edgeX * edgeY;

                fixed4 glow = _GlowColor * edgeFactor * _GlowStrength;
                col.rgb += glow.rgb;
                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
