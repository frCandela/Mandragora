Shader "Custom/VertexColor" {
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert vertex:vert
#pragma target 3.0

		struct Input {
		float4 vertColor;
	};

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.vertColor = v.color;
	}

	void surf(Input IN, inout SurfaceOutput o) 
	{
		o.Albedo = float4(0,0,0, 1);
		o.Albedo.r = IN.vertColor.r;
		o.Albedo.g = IN.vertColor.g;
		o.Albedo.b = IN.vertColor.b;

	}
	ENDCG
	}
		FallBack "Diffuse"
}