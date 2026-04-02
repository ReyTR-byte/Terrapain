float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    if (coords.x < 0.1)
    {
        coords.x *= 5;
        float dist = distance(coords, float2(0.5, 0.5));
        dist += coords.x * 0.5 / 5;
        if (dist > 0.5)
        {
            return float4(0, 0, 0, 0);
        }
        dist *= 2;
        dist *= dist * dist;
        dist = 1 - dist;
        return sampleColor *= dist;
    }
    else
    {
        float dist = distance(coords.y, 0.5);
        dist += coords.x * 0.5;
        if (dist > 0.5)
        {
            return float4(0, 0, 0, 0);
        }
        dist *= 2;
        dist *= dist * dist;
        dist = 1 - dist;
        return sampleColor * dist;
    }
    
}

technique Technique1
{
	pass AutoloadPass
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}