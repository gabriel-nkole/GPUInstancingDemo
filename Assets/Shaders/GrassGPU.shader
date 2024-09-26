// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Custom/GrassGPU" {
    Properties {
        [NoScaleOffset] _GrassTex ("Grass Texture", 2D) = "white" {}
    }

    SubShader {
        Cull Off
        ZWrite On

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
		    #pragma target 4.5

            #include "UnityCG.cginc"
            #include "../Resources/TRS.cginc"


            float _Resolution;
            float4x4 _ParentToWorld;
            float _Angle;

            sampler2D _GrassTex;


            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert(MeshData v, uint instanceID : SV_InstanceID) {
                Interpolators o;
                uint2 id = uint2(instanceID % (uint)_Resolution, instanceID / (uint)_Resolution);

                float3 translation = float3(id.x - 0.5 * (_Resolution - 1), 0.5, id.y - 0.5 * (_Resolution - 1));
	            float4x4 TRS = mul(Translate(translation), mul(Rotate(_Angle), Scale(float3(1,1,1))));

                unity_ObjectToWorld = mul(_ParentToWorld, TRS);
                float4 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, wPos);
                o.uv = v.uv;
                return o;
            }


            float4 frag(Interpolators i) : SV_Target {
                float4 col = tex2D(_GrassTex, i.uv);
                clip(col.a - 0.6);

                return float4(col.xyz, 1);
            }
            ENDCG
        }
    }
}