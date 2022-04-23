using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Tenebrae.Common;
using Terraria;
using Terraria.ModLoader;

namespace Tenebrae
{
    public partial class Tenebrae : Mod
    {
        private static void LoadHooks()
        {
            On.Terraria.Main.DrawDust += ModifyDrawDust;
        }

        private static void UnloadHooks()
        {
            On.Terraria.Main.DrawDust -= ModifyDrawDust;
        }

        private static void ModifyDrawDust(On.Terraria.Main.orig_DrawDust orig, Main main)
        {
            orig(main);

            var spriteBatch = Main.spriteBatch;
            var elems = new List<IDrawAdditive>();

            foreach (var projectile in Main.projectile)
            {
                if (projectile.ModProjectile is IDrawAdditive additive)
                {
                    elems.Add(additive);
                }
            }

            if (ParticleSystem.ActiveAlphaBlendParticles > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                ParticleSystem.DrawParticles(false);
                spriteBatch.End();
            }

            if (elems.Count == 0 && ParticleSystem.ActiveAdditiveParticles == 0) return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            {
                foreach (var elem in elems)
                {
                    elem.DrawAdditive(spriteBatch);
                }

                ParticleSystem.DrawParticles(true);
            }
            spriteBatch.End();
        }
    }
}