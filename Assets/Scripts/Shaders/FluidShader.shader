Shader "Custom/FluidShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            // Define the maximum number of sprites
            #define MAX_SPRITES 10

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Array to store sprite positions
            float3 _SpritePositions[MAX_SPRITES];
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 screenPos = i.vertex.xy / i.vertex.w;

                for (int j = 0; j < MAX_SPRITES; j++)
                {
                    float dist = distance(screenPos, _SpritePositions[j].xy);
                    if (dist < _Radius)
                    {
                        col.a *= smoothstep(_Radius, _Radius - 0.1, dist);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}