float radius;
float2 Scorspider;
float2 playerPos;

float2 screenPosition;
float2 screenSize;

float InverseLerp(float a, float b, float t)
{
    return saturate((t - a) / (b - a));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    //float textureScale = radius / 1400; // 1400 is abom's ritual size
    
    float2 worldUV = screenPosition + screenSize * uv;
    float worldDistance = distance(Scorspider, worldUV);
    
    // Pixelate the uvs
    float2 pixelatedUV = worldUV / screenSize;

    pixelatedUV.x -= worldUV.x % (1 / screenSize.x);
    pixelatedUV.y -= worldUV.y % (1 / (screenSize.y / 2) * 2);
    
    
    // Textures
    
    float opacity = 0.5;
    
    // Thresholds
    bool border = worldDistance > radius;
    float colorMult = 1;

    float4 color;

    if (border)
    {
        if (worldDistance - radius < 16)
        {
            float mult;
            if (distance(playerPos, worldUV) > 60)
            {
                mult = 0.5;
            }
            else
            {
                mult = (60 - distance(playerPos, worldUV)) / 60 * 0.5 + 0.5;
            }
            color = float4(1, 0.2, 0.2, 1) * mult;
            opacity = 0.65;
        }
        else
        {
            color = float4(0.5, 0, 0, 1);
        }
    }
    else
    {
        color = float4(0, 0, 0, 0);
        opacity = 0;
    }    

    color[3] = opacity;

    return color;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}