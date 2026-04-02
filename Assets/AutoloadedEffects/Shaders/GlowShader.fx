sampler2D sample : register(s0);
float4 color;
//matrix mat;

float Rotation(float2 target)
{
    float rotation = acos(target.x);
    if (target.y < 0)
    {
        rotation *= -1;
    }
    return rotation;
}
//struct VertexShaderInput
//{
//    float4 pos : POSITION0;
//    float2 uv : TEXCOORD0;
//};
//struct VertexShaderOutput
//{
//    float4 pos : SV_Position;
//    float2 uv : TEXCOORD0;
//    //float4 color : COLOR0;
//};
//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//    VertexShaderOutput output;
//    input.uv -= float2(0.5, 0.5);
//    output.uv.x = 1; //Rotation(normalize(input.uv)) / 6.2831853 + 0.5;
//    output.uv.y = max(1 - length(input.uv) * 2, 0);
//    output.pos = mul(input.pos, mat);
//    //output.color = tex2D(sample, output.uv);
//    return output;
//}

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    uv -= float2(0.5, 0.5);
    float2 newUV;
    newUV.x = Rotation(normalize(uv)) / 6.2831853 + 0.5;
    newUV.y = max(1 - length(uv) * 2, 0);
    float4 sampleColor = tex2D(sample, newUV);
    return sampleColor * (newUV.y + sampleColor.a - 1) * color;
}

technique Technique1
{
    pass AutoloadPass
    {
        //VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}