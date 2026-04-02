sampler mobtexture : register(s0);

float value;
float2 Center;
float2 Size;
float2 FrameStart;

float4 PixelShaderFunction (float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    sampleColor = tex2D(mobtexture, uv);
    if (sampleColor.a < 0.5)
    {
        return float4(0, 0, 0, 0);
    }
    uv *= Size;
    uv -= FrameStart;
    float distance = length(Center - uv);
    //if (distance < 40)
    //{
    //    return float4(1, 0, 0, 1);
    //}
    //if (uv.x > 40)
    //{
    //    return float4(1, 1, 1, 1);
    //}
    //if (uv.y<50)
    //{
    //    return float4(0, 0, 1, 1);
    //}
    float aditivevalue = value * 0.7 + value * 0.7 * max((60 - distance) / 60, 0);
    return float4(sampleColor.r + aditivevalue, sampleColor.g + aditivevalue, sampleColor.b + aditivevalue, sampleColor.a);
}

technique Technique1
{ 
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}