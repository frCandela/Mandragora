// The Color depends on Vertex Color
// 	R channel = Ground Level (0 = Min, 1 = Max)
// 	G channel = UnderWater value (0 = out of Water, 1 = deepest in Water)
// 	B channel = Snow Value (0 = not in snow, 1 = in nSnow)

Shader "Mandragora/PlanetsMandragoraFlatLit" {
	Properties {
		_Luminosity ("Luminosity", float) = 0
		_LightDiffuse ("Light Diffuse (POW)", float) = 1
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0,0,0,0)
		_SpecPow ("Specular Power", float) = 48.0
		_SpecularIntensity ("Specular Intensity", float) = 3
		_DepthDistance ("Depth Distance", float) = 40

		_RampTexture ("Ground Layers Ramp Texture", 2D) = "white" {}
		_SnowColor ("Snow Color(RGB), Step(A)", Color) = (1,1,1,0.4)
		_UwaterColor ("Under Water Color (RGB), Step(A)", Color) = (0,0,0,0.79)
		_GrassColor ("Grass Color(RGB), Step(A)", Color) = (0,0,0,0.4)
		_PlanetColorShadowAmount ("Amount of PlanetColor in Shadow", Range(0,1)) = 0.2

		_SinNoiseSettings ("Snow Noise Amp1(X) Freq1(Y) Amp2(Z) Freq2(W)", Vector) = (-0.07, 35, -0.1, 14.09)
		_FlatNoiseSettings ("Grass Noise Amp1(X) Freq1(Y) Amp2(Z) Freq2(W)", Vector) = (0.35, 2.38, 0.1, 11.25)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf MandragoraSurfaceFlatLit vertex:vert finalcolor:shadowColor fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _RampTexture;
		float4 _SnowColor, _UwaterColor, _GrassColor;
		fixed4 _EmissionColor, _ShadowColor;
		float _Luminosity, _SpecPow, _SpecularIntensity;
		float _DepthDistance, _LightDiffuse;
		float _PlanetColorShadowAmount;
		float4 _SinNoiseSettings, _FlatNoiseSettings;

		struct MandragoraSurfaceFlatLitOutput
		{
			fixed3 Albedo;  // diffuse color
			fixed3 Normal;  // tangent space normal, if written
			fixed3 Emission;
			fixed Alpha;    // alpha for transparencies
		};

		struct Input {
			float3 localPos;
			float3 worldPos;
			float4 tangent;
			float3 normal;
			float4 color : COLOR;
		};
		
		void vert (inout appdata_full v, out Input o) {
           	UNITY_INITIALIZE_OUTPUT(Input,o);

			o.normal = normalize(UnityObjectToWorldNormal(v.normal));
  			o.tangent.xyz = normalize(UnityObjectToWorldDir(v.tangent.xyz));
  			o.tangent.w = v.tangent.w * unity_WorldTransformParams.w;
			o.localPos = v.vertex.xyz;

     	}

		
		float SinNoise (float3 pos, float amplitude1, float frequency1, float amplitude2, float frequency2) {
			float noise1;
			noise1 = cos(pos.x*frequency1) + sin(pos.y*frequency1);
			noise1 *= amplitude1;

			float noise2;
			noise2 = sin(pos.x*frequency2) + cos(pos.y*frequency2);
			noise2 *= amplitude2;

			float noise = noise1 + noise2;
			return noise;
		}

		float FlatNoise (float3 pos, float amp1, float freq1, float amp2, float freq2) {
			float noise1;
			float noiseValue1 = frac((pos.x + pos.y + pos.z)*freq1);
			float stepNoise1 = 1 - step(1, noiseValue1*2);
			noise1 = (stepNoise1 * noiseValue1) + ((1 - stepNoise1) * (1 - noiseValue1));
			noise1 *= amp1;

			float noise2;
			float noiseValue2 = frac((pos.x + pos.y + pos.z)*freq2);
			float stepNoise2 = 1 - step(1, noiseValue2*2);
			noise2 = (stepNoise2 * noiseValue2) + ((1 - stepNoise2) * (1 - noiseValue2));
			noise2 *= amp2;

			return noise1 + noise2;

		}


		void surf (Input IN, inout MandragoraSurfaceFlatLitOutput o) {

			// build tangent rotation matrix here (saves one interpolator):
			float3 binormal = cross( IN.normal, IN.tangent.xyz) * IN.tangent.w; 
			float3x3 rotation = float3x3( IN.tangent.xyz, binormal, IN.normal );
			
			// get world space normal from position derivatives, and transform it to tangent space:
			half3 flatNormal = - normalize(cross(ddx(IN.worldPos), ddy(IN.worldPos))).xyz;
			o.Normal = mul(rotation, flatNormal);


			// Colors with Ground Levels
			float3 groundWaterSnowA = IN.color.rgb;
			float3 finalColor = tex2D(_RampTexture, groundWaterSnowA.r).rgb; // Set Ground Color
			float waterLerpFactor = saturate((saturate(1 - groundWaterSnowA.r) - _UwaterColor.a) / (1 - _UwaterColor.a));
			finalColor = lerp(finalColor, _UwaterColor.rgb, waterLerpFactor); // Set UnderWater Amount
			float offsetedGrassStep = _GrassColor.a + FlatNoise(IN.localPos, _FlatNoiseSettings.x, _FlatNoiseSettings.y, _FlatNoiseSettings.z, _FlatNoiseSettings.w);
			offsetedGrassStep = saturate(offsetedGrassStep);
			finalColor = lerp(finalColor, _GrassColor.rgb, step(offsetedGrassStep, groundWaterSnowA.g));
			float offsetedSnowStep = _SnowColor.a + SinNoise(IN.localPos, _SinNoiseSettings.x, _SinNoiseSettings.y, _SinNoiseSettings.z, _SinNoiseSettings.w);
			offsetedSnowStep = saturate(offsetedSnowStep);
			finalColor = lerp(finalColor, _SnowColor, step(offsetedSnowStep, groundWaterSnowA.b)); // Set Snow Amount


			// Apply
			fixed4 c = float4(finalColor, 1);
			o.Albedo = c.rgb + (c.rgb * _Luminosity);
			o.Emission = float3(0,0,0);
			o.Alpha = c.a;


			// shadow Color
			o.Albedo.rgb = float3(max(o.Albedo.r, _ShadowColor.r), max(o.Albedo.g, _ShadowColor.g), max(o.Albedo.b, _ShadowColor.b));

		}

		half4 LightingMandragoraSurfaceFlatLit (MandragoraSurfaceFlatLitOutput s, half3 lightDir, half3 viewDir, float atten) {

			// Light Process
            float NdotL = dot (s.Normal, lightDir);
			float powAtten = pow(atten, 1.5);
			float lighting = saturate(powAtten * NdotL);

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
    		c.rgb = (s.Albedo.rgb * _LightColor0.rgb * lighting) + ((spec * _LightColor0) * atten);	
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

			// Planets Color
			color.rgb = lerp(color.rgb, o.Albedo, _PlanetColorShadowAmount);

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
