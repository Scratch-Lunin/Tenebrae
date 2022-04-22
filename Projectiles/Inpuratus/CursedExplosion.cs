namespace Tenebrae.Projectiles.Inpuratus
{
    class CursedExplosion : ModProjectile
    {
        float timer = 0f;

        public override void SetDefaults()
        {
            for (int i = 0; i < 6; i++)
            {
                Dust dust;
                Vector2 position = projectile.Center;
                dust = Main.dust[Dust.NewDust(position, 0, 0, DustID.CursedTorch, Main.rand.Next(-5, 6), Main.rand.Next(-5, 6), 0, new Color(255, 255, 255), 3.552631f)];
                dust.noGravity = true;
                dust.noLight = true;
            }
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

        public override void AI()
        {
            projectile.velocity *= 0.95f;
            timer++;
            if (timer >= (3 * 7)) projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D tex = ModContent.GetTexture("Tenebrae/Projectiles/Inpuratus/CursedExplosion");

            float frame = (float)Math.Floor(timer / 3) * 70;

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition,
                   new Rectangle(0, (int)frame, 70, 70), Color.White, 0f,
                   new Vector2(70 / 2, 70 / 2), projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}
