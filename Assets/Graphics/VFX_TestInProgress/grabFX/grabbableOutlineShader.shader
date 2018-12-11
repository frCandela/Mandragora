Shader "Custom/grabbableOutlineShader"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Luminosity ("Luminosity", float) = 1
		_OutlineExtrusion("Outline Extrusion", float) = 0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_Speed ("Outline Speed", float) = 1
		_NoiseIntensity ("Outline Noise Intensity", float) = 1
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
			float _Luminosity;


			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4 localPos = input.vertex;
				float moyennePos = (localPos.x + localPos.y + localPos.z);
				float fraction = frac(_Time.y * _Speed + cos(moyennePos) * _NoiseIntensity);
				fraction = abs(fraction - 0.5) * 2;
				float3 offset = (input.normal * _OutlineExtrusion * fraction);
				localPos.xyz += offset;
				
				output.pos = UnityObjectToClipPos(localPos);
				output.color = _Color * _Luminosity;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return input.color;
			}

			ENDCG
		}

		
		

		
	}
}