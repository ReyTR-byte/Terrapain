float internalRadius;
float4 internalColor;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    if (internalRadius == 0.5)
    {
        return float4(0, 0, 0, 0);
    }
    float dist = distance(uv, float2(0.5, 0.5));
    if (dist > 0.5)
    {
        return float4(0, 0, 0, 0);
    }
    if (dist < internalRadius)
    {
        return internalColor;
    }
    dist -= internalRadius;
    dist *= 0.5 / (0.5 - internalRadius);
    dist *= 2;
    return sampleColor * dist + internalColor * (1 - dist);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}