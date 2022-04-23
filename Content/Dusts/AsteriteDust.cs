using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Tenebrae.Content.Dusts
{
    public class AsteriteDust : ModDust
    {
        public static readonly Color LightColor = new(244, 179, 12);

        public override string Texture => ModAssets.DustsPath + nameof(AsteriteDust);

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.7f;
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.93f;

            Lighting.AddLight(dust.position, LightColor.ToVector3() * 0.15f * dust.scale);

            if (dust.scale < 0.3f)
            {
                dust.active = false;
            }

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White;
    }
}