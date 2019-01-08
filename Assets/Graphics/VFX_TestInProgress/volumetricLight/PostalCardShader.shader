Shader "Mandragora/PostalCardShader"
{
	Properties
	{
		_Color ("Color", Color) = (0,0,0,0)
		_Opacity ("General Opacity", Range(0,1)) = 0.5
		_HighlightOpacity ("Highlight Opacity", Range(0,1)) = 0.5
		_DistFade ("Distance Fade", float) = 0
		_MaxInterDist ("Maximum Instersection Distance", float) = 0
		_FresnelSensibility ("Fresnel Sensibility", Range(0,1)) = 0.25
		_IsBackFace ("Is Back Face ?", Range(0,1)) = 1
		_HighlightColor ("Highlight Color", Color) = (0,0,0,0)
		_IsAlphaTex ("Is Alpha Tex ?", Range(0,1)) = 1
		_AlphaMaskTex ("Alpha Mask", 2D) = "white" {}
		_IsAlphaUV ("Is Alpha UV ?", Range(0,1)) = 1
		_AlphaMaskPow ("Alpha Power (powX, powY, minX, minY)", Vector) = (2,2,0,0)
		_Cursor ("Cursor", float) = 0
		_PositiveNegativeDepth ("Positive(1) or negative(0) depth", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
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
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
			};

			fixed4 _Color, _HighlightColor;
			float _Opacity, _HighlightOpacity, _DistFade;
			float _MaxInterDist;
			float _Cursor, _FresnelSensibility, _IsAlphaTex, _IsAlphaUV, _IsBackFace;
			float4 _AlphaMaskPow;
			sampler2D _CameraDepthTexture, _AlphaMaskTex;
			float _PositiveNegativeDepth;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul (UNITY_MATRIX_M, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{
				facing = sign(facing) * 0.5 + 0.5;
				facing = lerp(1, facing, _IsBackFace);
				i.worldNormal = (facing * i.worldNormal) + ((1 - facing) * -i.worldNormal);

				float3 toCam = _WorldSpaceCameraPos - i.worldPos; // Vector from vertex to camera
				float dist = length(toCam);						  // distance vertex/camera

				// Distance Fade
				float distFade = (dist - _ProjectionParams.y) / _DistFade;
				distFade = clamp(distFade, 0, 1);

				// Fresnel Effect
				float fresnel = dot( i.worldNormal, normalize(toCam));
				fresnel = (fresnel - _FresnelSensibility) / (1 - _FresnelSensibility);
				fresnel = saturate(fresnel);

				// Depth Buffer
				float2 screenUV = i.screenPos.xy/i.screenPos.w;
				float depthBuffer = Linear01Depth( tex2D(_CameraDepthTexture, screenUV).r);

				// Vertex Depth
				// with near/far distances
				float vertexDepth = (i.screenPos.w - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);


				// Intersection
				float intersectionValue = (depthBuffer - vertexDepth); // get base intersection
				float positiveHighLight = saturate(intersectionValue / _MaxInterDist);
				float negativeHighLight = 1 - saturate(intersectionValue / _MaxInterDist);
				float highLight = lerp(negativeHighLight, positiveHighLight, _PositiveNegativeDepth);


				// Alpha Mask
				float alphaMask = 1;

				float alphaMaskTexValue = tex2D(_AlphaMaskTex, i.uv).r; // Alpha Texture Applied

				float alphaMaskX = abs((i.uv.x - 0.5) * 2); // Alpha with uv.x
					alphaMaskX = cos(alphaMaskX)*0.5 + 0.5;
					alphaMaskX =pow(alphaMaskX, _AlphaMaskPow.x);
					alphaMaskX = (alphaMaskX - _AlphaMaskPow.z) / (1 - _AlphaMaskPow.z);
					alphaMaskX = clamp(alphaMaskX, 0, 1);

				float alphaMaskY = abs((i.uv.y - 0.5) * 2); // Alpha with uv.y
					alphaMaskY = pow(alphaMaskY, _AlphaMaskPow.y);
					alphaMaskY = 1 - alphaMaskY;
					alphaMaskY = (alphaMaskY - _AlphaMaskPow.w) / (1 - _AlphaMaskPow.w);
					alphaMaskY = clamp(alphaMaskY, 0, 1);

				alphaMask *= lerp(1, alphaMaskX * alphaMaskY, saturate(_IsAlphaUV)); // Apply alpha with UVs
				alphaMask *= lerp(1, alphaMaskTexValue, saturate(_IsAlphaTex)); // Apply alpha with Texture


				
				// Apply
				fixed4 col = _Color;

				// Color
				col.rgb = _HighlightColor.rgb; // with classic highlight

				// Alpha
				col.a = fresnel * distFade;
				col.a *= _HighlightOpacity * highLight;
				col.a *= alphaMask;

				return col;
			}
			ENDCG
		}
	}
}
