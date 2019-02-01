Shader "Mandragora/UnlitSquareZoneTP"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RampTex ("RampTexture", 2D) = "white" {}
		_PlanarCoordsR ("R Planar Tiling(X) Panner(YZ) Speed(W)", Vector) = (1,0,0,1)
		_PlanarCoordsG ("G Planar Tiling(X) Panner(YZ) Speed(W)", Vector) = (1,0,0,1)
		_PlanarCoordsB ("B lanar Tiling(X) Panner(YZ) Speed(W)", Vector) = (1,0,0,1)
		_Color ("Main Color", Color) = (0,0,0,1)
		_Luminosity ("Luminosity", float) = 1
		_Range ("Color Range", Range(0,0.5)) = 0.1
		_BrightOffset ("Brightness Offset", float) = 0
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 planarUVsRG : TEXCOORD0;
				float2 planarUVsB : TEXCOORD1;
			};

			sampler2D _MainTex, _RampTex;
			float4 _PlanarCoordsR, _PlanarCoordsG, _PlanarCoordsB;
			float4 _Color;
			float _Luminosity;
			float _Range, _BrightOffset;
			//float _ManagerUnlitFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				// Set planar UVs
				float2 UVs = mul(unity_ObjectToWorld, v.vertex).xz;
				float4 planUvRG;
				float2 planUvB;
				float cosTimeR = cos(_Time.x + 1) * 0.5 + 0.5;
				float cosTimeG = cos(_Time.x + 2) * 0.5 + 0.5;
				float cosTimeB = cos(_Time.x) * 0.5 + 0.5;
				planUvRG.xy = (UVs * _PlanarCoordsR.x) + (_PlanarCoordsR.yz * _Time.y * _PlanarCoordsR.w * cosTimeR);
				planUvRG.zw = (UVs * _PlanarCoordsG.x) + (_PlanarCoordsG.yz * _Time.y * _PlanarCoordsG.w * cosTimeG);
				planUvB = (UVs * _PlanarCoordsB.x) + (_PlanarCoordsB.yz * _Time.y * _PlanarCoordsB.w * cosTimeB);
				o.planarUVsRG = planUvRG;
				o.planarUVsB = planUvB;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float sinTime = sin(_Time.x) * 0.5 + 0.5;

				float texR = tex2D(_MainTex, i.planarUVsRG.xy).r;
				float texG = tex2D(_MainTex, i.planarUVsRG.zw).g;
				float texB = tex2D(_MainTex, i.planarUVsB).b;

				float texValue = texR * texG * texB;

				float rangedValue = 1 - saturate(abs(texValue * sinTime - 0.5) / _Range);
				rangedValue = clamp(sqrt(rangedValue), 0.001, 0.999);
				rangedValue += _BrightOffset;
				float3 texColor = tex2D(_RampTex, float2(rangedValue, 0)).rgb;

				fixed4 col = float4(0,0,0,1);
				col.rgb = saturate(texColor * _Color.rgb);
				col.rgb *= _Luminosity;
				return col;
			}
			ENDCG
		}
	}
}
