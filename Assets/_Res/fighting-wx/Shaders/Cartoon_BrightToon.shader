Shader "Cartoon/Bright Toon"
{
    Properties
    {
        _MainTex ("主贴图", 2D) = "white" {}
        _Color ("颜色", Color) = (1, 1, 1, 1)
        _ShadowBrightness ("阴影亮度", Range(0, 1)) = 0.55
        _LightBrightness ("受光亮度", Range(0, 2)) = 1.2
        _LightTint ("受光染色", Color) = (1, 0.96, 0.82, 1)
        _LightTintStrength ("染色强度", Range(0, 1)) = 0.35
        _AmbientStrength ("环境光", Range(0, 1)) = 0.18
        _ShadowThreshold ("明暗分界", Range(0, 1)) = 0.45
        _ShadowSoftness ("分界柔和度", Range(0.01, 0.5)) = 0.08
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _ShadowBrightness;
            float _LightBrightness;
            fixed4 _LightTint;
            float _LightTintStrength;
            float _AmbientStrength;
            float _ShadowThreshold;
            float _ShadowSoftness;
            float4 _SplitSceneLightDir;
            fixed4 _SplitSceneLightColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;
                float3 n = normalize(i.worldNormal);
                float3 l = normalize(_SplitSceneLightDir.xyz);
                float ndotl = saturate(dot(n, l));
                float toon = smoothstep(_ShadowThreshold - _ShadowSoftness, _ShadowThreshold + _ShadowSoftness, ndotl);
                fixed3 tint = lerp(fixed3(1, 1, 1), _LightTint.rgb, _LightTintStrength);
                fixed3 shadeColor = albedo.rgb * _ShadowBrightness;
                fixed3 lightColor = albedo.rgb * _LightBrightness * tint * _SplitSceneLightColor.rgb;
                fixed3 ambient = albedo.rgb * _AmbientStrength;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed3 lit = lerp(shadeColor, lightColor, toon) * atten;
                return fixed4(ambient + lit, albedo.a);
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
