using Tenebrae.Projectiles.Inpuratus;

namespace Tenebrae.NPCs.Inpuratus
{
    public class CursedSac : ModNPC
    {
        float firescale = 1f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.width = 34;
            npc.height = 34;
            npc.life = 200;
            npc.lifeMax = 200;
            npc.knockBackResist = 0f;
            npc.defense = 0;
            npc.scale = 1f;
            npc.damage = 23;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            float vel = (float)Math.Sqrt(Math.Pow(npc.velocity.X, 2) + Math.Pow(npc.velocity.Y, 2));
            npc.TargetClosest();
            Player player = Main.player[npc.target];
            npc.ai[0]++;

            if (npc.Distance(player.Center) < 90 && npc.ai[1] < 50)
            {
                npc.ai[1] = 50;
            }

            if (npc.ai[1] >= 50)
            {
                npc.ai[1]++;
                npc.velocity *= 0.8f;
                if (npc.ai[1] >= 90) npc.StrikeNPC(Main.rand.Next(2048, 2579), 0f, 1, true);
                firescale = MathHelper.Lerp(firescale, 0, 0.15f);
            }
            else
            {
                npc.velocity += (npc.DirectionTo(player.Center) * 0.35f * (npc.Distance(player.Center) / 150)) + new Vector2(0f, (float)Math.Cos(npc.ai[0] / 9) * 0.2f) * 0.1f;
                npc.velocity += npc.DirectionTo(player.Center) * 0.1f * 0.3f;
                if (vel > 10) npc.velocity *= 0.985f;
                if (vel > 15) npc.velocity *= 0.985f;
                if (vel > 25) npc.velocity *= 0.965f;
                if (vel > 20) npc.velocity = npc.velocity.SafeNormalize(new Vector2(0, 1)) * 20;
                if (npc.ai[2] > 1) npc.velocity.X *= npc.ai[2];
                if (npc.ai[2] < -1) npc.velocity.X *= Math.Abs(npc.ai[2]);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            npc.frame.Y = (int)Math.Floor(npc.frameCounter / 6) % Main.npcFrameCount[npc.type] * npc.height;
        }
        public override bool CheckDead()
        {
            return npc.ai[1] >= 90;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            npc.ai[1] += 50;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void NPCLoot()
        {
            for (int i = 0; i < 3; i++)
            {
                int proj = Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.ProjectileType<CursedExplosion>(), npc.damage, 4f);
                Main.projectile[proj].scale = Main.rand.NextFloat(1.5f, 2.2f);
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5)), ModContent.ProjectileType<CursedExplosion>(), npc.damage, 4f);
            }

            Main.PlaySound(SoundID.Item14, (int)npc.Center.X, (int)npc.Center.Y);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            float rot = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - 1.57f;

            Texture2D tex = mod.GetTexture("Projectiles/Inpuratus/CursedFireball");
            int frame = (int)(npc.ai[0] / 6 % 6) * (tex.Height / 6);
            for (int i = 0; i < 8; i++)
            {
                spriteBatch.Draw(tex, npc.Center + (new Vector2(0, -20).RotatedBy(MathHelper.ToRadians(i * 45)) * firescale) - Main.screenPosition,
                   new Rectangle(0, frame, 26, 60), Color.LimeGreen, rot,
                   new Vector2(tex.Width * 0.5f, (tex.Height * 0.5f / 6) + 20), firescale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            return true;
        }
    }
}

