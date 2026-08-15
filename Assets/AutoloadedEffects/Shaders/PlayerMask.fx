sampler2D inputTexture : register(s0);

int h;
int w;
float2 player;
float radius1;
float radius2;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 screen = coords;
    screen.x *= w;
    screen.y *= h;
    float dist = distance(screen, player);
    if (dist < radius1)
    {
        return float4(0, 0, 0, 0);
    }
    if (dist > radius2)
    {
        return tex2D(inputTexture, coords);
    } 
    float p = min((dist - radius1) / (radius2 - radius1), 1.0);
    return tex2D(inputTexture, coords) * p;
}

technique Technique1
{
	pass AutoloadPass
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}