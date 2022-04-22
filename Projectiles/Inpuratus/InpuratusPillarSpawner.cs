using Tenebrae;
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Terraria.ModLoader;

namespace Tenebrae.Projectiles.Inpuratus
{
    class InpuratusPillarSpawner : ModProjectile
    {
        float timer = 0f;

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.hostile = true;
            projectile.aiStyle = 0;
            projectile.penetrate = 1;      //this is how many enemy this projectile penetrate before disappear
            projectile.extraUpdates = 1;
            aiType = 507;
            projectile.timeLeft = 200;
            Main.projFrames[projectile.type] = 7;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            projectile.ai[0]++;
            if (projectile.ai[0] % 2 == 0)
            {
                Vector2 dest1 = projectile.Center;
                while (!Main.tileSolid[Main.tile[(int)(dest1.X / 16), (int)(dest1.Y / 16)].type])
                {
                    dest1.Y += 16;
                }    

                Projectile.NewProjectile(dest1, (projectile.Center - dest1) / 60 * 10, ModContent.ProjectileType<InpuratusFlamePillar>(), 10, 5, 255, 0, 16);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
    }
}
