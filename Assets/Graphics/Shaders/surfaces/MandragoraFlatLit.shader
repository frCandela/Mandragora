Shader "Mandragora/MandragoraFlatLit" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Luminosity ("Luminosity", float) = 0
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0)
		_SpecPow ("Specular Power", float) = 48.0
		_SpecularIntensity ("Specular Intensity", float) = 3
		_DepthDistance ("Depth Distance", float) = 40
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf MandragoraSurfaceFlatLit vertex:vert finalcolor:shadowColor fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color, _EmissionColor, _ShadowColor;
		float _Luminosity, _SpecPow, _SpecularIntensity;
		float _DepthDistance;

		struct MandragoraSurfaceFlatLitOutput
		{
			fixed3 Albedo;  // diffuse color
			fixed3 Normal;  // tangent space normal, if written
			fixed3 Emission;
			fixed Alpha;    // alpha for transparencies
		};

		struct Input {
			float3 worldPos;
			float4 tangent;
			float3 normal;
		};
		
		void vert (inout appdata_full v, out Input o) {
           	UNITY_INITIALIZE_OUTPUT(Input,o);

			o.normal = normalize(UnityObjectToWorldNormal(v.normal));
  			o.tangent.xyz = normalize(UnityObjectToWorldDir(v.tangent.xyz));
  			o.tangent.w = v.tangent.w * unity_WorldTransformParams.w;

     	}


		void surf (Input IN, inout MandragoraSurfaceFlatLitOutput o) {

			// build tangent rotation matrix here (saves one interpolator):
			float3 binormal = cross( IN.normal, IN.tangent.xyz) * IN.tangent.w; 
			float3x3 rotation = float3x3( IN.tangent.xyz, binormal, IN.normal );
			
			// get world space normal from position derivatives, and transform it to tangent space:
			half3 flatNormal = - normalize(cross(ddx(IN.worldPos), ddy(IN.worldPos))).xyz;
			o.Normal = mul(rotation, flatNormal);


			// Apply
			fixed4 c = _Color;
			o.Albedo = c.rgb + (c.rgb * _Luminosity);
			o.Emission = _EmissionColor.rgb;
			o.Alpha = c.a;


			// shadow Color
			o.Albedo.rgb = float3(max(o.Albedo.r, _ShadowColor.r), max(o.Albedo.g, _ShadowColor.g), max(o.Albedo.b, _ShadowColor.b));
			//depth

		}

		half4 LightingMandragoraSurfaceFlatLit (MandragoraSurfaceFlatLitOutput s, half3 lightDir, half3 viewDir, float atten) {

			// Light Process
            float NdotL = dot (s.Normal, lightDir);
			float lighting = saturate(atten * NdotL);

			// Specular
			float3 h = normalize(viewDir + lightDir);
			float nh = max(0, dot(s.Normal, h));
			float spec = pow(nh, _SpecPow);

			// Fresnel to atten specular
			float fresnel = max(0, dot(viewDir, s.Normal));
			float specAtten = pow(1 - fresnel, 2);
			spec *= specAtten;
			spec *= _SpecularIntensity;
			float3 specColor = (s.Albedo + _LightColor0.rgb)/2;

			
			// Apply
			float4 c;
    		c.rgb = (s.Albedo * _LightColor0.rgb * lighting) + ((spec * _LightColor0 + spec * s.Albedo) * atten);	
            c.a = 1.0;
            return c;
        }



		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		void shadowColor (Input IN, MandragoraSurfaceFlatLitOutput o, inout fixed4 color) {

			#if LIGHTPROBE_SH // Apply only on first pass
			
			fixed3 newColor;

			newColor = float3(max(color.r, _ShadowColor.r), max(color.g, _ShadowColor.g), max(color.b, _ShadowColor.b));

			color.rgb = newColor;

			#endif

			// Depth
			float distFromCam = length(_WorldSpaceCameraPos.xyz - IN.worldPos);
			float depth = 1 - saturate(distFromCam / _DepthDistance);

			float moyAlbedo = (color.r + color.g + color.b)/3;
			float3 depthColor = float3(moyAlbedo, moyAlbedo, moyAlbedo);
			color.rgb = lerp(depthColor, color.rgb, depth);

		}

		
		ENDCG
	}
	FallBack "Diffuse"
}
