Shader "Hidden/DepthFogVolumeURP"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            float4 _FogColor;
            float _FogStart;
            float _FogRange;
            float _FogPower;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float4 col =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float rawDepth =
                    SAMPLE_TEXTURE2D(
                        _CameraDepthTexture,
                        sampler_CameraDepthTexture,
                        i.uv
                    ).r;

                float depth =
                    LinearEyeDepth(rawDepth, _ZBufferParams);

                float fog =
                    saturate((depth - _FogStart) / _FogRange);

                fog = pow(fog, _FogPower);

                col.rgb =
                    lerp(col.rgb, _FogColor.rgb, fog);

                return fog;
            }
            ENDHLSL
        }
    }
}
