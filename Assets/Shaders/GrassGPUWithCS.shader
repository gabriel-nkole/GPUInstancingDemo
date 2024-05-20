// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Custom/GrassGPUWithCS" {
    Properties {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off
        ZWrite On

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
		    #pragma target 4.5

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            StructuredBuffer<float4x4> _Matrices;

            Interpolators vert (MeshData v, uint instanceID : SV_InstanceID) {
                Interpolators o;

                unity_ObjectToWorld = _Matrices[instanceID];
                o.vertex = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, v.vertex));
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv);
                clip(col.a - 0.6);
                return float4(col.xyz, 1);
            }
            ENDCG
        }
    }
}
