// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mandragora/FX/BlackHole"
{
	Properties
	{
		_Color ("Texture", Color) = (0,0,0,0)
		_Distortion ("Distortion", Range(-10, 0)) = 0
		_DistortionIntensity ("Intensity", Range(0, .01)) = 0
        _HoleSize ("HoleSize", Range(0, 1)) = 0
        _Scale ("Scale", Range(0, 10)) = 1
	}
	SubShader
	{
		GrabPass { "_GrabTexture" }
 
        Pass {
            Tags { "Queue"="Transparent" }
       
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
                half4 pos : SV_POSITION;
                half4 grabPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
            };
 
            sampler2D _GrabTexture;
            float _Distortion, _DistortionIntensity, _HoleSize, _Scale;
 
            v2f vert(appdata v)
			{
                v2f o;

                // float4 pos = v.vertex;
                // pos *= _Scale;
                // pos = mul(UNITY_MATRIX_P,
                //         float4(UnityObjectToViewPos(float3(0, 0, 0)), 1)
                //         + float4(pos.x, pos.z, 0, 0));
                // o.pos = pos;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }
 
            half4 frag(v2f i) : COLOR
			{
                float dist = distance(float2(.5,.5), i.uv);
                float2 warp = normalize(float2(.5,.5) - i.uv) * pow(dist, _Distortion) * _DistortionIntensity;
                warp.y = -warp.y;

                i.grabPos.x += warp.x;
                i.grabPos.y += warp.y;

                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grabPos));
                color *= saturate(4/_HoleSize * dist - 1);

                return color;
            }
            ENDCG
        }
	}
}
