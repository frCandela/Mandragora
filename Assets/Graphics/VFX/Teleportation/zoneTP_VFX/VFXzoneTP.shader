Shader "Mandragora/VFXzoneTP"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TilingScale ("Tiling (XY), Scale (ZW)", Vector) = (1,1,1,1)
		_Speed ("Speed", float) = 1
		_Frequency ("Frequency", float) = 1
		_Color ("Color", Color) = (1,1,1,1)
		_Luminosity ("Luminosity", float) = 1
	}
	SubShader
	{

		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		//Cull Off

		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;

			};

			sampler2D _MainTex;
			float4 _Color, _TilingScale;
			float _Frequency, _Speed, _Luminosity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			

			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{

				// Inverse Normal for VFACE
				/*float3 invertedNormal = -flatNormals;
				facing = step(1, facing);
				flatNormals = lerp(invertedNormal, flatNormals, facing);*/
				i.uv = clamp(i.uv, 0.01, 0.99);
				float2 UVs = (i.uv * _TilingScale.zw) + _TilingScale.xy;
				//UVs = clamp(UVs, 0.01, 0.99);
				float3 tex = tex2D(_MainTex, UVs).rgb;
				float fractions = (1 - tex.g) * 1;
				fractions = abs(frac(fractions * _Frequency + _Time.y * _Speed) - 0.5) * 2;

				float factor = fractions * (1 - i.uv.y);

				float3 newCol = _Color.rgb * _Luminosity * factor;
				

				fixed4 col;
				col.rgb = newCol;
				//col.rgb = 1 - tex.b;
				col.a = sqrt(factor) * (1 - pow(1 - i.uv.y, 4));
				
				return col;
			}
			ENDCG
		}
	}
}