float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    if (uv.x > 0.5)
    {
        return float4(0, 0, 0, 0);
    }
    float dist = length(uv - float2(0.5, 0.5));
    if (dist > 0.5)
    {
        return float4(0, 0, 0, 0);
    }
    dist *= 2;
    return sampleColor * 1 - (dist * dist * dist);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
