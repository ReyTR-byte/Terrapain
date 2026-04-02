float2 p1;
float2 p2;
float2 p3;
float2 p4;
float2 p5;
float2 p6;
float2 p7;
float2 p8;
float opacity;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float3 color = float3(1, 0, 0);
    color += float3(1 / (distance(p1.x - uv.x, p1.y - uv.y) + 1), 1 / (distance(p1.x - uv.x, p1.y - uv.y) + 1), 1 / (distance(p1.x - uv.x, p1.y - uv.y) + 1));
    color += float3(1 / (distance(p2.x - uv.x, p2.y - uv.y) + 1), 1 / (distance(p2.x - uv.x, p2.y - uv.y) + 1), 1 / (distance(p2.x - uv.x, p2.y - uv.y) + 1));
    color += float3(1 / (distance(p3.x - uv.x, p3.y - uv.y) + 1), 1 / (distance(p3.x - uv.x, p3.y - uv.y) + 1), 1 / (distance(p3.x - uv.x, p3.y - uv.y) + 1));
    color += float3(1 / (distance(p4.x - uv.x, p4.y - uv.y) + 1), 1 / (distance(p4.x - uv.x, p4.y - uv.y) + 1), 1 / (distance(p4.x - uv.x, p4.y - uv.y) + 1));
    color += float3(1 / (distance(p5.x - uv.x, p5.y - uv.y) + 1), 1 / (distance(p5.x - uv.x, p5.y - uv.y) + 1), 1 / (distance(p5.x - uv.x, p5.y - uv.y) + 1));
    color += float3(1 / (distance(p6.x - uv.x, p6.y - uv.y) + 1), 1 / (distance(p6.x - uv.x, p6.y - uv.y) + 1), 1 / (distance(p6.x - uv.x, p6.y - uv.y) + 1));
    color += float3(1 / (distance(p7.x - uv.x, p7.y - uv.y) + 1), 1 / (distance(p7.x - uv.x, p7.y - uv.y) + 1), 1 / (distance(p7.x - uv.x, p7.y - uv.y) + 1));
    color += float3(1 / (distance(p8.x - uv.x, p8.y - uv.y) + 1), 1 / (distance(p8.x - uv.x, p8.y - uv.y) + 1), 1 / (distance(p8.x - uv.x, p8.y - uv.y) + 1));
    color/= color.r;
    return float4(color.r * opacity, color.b * opacity, color.g * opacity, 1);
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}