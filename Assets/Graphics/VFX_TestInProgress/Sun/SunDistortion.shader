Shader "Mandragora/SunDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", float) = 1
	}
	SubShader
	{
		Tags { "RenderQueue"="Transparent" "DisableBatching" = "True" }

		ZTest Always

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

				fixed4 backCol = tex2Dproj(_BackgroundTex, i.grabPos);

				fixed4 col = float4(1,1,1,1);
				return backCol;
			}
			ENDCG
		}
	}
}
