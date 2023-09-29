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


float getZ(float d)
{
    float x = d / uEffectRadius;
    float y;
    if (x > 1 || x <= 0)
    {
        return 1;
    } 
    if (0 < x <= 0.7242f)
    {
        y = 5.068f * x * x - 4.110f * x;
    }
    else if (0.7242f < x <= 0.7562f)
    {
        y = 36.316f * x - 26.621f;
    }
    else if (0.7562f < x <= 1)
    {
        y = -29.19f * x * x * x * x + 49.20f * x * x * x - 19.80f * x * x + 0.80f * x;
    }
    /*else
    {
        y = 1 / (16 * x - 15);
    }
    */
    return y;
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

    return tex2D(uImage0, uEffectCenterPos + offset * getZ(dis));
}

technique Technique1
{
    pass Explosion
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}