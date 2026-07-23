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
    float num = (1 - dist * dist * dist) - 0.2;
    if (num <= 0.02)
        return float4(0, 0, 0, 0);
    float num1 = 1 - dist;
    float4 color = float4(num1, num1, num, num);
    return sampleColor * color;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
