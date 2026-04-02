float2 positions[45];
float scales[45];
float4 colors[45];
float2 normals[44];
int count;
float2 screenPosition;
float2 screenSize;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float2 worldUV = screenPosition + screenSize * uv;
    float4 color = float4(0,0,0,0);
    float value = 0;
    float alpha = 0;
    int j = 0;
    for (int i = 0; i < count - 1; i++)
    {
        float2 hui = positions[i];
        float2 hui1 = positions[i + 1];
        float lenght = distance(hui, worldUV); //((hui.x - worldUV.x) * (hui.x - worldUV.x)) + ((hui.y - worldUV.y) * (hui.y - worldUV.y));
        float hui2 = scales[i];
        float hui3 = scales[i + 1];
        float4 hui4 = colors[i];
        float4 hui5 = colors[i + 1];
        float2 hui6 = normals[i];
        float value1 = (hui2 - lenght) / (hui2);
        //if (i > 0)
        //{
        //    float a = distance(positions[i - 1], positions[i]);
        //    float s = abs(positions[i - 1].x * (positions[i].y - worldUV.y) + positions[i].x * (worldUV.y - positions[i - 1].y) + worldUV.x * (positions[i - 1].y - positions[i].y)) * 0.5;
        //    float h = s * 2 / a;
        //    float value2 = max(distance(positions[i - 1], positions[i]) - distance(positions[i], worldUV), 0) / distance(positions[i - 1], positions[i]);
        //    //value1 += max((0.5 -distance(normalize(positions[i - 1] - positions[i]), normalize(worldUV - positions[i]))) * 0.35, 0) * scales[i] * value2;
        //    value1 += h * scales[i] * value2;
        //}
        //if (i < count - 1)
        //{
        
        float a = distance(hui1, hui);
        float num1 = hui1.x * (hui.y - worldUV.y);
        float num2 = hui.x * (worldUV.y - hui1.y);
        float num3 = worldUV.x * (hui1.y - hui.y);
        num1 = abs(num1 + num2 + num3) * 0.5;
        num2 = num1 * 2 / a;
        float2 vec2;
        if (distance(hui, worldUV) > distance(hui + hui6, worldUV))
        {
            vec2 = worldUV - hui6 * num2;
        }
        else
        {
            vec2 = worldUV + hui6 * num2;
        }
        if (distance(hui, vec2) < distance(hui, hui1) && distance(hui1, vec2) < distance(hui, hui1))
        {
            a = max(distance(hui1, hui) - distance(hui1, vec2), 0) / distance(hui1, hui);
            num1 = lerp(hui2, hui3, a);
            value1 = max((num1 - num2) / num1, value1);
            color += (hui4 - (hui4 - hui5) * a) * value1;
            i++;
            value += value1;
            alpha = max(alpha, value1);
        }
        else
        {
            color += hui4 * value1;
            value += value1;
            alpha = max(alpha, value1);
        }
        j = i;
        //}
        
        //if (value1 > 0)
        //{
        //    //return float4(0, 0, 1, 1);
        //    //return float4(0, 0, 0, 1);
        //}
    }
    j++;
    if (j == count - 1)
    {
        float lenght = distance(positions[j], worldUV); //((hui.x - worldUV.x) * (hui.x - worldUV.x)) + ((hui.y - worldUV.y) * (hui.y - worldUV.y));
        float value1 = (scales[j] - lenght) / (scales[j]);
        color += colors[j] * value1;
        value += value1;
        alpha = max(alpha, value1);
    }
    return color / value * alpha;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_sw PixelShaderFunction();
    }
}