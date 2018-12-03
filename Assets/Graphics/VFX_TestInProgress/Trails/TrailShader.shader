Shader "Mandragora/TrailShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_UVanchor ("UV Anchor(XY), Min(Z), Max(W)", Vector) = (0.5, 0.5, 0, 0.5)
		_UVPow ("UV power", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _UVanchor;
			float4 _MainTex_ST, _Color;
			float _UVPow;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 newUv = i.uv - _UVanchor.xy;
				float UvValue = 1 - (length(newUv) - _UVanchor.z) / (_UVanchor.w - _UVanchor.z);
				UvValue = pow(UvValue, _UVPow);


				// Apply
				fixed4 col = fixed4(1,1,1,1);
				col.rgb *= _Color.rgb * UvValue;
				col.a *= UvValue;

				// DEBUG
				//col = fixed4(UvValue, UvValue, UvValue, UvValue);

				return col;
			}
			ENDCG
		}
	}
}
