// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Custom/Grass" {
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
            #pragma multi_compile_instancing
		    #pragma target 4.5

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;

            Interpolators vert (MeshData v, uint instanceID : SV_INSTANCEID) {
                Interpolators o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID (i);
                
                float4 col = tex2D(_MainTex, i.uv);
                clip(col.a - 0.6);
                return float4(col.xyz, 1);
            }
            ENDCG
        }
    }
}
