using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Tenebrae.Projectiles.Inpuratus
{
    public class InpuratusStillFireball : ModProjectile
	{
		public float start = 0;
		Vector2 cen = Vector2.Zero;
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 60;
			projectile.hostile = true;
			projectile.aiStyle = 0;
			projectile.penetrate = 1;      //this is how many enemy this projectile penetrate before disappear
			projectile.extraUpdates = 1;
			aiType = 507;
			projectile.timeLeft = 400;
			Main.projFrames[projectile.type] = 6;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void AI()
		{
			if (start == 0)
            {
				cen = projectile.position;
            }
			start = MathHelper.Lerp(start, 20, 0.05f);
            projectile.velocity = Vector2.Zero;
			projectile.position = cen + (new Vector2((float)Math.Cos((cen.X / 8) + (projectile.ai[0] / 25)), (float)Math.Cos((cen.Y / 6) + (projectile.ai[0] / 24))) * (6 + (start / 3)));
            projectile.ai[0] += 1f;
			projectile.rotation = MathHelper.ToRadians(MathHelper.Clamp(projectile.velocity.X * 0.5f, -10f, 10f));
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			int frame = (int)(projectile.ai[0] / 6 % 6) * (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]);

			for (int i = 1; i < projectile.oldPos.Length; i++)
			{
				projectile.oldPos[i] = projectile.oldPos[i - 1] + (projectile.oldPos[i] - projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(projectile.oldPos[i - 1], projectile.oldPos[i]), start);
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
			for (int i = 0; i < projectile.oldPos.Length; i++)
				{
					spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] + new Vector2(projectile.width / 2, projectile.height / 2) - Main.screenPosition,
					new Rectangle(0, frame, 26, 60), Color.Lerp(Color.DarkOliveGreen, Color.DarkSlateGray, (float)i / (float)projectile.oldPos.Length), projectile.rotation,
					new Vector2(26 * 0.5f, 60 * 0.5f), Vector2.Lerp(new Vector2(1f, 1f), new Vector2(0, 0), (float)i / (float)projectile.oldPos.Length), SpriteEffects.None, 0f);
				}
			spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position + new Vector2(projectile.width / 2, projectile.height / 2) - Main.screenPosition,
			   new Rectangle(0, frame, 26, 60), Color.LimeGreen, projectile.rotation,
			   new Vector2(26 * 0.5f, 60 * 0.5f), 1f, SpriteEffects.None, 0f);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
			return false;
		}

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 3; i++)
			{
				/*DrawEffect.NewDrawEffect(new CursedExplosion(projectile.Center, ));
				if (Main.rand.NextBool(2))
					DrawEffect.NewDrawEffect(new CursedExplosion(projectile.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5))));*/
				Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ProjectileType<CursedExplosion>(), projectile.damage, 4f);
				if (Main.rand.NextBool(2))
					Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5)), ProjectileType<CursedExplosion>(), projectile.damage, 4f);
			}

			Main.PlaySound(SoundID.Item14, (int)projectile.Center.X, (int)projectile.Center.Y);
		}
	}
}