Shader "Custom/Simple Texture Lit"
{
    Properties { _MainTex ("Texture", 2D) = "white" {} }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            sampler2D _MainTex; float4 _MainTex_ST;
            struct appdata { float4 vertex:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float3 normal:TEXCOORD1; };
            v2f vert(appdata v) { v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=TRANSFORM_TEX(v.uv,_MainTex); o.normal=UnityObjectToWorldNormal(v.normal); return o; }
            fixed4 frag(v2f i):SV_Target { fixed3 c=tex2D(_MainTex,i.uv).rgb; float n=saturate(dot(normalize(i.normal),normalize(_WorldSpaceLightPos0.xyz))); return fixed4(c*(UNITY_LIGHTMODEL_AMBIENT.rgb+_LightColor0.rgb*n),1); }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
