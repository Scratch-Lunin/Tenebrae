using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TenebraeMod.Projectiles.Melee
{
	public class MecharangProjectile : ModProjectile
	{
        private int timer;
        private NPC target;
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("MecharangProjectile");
		}

		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 36;
			projectile.aiStyle = 3;
			projectile.friendly = true;
			projectile.melee = true;
            projectile.timeLeft = 800;
            projectile.extraUpdates = 2;
			projectile.maxPenetrate = 6;
            projectile.penetrate = 6;
		}

        public override void AI()
        {
            timer++;
            if (timer == 45)
            {
                target = null;
                timer = 0;
                float distance = 500f;
                projectile.friendly = false;
                int targetID = -1;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && !Main.npc[k].immortal && Main.npc[k].chaseable)
                    {
                        Vector2 newMove = Main.npc[k].Center - Main.MouseWorld;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            targetID = k;
                            distance = distanceTo;
                            projectile.friendly = true;
                        }
                    }
                }
                if (projectile.friendly)
                {
                    target = Main.npc[targetID];

                    Vector2 shotVelocity = target.Center - projectile.Center;
                    shotVelocity.Normalize();
                    shotVelocity *= 16;
                    int shot = Projectile.NewProjectile(projectile.Center, shotVelocity, ModContent.ProjectileType<MecharangLaser>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[shot].minion = false;
                    Main.projectile[shot].melee = true; 
                }
                projectile.friendly = true;
            }
        }
	}
    public class MecharangLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 60;
            projectile.aiStyle = 1;
            aiType = ProjectileID.DeathLaser;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.timeLeft = 800;
            projectile.light = 1f;
            projectile.extraUpdates = 2;
            projectile.maxPenetrate = 3;
        }
    }
}