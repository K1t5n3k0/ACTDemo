Shader "Unlit/Genshin_Brow"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _ShiftX ("ShiftX", float) = 0
        _ShiftY ("ShiftY", float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-10" }
        LOD 100

        ZTest GEqual

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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

            CBUFFER_START(UnityPerMaterial)
            float3 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ShiftX;
            float _ShiftY;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                float4 viewvertex = mul(UNITY_MATRIX_MV,v.vertex);
                float4x4 PMatrix = UNITY_MATRIX_P;
                PMatrix[0][2] = _ShiftX;
                PMatrix[1][2] = _ShiftY;
                o.vertex = mul(PMatrix,viewvertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 maintex = tex2D(_MainTex, i.uv) * _Color;
                return float4(maintex,1);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
