Shader "Phong Tessellation" {
        Properties {
            _EdgeLength ("Edge length", Range(2,50)) = 5
            _Phong ("Phong Strengh", Range(0,1)) = 0.5
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _Color ("Color", color) = (1,1,1,0)
			_NoiseAmplitude ("Noise Amplitude", float) = 0
			_NoiseFrequency ("Noise Frequency", float) = 1
			_NoiseSpeed ("Noise Speed", float) = 1
        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf Lambert vertex:vert tessellate:tessEdge tessphong:_Phong nolightmap
            #include "Tessellation.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
				float4 tangent : TEXCOORD1;
				float4 worldVertex : TEXCOORD2;
            };

			float4 ApplyNoise (float4 pos, float amplitude, float frequency, float speed) {

				float3 noise = float3(sin(pos.y*frequency + _Time.y*speed) + cos(pos.z*frequency + _Time.y*speed),
										sin(pos.z*frequency + _Time.y*speed) + cos(pos.x*frequency + _Time.y*speed),
										sin(pos.x*frequency + _Time.y*speed) + cos(pos.y*frequency + _Time.y*speed)) * amplitude;

				return pos + float4(noise, 0);
			}

			float _Phong;
            float _EdgeLength;
			float _NoiseAmplitude, _NoiseFrequency, _NoiseSpeed;

            /*appdata vert (inout appdata v) {
				appdata o;
				v.vertex = ApplyNoise(v.vertex, _NoiseAmplitude, _NoiseFrequency, _NoiseSpeed);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}*/

			appdata vert (inout appdata v) {
				appdata o;

				v.vertex = ApplyNoise(v.vertex, _NoiseAmplitude, _NoiseFrequency, _NoiseSpeed);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.normal = normalize(UnityObjectToWorldNormal(v.normal));
				o.tangent.xyz = normalize(UnityObjectToWorldDir(v.tangent.xyz));
				o.tangent.w = v.tangent.w * unity_WorldTransformParams.w;

				return o;
     		}

            float4 tessEdge (appdata v0, appdata v1, appdata v2)
            {
                return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
            }

            struct Input {
                float2 uv_MainTex;
				float3 normal;
				float4 tangent;
				float4 worldVertex;
            };

            fixed4 _Color;
            sampler2D _MainTex;

            void surf (Input IN, inout SurfaceOutput o) {

				// build tangent rotation matrix here (saves one interpolator):
				float3 binormal = cross( IN.normal, IN.tangent.xyz) * IN.tangent.w; 
				float3x3 rotation = float3x3( IN.tangent.xyz, binormal, IN.normal );
				
				// get world space normal from position derivatives, and transform it to tangent space:
				half3 flatNormal = - normalize(cross(ddx(IN.worldVertex), ddy(IN.worldVertex))).xyz;
				o.Normal = mul(rotation, flatNormal);

                half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }

            ENDCG
        }
        FallBack "Diffuse"
    }