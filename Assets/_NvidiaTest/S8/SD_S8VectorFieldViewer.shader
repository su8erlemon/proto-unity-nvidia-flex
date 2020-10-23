Shader "Custom/SD_S8VectorFieldViewer"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        _SmoothTex ("SmoothTex (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5

        _MetalTex ("MetalTex (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _OcclusionTex ("OcclusionTex", 2D) = "white" {}
        _OcclusionScale ("OcclusionScale", Range(0,1)) = 1.0

        _EmissionTex ("EmissionTex", 2D) = "white" {}
        _Emissioness ("Emissioness", Float) = 1.0

        [Normal]_NormalTex ("NormalTex (RGB)", 2D) = "bump" {}
        _NormalScale ("NormalScale", Float) = 1.0
        

        [Normal]_NormalTex2 ("NormalTex2 (RGB)", 2D) = "bump" {}
        _NormalScale2 ("NormalScale2", Float) = 1.0

        _ParticleTime ("ParticleTime", Float) = 0.0
        _Params ("Params", Vector) = (0.0,0.0,0.0,0.0)
        _Params2 ("Params2", Vector) = (0.0,0.0,0.0,0.0)
        _Params3 ("Params3", Vector) = (0.0,0.0,0.0,0.0)

        _EasingTexture ("EasingTexture", 2D) = "white" {}
        _GeometoryTexture ("GeometoryTexture", 2D) = "black" {}
        _FFTTexture ("FFTTexture", 2D) = "black" {}
                
                
		_Thickness ("Thickness", 2D) = "white" {}
		_Distortion("Distortion", Range(0,1)) = 0.0
        _Power("Power", Range(0,10)) = 0.0
		_Scale("Scale", Range(0,10)) = 0.0

        _Type ("Type", Int) = 0
        _Num ("Num", Int) = 0
    }
    
    SubShader
    {
        // Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM

		#include "UnityCG.cginc"
        #include "Assets/S8/Quaternion.cginc"
        #include "Assets/S8/SDL_NoiseSet.cginc"
        
		#include "UnityPBSLighting.cginc"

        // Physically based Standard lighting model, and enable shadows on all light types
        // #pragma surface surf StandardTranslucent fullforwardshadows vertex:vert addshadow
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        // #pragma multi_compile_instancing
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 5.0

        struct Particle{
            float3 initPosition;
            float3 position;
            float3 velocity;
            float3 direction;
            float life;
            float duration;
        };

        #ifdef SHADER_API_D3D11
            StructuredBuffer<Particle> particleBuffer;
        #endif

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalTex2;
			float4 vertex;
            float index;
            UNITY_FOG_COORDS(1)
        };

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            float4 texcoord3 : TEXCOORD3;
        #if defined(SHADER_API_XBOX360)
            half4 texcoord4 : TEXCOORD4;
            half4 texcoord5 : TEXCOORD5;
        #endif
            fixed4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            uint index : SV_InstanceID;
            uint id : SV_VertexID;
        };

        sampler2D _MainTex;
        sampler2D _OcclusionTex;
        sampler2D _NormalTex;
        sampler2D _NormalTex2;
        sampler2D _MetalTex;
        sampler2D _SmoothTex;
        sampler2D _EmissionTex;
        sampler2D _EasingTexture;
        sampler2D _GeometoryTexture;
        sampler2D _FFTTexture;
		sampler2D _Thickness;

        half _Glossiness;
        half _Emissioness;
        half _Metallic;
        float _OcclusionScale;
        float _NormalScale;
        float _NormalScale2;

        fixed4 _Color;
        float _ParticleTime;
        float4 _Params;
        float4 _Params2;
        float4 _Params3;

        
		float thickness;
		float _Distortion;
		float _Power;        
		float _Scale;

        int _Type;
        int _Num;
        
        inline fixed4 LightingStandardTranslucent(SurfaceOutputStandard s, fixed3 viewDir, UnityGI gi)
		{
			// Original colour
			fixed4 pbr = LightingStandard(s, viewDir, gi);

			// --- Translucency ---
			float3 L = gi.light.dir;
			float3 V = viewDir;
			float3 N = s.Normal;

			float3 H = normalize(L + N * _Distortion);
			float VdotH = pow(saturate(dot(V, -H)), _Power) *  _Scale;
			float3 I = (VdotH + unity_AmbientSky) * thickness;

			// Final add
			pbr.rgb = pbr.rgb + gi.light.color * I;
			return pbr;
		}

		void LightingStandardTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);
		}

        float inoutStep(float minValue, float maxValue, float value){
            return ( value < minValue ) ? smoothstep(0.0,1.0,1.0 - ((minValue - value) / minValue)) : smoothstep(0.0,1.0, 1.0 - ((value-maxValue) / (1.0 - maxValue)));
        }


        float fft(float index){
            float dd = (1.0/1024.0) * 0.5;
            return tex2Dlod (_FFTTexture, float4(dd+(1.0/1024.0)*index,dd ,0.0,0.0)).r;
        }

        float easing(float id, float t){
            float dd = (1.0/1024.0) * 0.5;
            return tex2Dlod (_EasingTexture, float4(t+dd,dd + dd * id,0.0,0.0)).r;
        }

        float3 geometory(float index){
            float dd = (1.0/1024.0) * 0.5;
            float y = (1.0/1024.0) * index;
            // return (tex2Dlod (_GeometoryTexture, float4(y+dd,dd,0.0,0.0)).xyz-0.5)*2.0;
            
            return (tex2Dlod (_GeometoryTexture, float4(y+dd,dd,0.0,0.0)).xyz-0.5)*2.0;
        }

        float4 scaleTween(float4 pos, float3 from, float3 to, float duration, float delay ){
            float t = clamp(_ParticleTime-delay,0.0,duration);
            float3 s = lerp(from, to, easing(0,t/duration));
            pos.xyz *= s;
            return pos;
        }

        float4 translateTween(float4 pos, float3 from, float3 to, float duration, float delay ){
            float t = clamp(_ParticleTime-delay,0.0,duration);
            float3 s = lerp(from, to, easing(0, t/duration));
            pos.xyz += s;
            return pos;
        }

        float4 rotateTween(float4 pos, float3 from, float3 to, float duration, float delay ){
            float t = clamp(_ParticleTime-delay,0.0,duration);
            float3 s = lerp(from, to, easing(0, t/duration));
            pos.xyz = quat_rot(quat_axis_angle(float3(1.0,0.0,0.0),s.x),pos.xyz);
            pos.xyz = quat_rot(quat_axis_angle(float3(0.0,1.0,0.0),s.y),pos.xyz);
            pos.xyz = quat_rot(quat_axis_angle(float3(0.0,0.0,1.0),s.z),pos.xyz);
            return pos;
        }

        float4 modify(float4 pos, float index) {
          
            float4 p = pos;


            #ifdef SHADER_API_D3D11
                p.xyz *= _Scale;
                // p.xyz *= particleBuffer[index].initPosition.x;
                // p.xyz *= (0.3+particleBuffer[index].velocity.x*0.7) * shrinkFactor;
                // p.z *= (0.3+particleBuffer[index].velocity.z*0.7);

                float4 q = quat_look_at(particleBuffer[index].direction, float3(0.0,-1.0,0.0));
                p.xyz = quat_rot(q,p.xyz);

                // p.xyz = quat_rot(quat_axis_angle(curlNoise(particleBuffer[index].velocity*10.0), sin(_Time.y*1.0+index)*6.28),p.xyz);

                p.xyz += particleBuffer[index].initPosition;
            #endif
            
            return float4(p.xyz,pos.w);
        }
        
        void vert (inout appdata v, out Input o) {
        
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // instancing unique ID
            float index = v.index;
            // particleBuffer[index];
            float4 pos = modify(v.vertex, index);
            float4 tangent = v.tangent;
            float4 normal = float4(v.normal.xyz,1.0);
            float3 binormal = normalize(cross(normal, tangent));

            float delta = 0.001;
            float4 posT = modify(v.vertex + tangent * delta, index);
            float4 posB = modify(v.vertex + float4(binormal,1.0) * delta, index);

            float4 modifiedTangent = posT - pos;
            float4 modifiedBinormal = posB - pos;
            v.normal = normalize(cross(modifiedTangent, modifiedBinormal));            
            v.vertex = pos;
    
            o.vertex = v.vertex;
            o.index = v.index;
            // UNITY_TRANSFER_FOG(o, o.vertex);


        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // UNITY_APPLY_FOG(IN.fogCoord, c);
            o.Albedo = c.rgb;

            // o.Albedo.xy = IN.uv_MainTex.xy;
            // o.Albedo.xyz = float3(0.0,0.0,0.0);
            o.Normal =  UnpackScaleNormal ( tex2D (_NormalTex, IN.uv_MainTex), _NormalScale);
            o.Normal += UnpackScaleNormal ( tex2D (_NormalTex2, IN.uv_NormalTex2), _NormalScale2);
            o.Occlusion = tex2D (_OcclusionTex, IN.uv_MainTex).r*_OcclusionScale;
            o.Metallic = tex2D (_MetalTex, IN.uv_MainTex) * _Metallic;
            o.Smoothness = tex2D (_SmoothTex, IN.uv_MainTex) * _Glossiness;
            o.Emission = tex2D (_EmissionTex, IN.uv_MainTex) * _Emissioness;
            o.Alpha = 1.0;
            // thickness = tex2D (_Thickness, IN.uv_MainTex).r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
