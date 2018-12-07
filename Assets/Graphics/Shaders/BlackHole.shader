// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mandragora/FX/BlackHole"
{
	Properties
	{
		_Color ("Texture", Color) = (0,0,0,0)
		_Distortion ("Distortion", Range(-2.5, 0)) = 0
		_DistortionIntensity ("Intensity", Range(0, 1000)) = 0
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
            float _Distortion, _DistortionIntensity;
 
            v2f vert(appdata v)
			{
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }
 
            half4 frag(v2f i) : COLOR
			{
                float2 uv = i.uv;
                float2 center = float2(.5,.5) * _ScreenParams.xy;
                float2 uvpixel = uv * _ScreenParams.xy;
                
                float dist = distance(center, uvpixel);

                float2 warp = normalize(float2(.5,.5) - uv) * pow(dist, _Distortion) * _DistortionIntensity;
                warp.y = -warp.y;

                i.grabPos.x += warp.x;
                i.grabPos.y += warp.y;

                fixed4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.grabPos));
                color *= saturate(50/_DistortionIntensity * dist - 1.5);

                // color = float4(dist,0,0,0);

                return color;
            }
            ENDCG
        }
	}
}
