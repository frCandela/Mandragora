Shader "Mandragora/SunDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", float) = 1
		_Distortion ("Distortion", Range(-10, 0)) = 0
		_DistortionIntensity ("Intensity", Range(0, .01)) = 0
        _HoleSize ("HoleSize", Range(0, 1)) = 0
		_MaxDist ("Max Distortion Size", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }

		//ZTest Always

		GrabPass {
			"_BackgroundTex"
		}

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
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _MainTex, _BackgroundTex;
			float _Scale;
			float _HoleSize, _DistortionIntensity, _Distortion, _MaxDist;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 pos = v.vertex;
				pos *= _Scale;
                pos = mul(UNITY_MATRIX_P, 
                      float4(UnityObjectToViewPos(float3(0, 0, 0)), 1)
                          + float4(pos.x, pos.z, 0, 0));
                o.vertex = pos;
				o.grabPos = ComputeGrabScreenPos(o.vertex);

				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 fromCenter = (float2(.5,.5) - i.uv)*2;
				float dist = length(fromCenter);
				dist = saturate(dist / _MaxDist);
                float2 warp = normalize(fromCenter) * pow(dist, _Distortion) * _DistortionIntensity;
                warp.y = -warp.y;

                i.grabPos.x += warp.x;
                i.grabPos.y += warp.y;

                fixed4 col = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.grabPos));
                col *= saturate(4/_HoleSize * dist - 1);

				return col;
			}
			ENDCG
		}
	}
}
