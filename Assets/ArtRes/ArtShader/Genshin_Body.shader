Shader "Unlit/Genshin_Body"
{
    Properties
    {
        [HDR]_MetalBrightColor ("MetalBrightColor", Color) = (1,1,1,1)
        [HDR]_MetalDarkColor ("MetalDarkColor", Color) = (0,0,0,0)
        [HDR]_HandBrightColor ("HandBrightColor", Color) = (1,1,1,1)
        [HDR]_HandDarkColor ("HandDarkColor", Color) = (0,0,0,0)
        [HDR]_SoftBrightColor ("SoftBrightColor", Color) = (1,1,1,1)
        [HDR]_SoftDarkColor ("SoftDarkColor", Color) = (0,0,0,0)
        [HDR]_SilkBrightColor ("SilkBrightColor", Color) = (1,1,1,1)
        [HDR]_SilkDarkColor ("SilkDarkColor", Color) = (0,0,0,0)
        [HDR]_SkinBrightColor ("SkinBrightColor", Color) = (1,1,1,1)
        [HDR]_SkinDarkColor ("SkinDarkColor", Color) = (0,0,0,0)

        _MainTex ("MainTex", 2D) = "white" {}
        _DarkExcessive ("DarkExcessive", Range(0,1)) = 0.5
        _DarkStrength ("DarkStrength", Range(0,1)) = 0.5
        [HDR]_SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _SpecularExcessive ("SpecularExcessive", Range(0,1)) = 0.5
        _SpecularStrength ("SpecularStrength", Range(0,1)) = 0.5
        _CubeMap ("CubeMap", CUBE) = "" {}
        _CubeMapStrength ("CubeMapStrength", Range(0,10)) = 1
        _LightMap ("LightMap", 2D) = "white" {}
        [HDR]_FresnelColor ("FresnelColor", Color) = (1,1,1,1)
        _Fresnel ("Fresnel", Range(0,10)) = 1
        _FresnelExcessive ("FresnelExcessive", Range(0,1)) = 0.5
        _FresnelStrength ("FresnelStrength", Range(0,1)) = 0.5
        [HDR]_OutLineColor ("OutLineColor", Color) = (0,0,0,0)
        _OutLineStrength ("OutLineStrength", float) = 0.001
        _ShiftX ("ShiftX", float) = 0
        _ShiftY ("ShiftY", float) = 0
        [Enum]_Cull ("Cull", Int) = 2
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-20" }
        LOD 100
        Cull [_Cull]

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldvertex : COLOR0;
                float3 worldnormaldir : COLOR1;
            };

            float3 _MetalBrightColor;
            float3 _MetalDarkColor;
            float3 _HandBrightColor;
            float3 _HandDarkColor;
            float3 _SoftBrightColor;
            float3 _SoftDarkColor;
            float3 _SilkBrightColor;
            float3 _SilkDarkColor;
            float3 _SkinBrightColor;
            float3 _SkinDarkColor;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DarkExcessive;
            float _DarkStrength;
            float3 _SpecularColor;
            float _SpecularExcessive;
            float _SpecularStrength;
            samplerCUBE _CubeMap;
            float _CubeMapStrength;
            sampler2D _LightMap;
            float3 _FresnelColor;
            float _Fresnel;
            float _FresnelExcessive;
            float _FresnelStrength;
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
                o.worldvertex = TransformObjectToWorld(v.vertex);
                o.worldnormaldir = normalize(TransformObjectToWorldDir(v.normal));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 maintex = tex2D(_MainTex, i.uv);
                float4 lightmaptex = tex2D(_LightMap,i.uv);
                float3 BrightColor;
                float3 DarkColor;
                if(lightmaptex.a >= 0 && lightmaptex.a <= 0.15)
                {
                    BrightColor = _MetalBrightColor;
                    DarkColor = _MetalDarkColor;
                }
                if(lightmaptex.a >= 0.2 && lightmaptex.a <= 0.35)
                {
                    BrightColor = _HandBrightColor;
                    DarkColor = _HandDarkColor;
                }
                if(lightmaptex.a >= 0.4 && lightmaptex.a <= 0.55)
                {
                    BrightColor = _SoftBrightColor;
                    DarkColor = _SoftDarkColor;
                }
                if(lightmaptex.a >= 0.6 && lightmaptex.a <= 0.75)
                {
                    BrightColor = _SilkBrightColor;
                    DarkColor = _SilkDarkColor;
                }
                if(lightmaptex.a >= 0.8 && lightmaptex.a <= 1)
                {
                    BrightColor = _SkinBrightColor;
                    DarkColor = _SkinDarkColor;
                }
                Light light = GetMainLight();
                float3 worldlightdir = normalize(light.direction);
                float NdotL = smoothstep(_DarkStrength - _DarkExcessive,_DarkStrength + _DarkExcessive,(max(dot(i.worldnormaldir,worldlightdir),0) * 0.5 + 0.5) + lightmaptex.g);
                float3 lambert = lerp(DarkColor,BrightColor * light.color,NdotL) * maintex;
                float3 worldviewdir = normalize(_WorldSpaceCameraPos.xyz - i.worldvertex);
                float3 cubemap = texCUBE(_CubeMap,reflect(-worldviewdir,i.worldnormaldir)) * _CubeMapStrength;
                float NdotH = max(dot(i.worldnormaldir,normalize(worldlightdir + worldviewdir)),0);
                float3 specular = smoothstep(_SpecularStrength - _SpecularExcessive,_SpecularStrength + _SpecularExcessive,(NdotH + max(dot(i.worldnormaldir.xz,worldlightdir.xz),0)) * lightmaptex.b * lightmaptex.r) * _SpecularColor * cubemap * light.color;
                float NdotV = pow(1 - max(dot(i.worldnormaldir,worldviewdir),0),_Fresnel);
                float3 fresnel = smoothstep(_FresnelStrength - _FresnelExcessive,_FresnelStrength + _FresnelExcessive,NdotV) * _FresnelColor * lambert;
                float3 finalcolor = lambert + specular + fresnel;
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
