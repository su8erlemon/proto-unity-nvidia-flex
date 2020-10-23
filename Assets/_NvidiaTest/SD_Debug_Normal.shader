﻿Shader "Custom/SD_Debug_Normal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Params1 ("_Params1", Vector) = (0.0,0.0,0.0,0.0)

    }
    SubShader
    {
        
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 200

        Cull Front

        CGPROGRAM

        #include "Assets/_NvidiaTest/S8/SDL_NoiseSet.cginc"
        
        
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 normal;
            // float3 position;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4 _Params1;
        
        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.normal = abs(v.normal.xyz);
            // o.vertex = v.vertex.xyz;
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // o.Albedo = c.rgb;
            o.Albedo = IN.normal + c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;

            o.Albedo -= abs(curlNoise(IN.uv_MainTex.xyx*_Params1.x+_Params1.y+_Time.x*_Params1.w)*_Params1.z);
            // o.Alpha = 1;
            // clip(o.Albedo.r-0.5);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
