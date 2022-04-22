namespace Tenebrae.Items.Misc
{
    public class WarriorDebuffTest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit of the Warrior");
            Tooltip.SetDefault("Unleashes the bane of the Warrior upon your enemies via condensed bullets of energy.");
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.width = 18;
            item.height = 34;
            item.noMelee = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = mod.ProjectileType("WarriorDebuffTestProj");
            item.shootSpeed = 5;
            item.maxStack = 1;
            item.UseSound = SoundID.Item20;
            Item.sellPrice(66, 11, 22, 14);
            item.rare = -20;
            // Set other item.X values here
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TenebraeWorld.timer++;
            if (TenebraeWorld.timer == 380)
            {
                TenebraeWorld.timer = 0;
            }
            int r = 255;
            int g = (int)(100 * (1 + Math.Sin(TenebraeWorld.timer / 60f)));
            int b = 0;
            var line = new TooltipLine(mod, "FavoriteItem", "Favorite this item to unleash the Warrior's animosity upon yourself.")
            {
                overrideColor = new Color(r, g, b)
            };
            tooltips.Add(line);
            line = new TooltipLine(mod, "UnobtainableItem", "[ Unobtainable Item ]");
            tooltips.Add(line);
        }

        public override void UpdateInventory(Player player)
        {
            if (item.favorited)
            {
                player.AddBuff(ModContent.BuffType<Buffs.WarriorsAnimosity>(), 60, true);
            }
        }
    }

    public class WarriorDebuffTestProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scarlet Bullet");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 9999; // Added extra penetration in the event that it hits 0 before the explode code can run
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60 * 3;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];

            Color mainColor = Color.White;

            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Color color = mainColor * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                float scale = projectile.scale * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

                float rotation;
                if (k + 1 >= projectile.oldPos.Length)
                {
                    rotation = (float)((projectile.oldPos[k] - projectile.position).ToRotation() + MathHelper.PiOver2 / 2);
                }
                else
                {
                    rotation = (float)((projectile.oldPos[k] - projectile.oldPos[k + 1]).ToRotation() + MathHelper.PiOver2 / 2);
                }

                spriteBatch.Draw(texture, projectile.Center - projectile.position + projectile.oldPos[k] - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, rotation, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2), scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), mainColor, projectile.rotation, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            projectile.velocity *= 0.97f;
            if (++projectile.frameCounter >= 7)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(projectile.position - new Vector2(2f, 2f), projectile.width + 4, projectile.height + 4, DustID.HealingPlus, 0f, 0f, 100, new Color(255, 0, 0), 1f);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(projectile.position - new Vector2(2f, 2f), projectile.width + 4, projectile.height + 4, DustID.FlameBurst, 0f, 0f, 100, new Color(255, 0, 0), .8f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        { // TODO: Add OnHitPlayer method for PVP?
            target.AddBuff(ModContent.BuffType<Buffs.WarriorsAnimosity>(), 60 * 3, true);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position - new Vector2(2f, 2f), projectile.width + 4, projectile.height + 4, DustID.HealingPlus, 0f, 0f, 100, new Color(255, 0, 0), 1f);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(projectile.position - new Vector2(2f, 2f), projectile.width + 4, projectile.height + 4, DustID.FlameBurst, 0f, 0f, 100, new Color(255, 0, 0), .8f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}