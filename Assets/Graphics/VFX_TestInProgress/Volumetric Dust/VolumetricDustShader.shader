Shader "Unlit/VolumetricDustShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_FresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite Off
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
				float3 wVertex : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float3 wVertex : TEXCOORD0;
				fixed4 color : COLOR;
			};

			float4 _Color;
			float _FresnelIntensity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.wVertex = mul (unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
			{

				//facing = sign(facing) * 0.5 + 0.5;
				//i.normal = (facing * i.normal) + ((1 - facing) * -i.normal);

				float3 invertedNormal = - i.normal;
				facing = step(1, facing);
				i.normal = lerp(invertedNormal, i.normal, facing);

				float3 toCam = _WorldSpaceCameraPos - i.wVertex;
				toCam = normalize(toCam);

				float fresnel = dot(toCam, i.normal);
				fresnel = (fresnel - _FresnelIntensity) / (1 - _FresnelIntensity);
				fresnel = saturate(fresnel);

				fixed4 col = _Color;
				col = fixed4(i.color.rgb * fresnel, i.color.a * fresnel);
				return col;
			}
			ENDCG
		}
	}
}
