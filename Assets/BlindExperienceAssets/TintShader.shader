Shader "Hidden/Custom/Tint"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Tint"
            
            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _BlurRadius;
            float _Iteration;
            float4 _TintColor;

            float Rand(float2 n)
            {
                return sin(dot(n,float2(1233.224, 1743.335)));
            }

            float Random(float2 uv, float seed)
            {
                // 混合UV和随机种子，打破行相关性
                float2 noiseUV = uv * float2(1973.123, 2654.431) + seed;
                return frac(sin(dot(noiseUV, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            half4 Frag(Varyings input, SamplerState blitsampler) : SV_Target
            {
                half4 resCol = half4(0,0,0,0);
                // 时间种子：让随机模式随时间变化，避免静态条纹
                float timeSeed = _Time.y * 0.3;
                // 初始随机种子：用UV+时间，彻底打散行相关性
                float random = Random(input.texcoord, timeSeed + input.texcoord.x * input.texcoord.y);
                
                for (int i=0;i<_Iteration;i++)
                {
                    // 生成二维随机数（0-1范围）
                    random = frac(random * 43758.5453 + 0.12345);
                    float rndX = random;
                    random = frac(random * 43758.5453 + 0.67890);
                    float rndY = random;

                    // 关键：基于像素尺寸计算偏移（适配0-1 UV）
                    // 将随机数转为[-1,1]范围，乘以模糊强度和像素尺寸，得到UV偏移
                    float2 pixelOffset = float2(rndX * 2 - 1, rndY * 2 - 1) * 2;
                    float2 uvOffset = pixelOffset*_BlurRadius*0.1;

                    // 计算最终采样UV，并限制在0-1范围内（防止越界）
                    float2 sampleUV = input.texcoord + uvOffset;

                    // 线性采样+边缘夹紧：避免重复采样导致的硬边
                    resCol += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearRepeat, sampleUV);
                }
                
                return resCol/_Iteration;
            }

            ENDHLSL
        }
    }
}
