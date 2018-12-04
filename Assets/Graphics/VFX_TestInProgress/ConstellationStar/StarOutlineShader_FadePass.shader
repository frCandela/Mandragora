Shader "Custom/StarOutlineShader_FadePass"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_OutlineExtrusion("[S] Spikes Extrusion", float) = 0
		_OutlineColor("[S] Spikes Color", Color) = (0, 0, 0, 1)
		_Speed ("[S] Speed", float) = 1
		_PowDist ("[S] Length Visibility", float) = 2
		_PowFraction ("[S] Opacity smooth", float) = 2
		_FadeColor("[F] Color", Color) = (1, 1, 1, 1)
		_FadeSize ("[F] Fade Size", float) = 1
		_FadeSpeed ("[F] Fade Speed", float) = 1
		_FadePow ("[F] Fade Power", float) = 1
		_FadeRoundFactor ("[F] Fade Round Factor", float) = 1
	}

	CGINCLUDE
		
		#include "UnityCG.cginc"
		//#pragma vertex vert
		//#pragma fragment frag

		struct vertexInput
			{
				float4 vertex : POSITION;
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
			NAME "Base Pass"
            Tags { "RenderType"="Opaque" }

			// Write to Stencil buffer (so that outline pass can read)
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// Properties
			float4 _Color;

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				float4 newVertex = input.vertex;
				newVertex *= 1 + cos(_Time.z + newVertex.x + newVertex.y + newVertex.z)*0.2;
				output.pos = UnityObjectToClipPos(newVertex);
				output.color = _Color;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return input.color;
			}

			ENDCG
		}

		Pass
		{
			NAME "Fade Pass"

			Tags {"Queue"="Transparent" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			// Won't draw where it sees ref value 4
			ZWrite Off
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
			float4 _FadeColor;
			float _FadeSize, _FadeSpeed, _FadePow;
			float _FadeRoundFactor;


			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4 localPos = input.vertex;
				float4 normLocalPos = localPos * _FadeRoundFactor; // Make round version of object

				float timeFrac = frac(_Time.y * _FadeSpeed); // fraction time speed
				float invertedTimeFrac = 1 - abs(0.5 - timeFrac) * 2; // make it inverted to fade-in/fade-out opacity
				invertedTimeFrac = pow(invertedTimeFrac, _FadePow); // give it more intensity with Pow

				localPos = lerp(localPos, normLocalPos, timeFrac); // lerp between round and normal object
				localPos *= _FadeSize * (1 + timeFrac); // finaly apply the size with frac time

				float4 newColor = float4(_FadeColor.rgb, _FadeColor.a * invertedTimeFrac);
				
				output.pos = UnityObjectToClipPos(localPos);
				output.color = newColor;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return input.color;
			}

			ENDCG
		}

		// Outline pass
		Pass
		{
			NAME "Spikes Pass"

			Tags {"Queue"="Transparent" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			// Won't draw where it sees ref value 4
			Cull Off
			ZWrite Off
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
			float4 _OutlineColor;
			float _OutlineExtrusion, _MaxLocalDist, _MinLocalDist, _PowDist, _PowFraction, _Speed;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float distFromCenter : TEXCOORD0;
				float fraction : TEXCOORD1;
				
			};

			v2f vert(appdata input)
			{
				v2f output;

				float4 newPos = input.vertex;
				float4 localPos = newPos;
				float time = _Time.y * _Speed; // set time speed


				_MinLocalDist = 1.4; // Hard coded for a specific mesh
				_MaxLocalDist = 5.68; // ""
				float dist = length(localPos.xyz); // take distance vertex/pivot
				dist = (dist - _MinLocalDist) * (_MaxLocalDist - _MinLocalDist); // make it between 0 - 1
				output.distFromCenter = saturate(dist);


				// normal extrusion technique
				float4 normal = normalize(float4(input.normal, 1));
				float noise = frac(time + localPos.x + localPos.y + localPos.z); // desynchronize vertices
				float fractionNoise = 1 - (abs(0.5 - noise) * 2); // make frac between 0-1 and inverted

				output.fraction = pow(fractionNoise, _PowFraction); // give it more intensity
				newPos += float4(normal.xyz, 0.0) * _OutlineExtrusion * dist * noise; // apply everything


				// convert to world space
				output.pos = UnityObjectToClipPos(newPos);

				return output;
			}

			float4 frag(v2f input) : COLOR
			{
				float dist = input.distFromCenter;
				float frac = input.fraction;
				dist = pow(dist, _PowDist);
				dist *= frac;
				return float4(_OutlineColor.rgb * dist, _OutlineColor.a * dist * frac);
			}

			ENDCG
		}

		
		

		
	}
}