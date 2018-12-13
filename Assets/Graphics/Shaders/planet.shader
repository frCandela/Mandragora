Shader "Custom/planet" {
	Properties {
		_CoreColor ("CoreColor", Color) = (1,1,1,1)
		_SurfaceColor("SurfaceColor", Color) = (1,1,1,1)		 
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MinDiameter("MinDiameter", Range(0,1)) = 0.0
		_MaxDiameter("MaxDiameter", Range(0,1)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 localPos;
			float2 uv_MainTex;
			float3 worldPos; 
			fixed4 color : COLOR;
		};

		fixed4 _CoreColor;
		fixed4 _SurfaceColor;
		half _MinDiameter;
		half _MaxDiameter;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex;	// local position
		}

		fixed4 mix(fixed4 col1, fixed4 col2, float blendValue)
		{
			return col2 * blendValue + col1 * (1 - blendValue);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color/ _MaxDiameter
			float radius = length(IN.localPos);
			o.Albedo = mix(_CoreColor, _SurfaceColor, (radius - _MinDiameter) / (_MaxDiameter - _MinDiameter));
			o.Albedo = IN.color;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
