texture texture0 : register(s0);
sampler textureSampler0 = sampler_state
{
    texture = <texture0>;
};

texture texture1 : register(s1);
sampler textureSampler1 = sampler_state
{
    texture = <texture1>;
};

float time;
float scale;

float4 AnimatedModName(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 t0 = tex2D(textureSampler0, coords);
    float4 t1 = tex2D(textureSampler1, coords * scale + float2(time, 0));

    t0.rgb *= t1.rgb;

    return t0 * sampleColor;
}

technique Technique1
{
    pass AnimatedModName
    {
        PixelShader = compile ps_2_0 AnimatedModName();
    }
}