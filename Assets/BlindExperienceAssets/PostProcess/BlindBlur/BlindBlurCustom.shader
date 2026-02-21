Shader "Shader Graphs/BlindBlur"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BlindBlurPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            float _Iteration;
            float _BlurRadius;
            
            float Rand(float2 n)
            {
                return sin(dot(n,float2(1233.224, 1743.335)));
            }
            
            half4 frag (Varyings input) : SV_Target
            {
                half2 randomOffset = half2(0,0);
                half4 resCol = half4(0,0,0,0);
                float random = Rand(input.texcoord);
            
                for (int i=0;i<(int)_Iteration;i++)
                {
                    random = frac(43758.5453 * random + 0.61432);
                    randomOffset.x = (random - 0.5) * 2.0;
            
                    random = frac(43758.5453 * random + 0.61432);
                    randomOffset.y = (random - 0.5) * 2.0;

                    half2 uv = input.texcoord + randomOffset * _BlurRadius;
                    
                    resCol += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture,uv);
                }
                return resCol/_Iteration;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}