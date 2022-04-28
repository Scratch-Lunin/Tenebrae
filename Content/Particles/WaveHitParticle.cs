using Microsoft.Xna.Framework;
using System;
using Tenebrae.Common.Particles;

namespace Tenebrae.Content.Particles
{
    public class WaveHitParticle : Particle
    {
        public override string Texture => ModAssets.MiscPath + "Extra_2";

        public override void OnSpawn()
        {
            timeLeft = 30;
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            velocity *= 0.95f;

            if (--timeLeft <= 0)
            {
                Kill();
            }
        }

        public override Color GetAlpha(Color lightColor) => Color.White * MathF.Sin(timeLeft / 30f * MathHelper.Pi);

        protected override bool IsAdditive => true;
    }
}