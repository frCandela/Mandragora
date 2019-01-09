Shader "custom/mandragoraStandard" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ShadowColor ("Shadow Color", Color) = (0,0,0,1)
		_RampTex ("Ramp Texture", 2D) = "white" {}
		_ShadowRampTex ("Shadow Ramp Texture", 2D) = "white" {}
		_Slider ("slider", float) = 0
		_EmissiveColor ("Emissive Color", Color) = (0,0,0,0)
		_EmissionIntensity ("Emission Intensity", float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf MandragoraToonStandard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _RampTex, _ShadowRampTex;
		fixed4 _ShadowColor;
		float _Slider;
		float lightingInfo;

		struct MandragoraSurfaceOutput
		{
			fixed3 Albedo;  // diffuse color
			fixed3 Normal;  // tangent space normal, if written
			fixed3 Emission;
			//half Specular;  // specular power in 0..1 range
			//fixed Gloss;    // specular intensity
			fixed Alpha;    // alpha for transparencies
		};

		half4 LightingMandragoraToonStandard (MandragoraSurfaceOutput s, half3 lightDir, half atten) {

			// Light Process
            half NdotL = dot (s.Normal, lightDir);
			NdotL = (NdotL * 0.5) + 0.5;
			half toon = tex2D(_RampTex, half2(NdotL, 0)).r;

			// Shadow Process
			atten = pow(atten, _Slider);
			half shadowToon = tex2D(_ShadowRampTex, half2(atten, 0)).r;

			// Lighting atten
			float lighting =  min(shadowToon, toon); // toonLighing
			//float lighting =  min(atten, NdotL); // baseLighting

			lightingInfo = lighting;
			fixed4 shadowCol = (1 - lighting) * _ShadowColor; // With choosen Color
			//fixed4 shadowCol = (1 - lighting) * _ShadowColor; // With light Color
			
			// Apply
			half4 c;
    		c.rgb = s.Albedo * _LightColor0.rgb * lighting;

			#if LIGHTPROBE_SH
			c.rgb += shadowCol;
			#else
			
			#endif

			// DEBUG
			//c.rgb = half3(shadowCol.rgb);
			/*
			#if LIGHTPROBE_SH  // first pass
			#else
			c.rgb = half3(shadowCol.rgb);
			#endif
			*/
			
			
			

            c.a = s.Alpha;
            return c;
        }

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		float4 _EmissiveColor;
		float _EmissionIntensity;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout MandragoraSurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//o.Albedo = c.rgb;

			//Emissive
			float3 emissive = _EmissiveColor * _EmissionIntensity;
			o.Albedo = emissive + c.rgb;

			o.Emission = emissive;

			// Metallic and smoothness come from slider variables
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
