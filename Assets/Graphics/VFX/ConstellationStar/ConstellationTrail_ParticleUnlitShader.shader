// The Noise Position Offset is factor of RED VERTEX COLOR !


Shader "Mandragora/ConstellationTrail_ParticleUnlitShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Luminosity ("Luminosity", float) = 1
		_NoiseSettings ("Noise Amplitude(X) Frequency(Y) colorFactor(Z)", Vector) = (1,1,1,0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend One One
		Cull Off

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 worldVertex : TEXCOORD0;
				float4 color : COLOR;
			};

			float4 ApplyNoise(float4 pos, float amp, float freq, float colorFactor, float colorValue) {
				float3 newPos;
				newPos.x = cos(pos.y*freq)*amp + sin(pos.z*freq)*amp;
				newPos.y = cos(pos.z*freq)*amp + sin(pos.x*freq)*amp;
				newPos.z = cos(pos.x*freq)*amp + sin(pos.y*freq)*amp;
				newPos *= (1 - colorValue) * colorFactor;
				return float4(newPos + pos.xyz, pos.w);
			}

			float4 _Color;
			float3 _NoiseSettings;
			float _Luminosity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.color = v.color;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldVertex = ApplyNoise(wPos, _NoiseSettings.x, _NoiseSettings.y, _NoiseSettings.z, o.color.r);
				//o.worldVertex = wPos;
				o.vertex = mul(UNITY_MATRIX_VP, o.worldVertex);
				//o.vertex = UnityObjectToClipPos(v.vertex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Apply
				fixed4 col = fixed4(1,1,1,1);
				col.rgb *= i.color.rgb * _Color.rgb;
				col.rgb = saturate(col.rgb);
				col.rgb *= _Luminosity;

				return col;
			}
			ENDCG
		}
	}
}
