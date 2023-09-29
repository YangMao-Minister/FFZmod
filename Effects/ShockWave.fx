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
float uEffectPower;
float uSigma;
float uEffectZoom;


float getVaule(float x, float sigma, int a = -1)
{
    float t = sigma * (uEffectRadius + a * x);
    return max(1 / (t + 0.06 / t), 0);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    float2 offset = (coords - uEffectCenterPos);
    // 因为长宽比不同进行修正
    float dist = length(offset * float2(uScreenResolution.x / uScreenResolution.y, 1));

    float r = uEffectPower * getVaule(dist, uSigma) / 2.0412f; // 函数1 / (t + 0.06 / t) 的最大值
    // 用旋转矩阵旋转向量 
    float2 target = mul(offset, float2x2(cos(r), -sin(r), sin(r), cos(r)));
    // 放大
    return tex2D(uImage0, uEffectCenterPos + target / (dist < uEffectRadius ? (1 + (uEffectZoom - 1) * 2 * (1 / (dist / uEffectRadius + 1) - 0.5f)) : 1));
}

technique Technique1
{
    pass ShockWave
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}