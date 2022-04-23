using Microsoft.Xna.Framework;
using Tenebrae.Common;

namespace Tenebrae.Content.Particles
{
    public class AsteriteParticle : Particle
    {
        public override string Texture => ModAssets.MiscPath + "Extra_1";

        public override void OnSpawn()
        {
            timeLeft = 60;
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.05f;
            scale *= 0.9f;

            if (--timeLeft <= 0 || scale <= 0.2f)
            {
                Kill();
            }
        }

        public override Color GetAlpha(Color lightColor) => Color.White;

        protected override bool IsAdditive => true;

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.8f;
            return true;
        }
    }
}
