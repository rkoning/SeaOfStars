Shader "Unlit/HologramShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,0,0,1)
		_SecondaryColor ("Secondary Color", Color) = (1,0,0,1)
		_Bias ("Bias", Float) = 0
		_ScanningFrequency ("Scanning Frequency", Float) = 100
		_ScanningSpeed ("Scanning Speed", Float) = 100
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha One
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				float4 objVertex : TEXCOORD1;
			};

			fixed4 _Color;
			fixed4 _SecondaryColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScanningSpeed;
			float _ScanningFrequency;
			float _Bias;

			float rand(float3 myVector) {
				return frac(sin(_Time[0] * dot(myVector, float3(12.9898, 78.233,45.5432) * 43758.5432)));
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.objVertex = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				col = _Color * max(0, cos(i.objVertex.y * _ScanningFrequency + _Time.x * _ScanningSpeed) + cos(rand(i.objVertex.x)) * _Bias);
				// sub lines
				//col += _SecondaryColor * (1 - max(0, cos(i.objVertex.y * _ScanningFrequency + _Time.x * 3 * _ScanningSpeed))) / 4;
				col += _SecondaryColor * (1 - max(0, cos((i.objVertex.z + i.objVertex.x) * _ScanningFrequency + _Time.x * _ScanningSpeed)  + 0.9));
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
