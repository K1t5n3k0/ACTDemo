Shader "Unlit/Genshin_Face"
{
    Properties
    {
        [HDR]_BrightColor ("BrightColor", Color) = (1,1,1,1)
        [HDR]_DarkColor("DarkColor", Color) = (0,0,0,0)
        _MainTex ("MainTex", 2D) = "white" {}
        _LightMap ("LightMap", 2D) = "white" {}
        [HDR]_OutLineColor ("OutLineColor", Color) = (0,0,0,0)
        _OutLineStrength ("OutLineStrength", float) = 0.001
        _ShiftX ("ShiftX", float) = 0
        _ShiftY ("ShiftY", float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-20" }
        LOD 100

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
            
            float3 _BrightColor;
            float3 _DarkColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _LightMap;
            float _ShiftX;
            float _ShiftY;

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
                float3 maintex = tex2D(_MainTex, i.uv);
                float3 lightmaptex01 = tex2D(_LightMap, float2(1 - i.uv.x, i.uv.y));
                float3 lightmaptex02 = tex2D(_LightMap, i.uv);
                float2 left = normalize(TransformObjectToWorldDir(float3(1, 0, 0)).xz);
                float2 front = normalize(TransformObjectToWorldDir(float3(0, 0, 1)).xz);
                Light light = GetMainLight();
                float3 worldlightdir = normalize(light.direction);
                float ctrl = 1 - max(dot(front, worldlightdir.xz) * 0.5 + 0.5,0);
                //float ilm = dot(worldlightdir.xz, left) > 0 ? lightmaptex01.r : lightmaptex02.r;
                float ilm;
                if(dot(worldlightdir.xz, left) > 0)
                {
                    ilm = lightmaptex01.r;
                }
                else
                {
                    ilm = lightmaptex02.r;
                }
                float isSahdow = smoothstep(ilm,ilm + 0.01, ctrl);
                float3 finalcolor = maintex * lerp(_BrightColor * light.color,_DarkColor,isSahdow);
                return float4(finalcolor,1);
            }
            ENDHLSL
        }

        Pass
        {
            Cull Front
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _OutLineColor;
            float _OutLineStrength;
            float _ShiftX;
            float _ShiftY;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += v.normal.xyz * _OutLineStrength;
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
                float3 maintex = tex2D(_MainTex, i.uv);
                return float4(maintex * _OutLineColor,1);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
