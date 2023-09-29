sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float2 uEffectCenterPos;
float uEffectRadius;
float uEffectZoom;
float uSigma;


float NormalDistribution(float x, float mu, float sigma)
{
    // 魔改正态分布
    return exp(-(x - mu) * (x - mu) / (sigma * sigma));
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
// 屏幕放大效果
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    // pos 就是中心了
    float2 offset = (coords - uEffectCenterPos);
    // 因为长宽比不同进行修正
    // float2 rpos = offset;
    float2 rpos = offset * float2(uScreenResolution.x / uScreenResolution.y, 1);
    float dis = length(rpos);

    return tex2D(uImage0, uEffectCenterPos + offset / (1 + (uEffectZoom - 1) * NormalDistribution(dis, uEffectRadius, uSigma)));
}

technique Technique1
{
    pass SZoom
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}