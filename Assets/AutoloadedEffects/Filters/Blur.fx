sampler2D inputTexture : register(s0);

int w;
int h;

float2 center;
float radius1;
float radius2;
float power;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 screen = uv;
    screen.x *= w;
    screen.y *= h;
    float dist = distance(screen, center);
    if (dist < radius1)
    {
        return tex2D(inputTexture, uv);
    }
    float4 color = tex2D(inputTexture, uv) * 0.4;
    float p = min((dist - radius1) / (radius2 - radius1), 1.0) * power;
    float2 vectors[8] = {
        float2(1, 0),
        float2(0.707106, -0.707106),
        float2(0, -1),
        float2(-0.707106, -0.707106),
        float2(-1, 0),
        float2(-0.707106, 0.707106),
        float2(0, 1),
        float2(0.707106, 0.707106),
    };
    float weight = 0.0375;
    [unroll]
    for (int i = 0; i < 8; i++)
    {
        float2 offset = vectors[i] * p;
        offset.x /= w;
        offset.y /= h;
        color += tex2D(inputTexture, uv + offset) * weight;
    }
    float2 vectors2[12] = {
        float2(2, 0),
        float2(1.7320508, -1),
        float2(1, -1.7320508),
        float2(0, -2),
        float2(-1, -1.7320508),
        float2(-1.7320508, -1),
        float2(-2, 0),
        float2(-1.7320508, 1),
        float2(-1, 1.7320508),
        float2(0, 2),
        float2(1, 1.7320508),
        float2(1.7320508, 1),
    };
    float weight2 = 0.01666666;
    [unroll]
    for (int j = 0; j < 12; j++)
    {
        float2 offset = vectors2[j] * p;
        offset.x /= w;
        offset.y /= h;
        color += tex2D(inputTexture, uv + offset) * weight2;
    }
    float2 vectors3[16] = {
        float2(3, 0),
        float2(2.77163285, -1.14805029),
        float2(2.12132034, -2.12132034),
        float2(1.14805029, -2.77163285),
        float2(0, -3),
        float2(-1.14805029, -2.77163285),
        float2(-2.12132034, -2.12132034),
        float2(-2.77163285, -1.14805029),
        float2(-3, 0),
        float2(-2.77163285, 1.14805029),
        float2(-2.12132034, 2.12132034),
        float2(-1.14805029, 2.77163285),
        float2(0, 3),
        float2(1.14805029, 2.77163285),
        float2(2.12132034, 2.12132034),
        float2(2.77163285, 1.14805029),
    };
    float weight3 = 0.00625;
    [unroll]
    for (int k = 0; k < 16; k++)
    {
        float2 offset = vectors3[k] * p;
        offset.x /= w;
        offset.y /= h;
        color += tex2D(inputTexture, uv + offset) * weight3;
    }
    return color;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}