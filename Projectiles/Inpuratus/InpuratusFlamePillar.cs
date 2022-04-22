namespace Tenebrae.Projectiles.Inpuratus
{
    public class InpuratusFlamePillar : ModProjectile
    {
        float start = 0;
        Vector2 startPos;

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 36;
            projectile.hostile = true;
            projectile.aiStyle = 0;
            projectile.penetrate = 1;      //this is how many enemy this projectile penetrate before disappear
            projectile.extraUpdates = 1;
            aiType = 507;
            projectile.timeLeft = 60;
            projectile.aiStyle = -1;
            Main.projFrames[projectile.type] = 4;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
            projectile.ai[0] = 255;
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
                startPos = projectile.position;
                start = 1;
            }
            if (projectile.timeLeft < 20)
            {
                projectile.ai[0] -= 15;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 0; i < projectile.ai[1]; i++)
            {
                int frame2 = 0;
                if (i > 1) frame2 = 1;
                if (i > 3) frame2 = 2;
                if (i > 4) frame2 = 3;
                int frame = frame2 * (Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]);

                spriteBatch.Draw(Main.projectileTexture[projectile.type], Vector2.Lerp(projectile.position, startPos, (float)i / projectile.ai[1]) + new Vector2(projectile.width / 2, projectile.height / 2) - Main.screenPosition,
                    new Rectangle(0, frame, 34, 36), new Color(255, 255, 255), projectile.rotation,
                    new Vector2(34 * 0.5f, 36 * 0.5f), 1f, SpriteEffects.None, 0f);

            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ProjectileType<CursedExplosion>(), projectile.damage, 4f);
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5)), ProjectileType<CursedExplosion>(), projectile.damage, 4f);
            }

            Main.PlaySound(SoundID.Item14, (int)projectile.Center.X, (int)projectile.Center.Y);
        }
    }
}