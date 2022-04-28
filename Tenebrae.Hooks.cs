using Microsoft.Xna.Framework.Graphics;
using Tenebrae.Common;
using Tenebrae.Common.AdditiveDrawing;
using Tenebrae.Common.Particles;
using Tenebrae.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace Tenebrae
{
    public partial class Tenebrae : Mod
    {
        private static void LoadHooks()
        {
            On.Terraria.Main.SortDrawCacheWorms += ModifySortDrawCacheWorms;
            On.Terraria.Main.DrawDust += ModifyDrawDust;
            On.Terraria.Main.DoDraw_UpdateCameraPosition += DoAfterCameraUpdate;
        }

        private static void UnloadHooks()
        {
            On.Terraria.Main.SortDrawCacheWorms -= ModifySortDrawCacheWorms;
            On.Terraria.Main.DrawDust -= ModifyDrawDust;
            On.Terraria.Main.DoDraw_UpdateCameraPosition -= DoAfterCameraUpdate;
        }

        private static void ModifySortDrawCacheWorms(On.Terraria.Main.orig_SortDrawCacheWorms orig, Main main)
        {
            orig(main);

            var canDraw = false;
            canDraw |= AdditiveDrawSystem.Any(behindEntities: true);

            if (canDraw) // Additive
            {
                var spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                {
                    AdditiveDrawSystem.Draw(behindEntities: true);
                }
                spriteBatch.End();
            }
        }

        private static void ModifyDrawDust(On.Terraria.Main.orig_DrawDust orig, Main main)
        {
            orig(main);

            var spriteBatch = Main.spriteBatch;
            var canDraw = false;
            canDraw |= ParticleSystem.Any(additive: false);

            if (canDraw) // AlphaBlend
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                {
                    ParticleSystem.Draw(additive: false);
                }
                spriteBatch.End();
            }

            canDraw = false;
            canDraw |= AdditiveDrawSystem.Any(behindEntities: false);
            canDraw |= ParticleSystem.Any(additive: true);

            if (canDraw) // Additive
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                {
                    AdditiveDrawSystem.Draw(behindEntities: false);
                    ParticleSystem.Draw(additive: true);
                }
                spriteBatch.End();
            }
        }

        private static void DoAfterCameraUpdate(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();

            foreach (var system in Tenebrae.Instance.GetModSystems())
            {
                if (system is IAfterUpdatingCameraPosition obj)
                {
                    obj.PostUpdateCameraPosition();
                }
            }

            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.ModProjectile is IAfterUpdatingCameraPosition obj)
                {
                    obj.PostUpdateCameraPosition();
                }
            }
        }
    }
}