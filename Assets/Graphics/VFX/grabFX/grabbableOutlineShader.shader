// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/grabbableOutlineShader"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Luminosity ("Luminosity", float) = 1
		_OutlineExtrusion ("Outline Extrusion", float) = 0.07
		_MaxFadeDist ("Maximum Fade Distance", float) = 1
		_Speed ("Outline Speed", float) = 1
		_NoiseIntensity ("Outline Noise Intensity", float) = 1008
		_NoisePow ("Noise Power", float) = 0.5
		_NoiseFreq ("Noise Frequency", float) = 0.03
	}

	CGINCLUDE
		
		#include "UnityCG.cginc"

		struct vertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float fadeDist : TEXCOORD0;
			};

		ENDCG

	SubShader
	{

		Pass
		{
			NAME "Stencil Write Pass"
            Tags { "RenderQueue"="Transparent" "IgnoreProjector"="True" }

			Blend Zero One

			// Write to Stencil buffer (so that outline pass can read)
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex myVertex
			#pragma fragment myFrag
			#include "UnityCG.cginc"

			struct vertIn {
				float4 vertex : POSITION;
				float4 normal : POSITION;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
			};

			// properties
			float _OutlineExtrusion, _Speed;

			vertOut myVertex(vertIn input)
			{
				vertOut output;
				output.vertex = UnityObjectToClipPos(input.vertex);
				return output;
			}

			float4 myFrag(vertOut input) : COLOR
			{
				return float4(0,0,0,0);
			}

			ENDCG
		}

		Pass
		{
			NAME "Outline Pass"

			Tags {"Queue"="Transparent" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			// Won't draw where it sees ref value 4
			//ZWrite Off
			ZTest On
			Stencil
			{
				Ref 4
				Comp notequal
				Fail keep
				Pass replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Properties
			float4 _Color;
			float _OutlineExtrusion, _Speed, _NoiseIntensity;
			float _Luminosity, _NoiseFreq, _NoisePow, _MaxFadeDist;


			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// store different positions
				float4 worldPivot = mul(unity_ObjectToWorld, float4(0,0,0,1));
				float4 worldPos = mul(unity_ObjectToWorld, input.vertex);
				float4 worldLocalPos = worldPos - worldPivot;
				float3 worldNormal = UnityObjectToWorldNormal(input.normal).xyz;

				// Compute noise
				float moyennePos = (worldLocalPos.x + worldLocalPos.y + worldLocalPos.z);
				float fraction = frac(_Time.y * _Speed + cos(moyennePos * _NoiseFreq) * _NoiseIntensity);
				fraction = abs(fraction - 0.5) * 2;
				fraction = pow(fraction, _NoisePow);
				// apply noise
				float3 offset = (worldNormal * _OutlineExtrusion * fraction);
				float offsetDist = length(offset);
				worldLocalPos.xyz += offset;

				// Set final position
				float4 localPos = mul(unity_WorldToObject, worldLocalPos + worldPivot);
				output.pos = UnityObjectToClipPos(localPos);

				output.color = _Color * _Luminosity;
				output.fadeDist = 1 - saturate(offsetDist / _OutlineExtrusion * _MaxFadeDist);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				fixed4 col = input.color;
				col.a = input.fadeDist;
				return col;
			}

			ENDCG
		}

		
		

		
	}
}