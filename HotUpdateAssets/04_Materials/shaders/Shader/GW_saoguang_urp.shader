Shader "Shader Forge/GW_saoguang_URP_Lit"
{
    Properties {
        _node_8939 ("Texture A", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _node_6823 ("Multiplier", Float) = 1
        _node_7050 ("Texture B", 2D) = "white" {}
        _node_6112 ("Texture C", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline" }
        Pass {
            Name "URP_Lit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma only_renderers d3d11 glcore gles3 metal
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_node_8939); SAMPLER(sampler_node_8939);
            TEXTURE2D(_node_7050); SAMPLER(sampler_node_7050);
            TEXTURE2D(_node_6112); SAMPLER(sampler_node_6112);

            CBUFFER_START(UnityPerMaterial)
                float4 _node_8939_ST;
                float4 _node_7050_ST;
                float4 _node_6112_ST;
                float4 _Color;
                float _node_6823;
                float _Cutoff;
            CBUFFER_END

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                half lightAmount   : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv = TRANSFORM_TEX(IN.uv, _node_8939);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                VertexNormalInputs normInput = GetVertexNormalInputs(IN.positionOS);
                Light mainLight = GetMainLight();
                OUT.lightAmount = LightingLambert(mainLight.color, mainLight.direction, normInput.normalWS.xyz);

                UNITY_TRANSFER_FOG(OUT, OUT.positionHCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                clip(_Color.a - _Cutoff);

                float4 t1 = SAMPLE_TEXTURE2D(_node_8939, sampler_node_8939, IN.uv);

                float2 timeOffset = float2(_Time.y * 0.1, _Time.y * 0.2);
                float2 uv2 = IN.uv + timeOffset;
                float4 t2 = SAMPLE_TEXTURE2D(_node_7050, sampler_node_7050, TRANSFORM_TEX(uv2, _node_7050));
                float4 t3 = SAMPLE_TEXTURE2D(_node_6112, sampler_node_6112, TRANSFORM_TEX(uv2, _node_6112));

                float3 finalColor = (((t1.rgb * _node_6823) + (t2.a * t3.rgb)) * _Color.rgb);
                finalColor *= IN.lightAmount;

                half4 outCol = half4(finalColor, 1.0);
                UNITY_APPLY_FOG(IN.fogCoord, outCol);
                return outCol;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
