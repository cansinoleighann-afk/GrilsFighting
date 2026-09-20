Shader "Cartoon/Bright Toon Scene"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _ShadowBrightness ("Shadow Brightness", Range(0, 1)) = 0.65
        _LightBrightness ("Light Brightness", Range(0, 2)) = 1.15
        _LightTint ("Light Tint", Color) = (1, 0.96, 0.82, 1)
        _LightTintStrength ("Light Tint Strength", Range(0, 1)) = 0.35
        _AmbientStrength ("Ambient Strength", Range(0, 2)) = 1
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.45
        _ShadowSoftness ("Shadow Softness", Range(0.01, 0.5)) = 0.08
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
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color;
            float _ShadowBrightness; float _LightBrightness; fixed4 _LightTint; float _LightTintStrength;
            float _AmbientStrength; float _ShadowThreshold; float _ShadowSoftness;
            float4 _BrightToonSceneLightDirection; fixed4 _BrightToonSceneLightColor;
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float2 uv:TEXCOORD0; float3 normal:TEXCOORD1; float4 vertex:SV_POSITION; };
            v2f vert(appdata v) { v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=TRANSFORM_TEX(v.uv,_MainTex); o.normal=UnityObjectToWorldNormal(v.normal); return o; }
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 albedo=tex2D(_MainTex,i.uv)*_Color;
                float n=saturate(dot(normalize(i.normal),normalize(_BrightToonSceneLightDirection.xyz)));
                float toon=smoothstep(_ShadowThreshold-_ShadowSoftness,_ShadowThreshold+_ShadowSoftness,n);
                fixed3 ambient=max(ShadeSH9(float4(normalize(i.normal),1))*_AmbientStrength,0);
                fixed3 shadow=albedo.rgb*_ShadowBrightness;
                fixed3 tint=lerp(fixed3(1,1,1),_LightTint.rgb,_LightTintStrength);
                fixed3 light=albedo.rgb*_LightBrightness*tint*_BrightToonSceneLightColor.rgb;
                return fixed4(albedo.rgb*ambient+lerp(shadow,light,toon),albedo.a);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
