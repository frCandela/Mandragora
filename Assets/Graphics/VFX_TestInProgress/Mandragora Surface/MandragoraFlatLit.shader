Shader "Custom/MandragoraFlatLit" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Luminosity ("Luminosity", float) = 0
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0)
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
		float _Luminosity;

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

		half4 LightingMandragoraSurfaceFlatLit (MandragoraSurfaceFlatLitOutput s, half3 lightDir, half3 viewDir, float atten) {

			// Light Process
            float NdotL = dot (s.Normal, lightDir);
			float lighting = saturate(atten * NdotL);

			// Shadow Color
			//float3 shadowCol = (1 - lighting) * _ShadowColor.rgb;
			
			// Apply
			float4 c;
    		c.rgb = s.Albedo * _LightColor0.rgb * lighting;

			// baseShadow
			/*#if LIGHTPROBE_SH
			c.rgb += shadowCol;
			#else
			#endif*/

			// DEBUG
			//c.rgb = half3(shadowCol.rgb);
			/*
			#if LIGHTPROBE_SH  // first pass
			#else
			c.rgb = half3(shadowCol.rgb);
			#endif
			*/
			
			
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

			fixed3 newColor;
			fixed baseValue = max(max(color.r, color.g), color.b);
			newColor = _ShadowColor * (1 - baseValue);

			newColor.r = max(color.r, newColor.r);
			newColor.g = max(color.g, newColor.g);
			newColor.b = max(color.b, newColor.b);

			color.rgb = newColor;
		}

		void surf (Input IN, inout MandragoraSurfaceFlatLitOutput o) {

			// build tangent rotation matrix here (saves one interpolator):
			float3 binormal = cross( IN.normal, IN.tangent.xyz) * IN.tangent.w; 
			float3x3 rotation = float3x3( IN.tangent.xyz, binormal, IN.normal );
			
			// get world space normal from position derivatives, and transform it to tangent space:
			half3 worldNormal = -normalize(cross(ddx(IN.worldPos), ddy(IN.worldPos)));
			o.Normal = mul(rotation,worldNormal);

			// Apply
			fixed4 c = _Color;
			o.Albedo = c.rgb + (c.rgb * _Luminosity);
			o.Emission = _EmissionColor.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
