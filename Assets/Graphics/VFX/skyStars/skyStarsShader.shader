Shader "Unlit/VolumetricDustShader"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Visibility ("Visibility", float) = 1
		_NoiseFreq ("Glitter Frequency", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 wVertex : TEXCOORD1;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float _Visibility, _NoiseFreq;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float noise = cos((_Time.y + i.wVertex.x + i.wVertex.y) * _NoiseFreq) * 0.5 + 0.5;

				float4 tex = tex2D(_MainTex, i.uv);
				fixed4 col = tex * i.color * _Visibility * noise;
				return col;
			}
			ENDCG
		}
	}
}
