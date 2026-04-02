float lenght;
float width;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //if (width > 1)
    //{
    //    sampleColor.b = 1;
    //}
    //if (lenght > 1)
    //{
    //    sampleColor.g = 1;
    //}
    float kef = lenght / width;
    coords.y *= kef;
    float dist = distance(coords.y, 0.5 * kef);
    if (coords.y > kef - 0.5)
    {
        dist = distance(coords, float2(0.5, kef - 0.5));
        if (dist > 0.5)
        {
            return float4(0, 0, 0, 0);
        }
        dist *= 2;
        dist *= dist * dist;
        dist = 1 - dist;
        sampleColor *= dist;
    }
    else if (coords.y < 0.5)
    {
        dist = distance(coords, float2(0.5, 0.5));
        if (dist > 0.5)
        {
            return float4(0, 0, 0, 0);
        }
        dist *= 2;
        dist *= dist * dist;
        dist = 1 - dist;
        sampleColor *= dist;
    }
    else
    {
        dist = distance(coords.x, 0.5);
        if (dist > 0.5)
        {
            return float4(0, 0, 0, 0);
        }
        dist *= 2;
        dist *= dist * dist;
        dist = 1 - dist;
        sampleColor *= dist;
    }
    //dist = 0.5 - dist;
    //dist *= 2;
    //sampleColor *= dist * dist * dist;
    return sampleColor;
}

technique Technique1
{
	pass AutoloadPass
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}