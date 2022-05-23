using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Content.Items.Pets
{
    public class HandheldCruxtruder : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + Name;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            DisplayName.SetDefault("Handheld Cruxtruder");
            Tooltip.SetDefault(
                "Releases the Kernelsprite\n" +
                "The Kernelsprite's color is affected by the color of your eyes"
            );
        }

        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.width = 36;
            Item.height = 36;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.value = Item.sellPrice(0, 4, 1, 3);
            Item.rare = ItemRarityID.Lime;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item105;
            Item.useStyle = ItemUseStyleID.Swing; // 1 is the useStyle

            Item.shoot = ModContent.ProjectileType<KernelspriteProjectile>(); // "Shoot" your pet projectile.
            Item.buffType = ModContent.BuffType<KernelBuff>(); // Apply buff upon usage of the Item.
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dowelVelocity = player.Center.DirectionTo(Main.MouseWorld) * Main.rand.Next(2, 3);
            Vector2 perturbedSpeed = new Vector2(dowelVelocity.X, dowelVelocity.Y).RotatedByRandom(MathHelper.ToRadians(10));

            dowelVelocity.X = perturbedSpeed.X;
            dowelVelocity.Y = perturbedSpeed.Y;

            Projectile.NewProjectile(source, player.Center.X, player.Center.Y, dowelVelocity.X, dowelVelocity.Y, ModContent.ProjectileType<CruxtruderDowelProjectile>(), 0, 0, player.whoAmI);

            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }

    public class KernelspriteProjectile : ModProjectile
    {
        public override string Texture => ModAssets.ProjectilesPath + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kernelsprite");

            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            AIType = ProjectileID.BabySkeletronHead;
            Projectile.CloneDefaults(ProjectileID.ZephyrFish); // Copy the stats of the Zephyr Fish

            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.penetrate = -1;
        }

        public override bool PreAI()
        {
            Main.player[Projectile.owner].skeletron = false; // Relic from aiType
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            /*Vector2 dowelVelocity = Main.player[Projectile.owner].DirectionTo(Main.MouseWorld) * Main.rand.Next(2, 3);

                Vector2 perturbedSpeed = new Vector2(dowelVelocity.X, dowelVelocity.Y).RotatedByRandom(MathHelper.ToRadians(10));
                dowelVelocity.X = perturbedSpeed.X;
                dowelVelocity.Y = perturbedSpeed.Y;
                Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, dowelVelocity.X, dowelVelocity.Y, Mod.Find<ModProjectile>("Dowel").Type, 0, 0, Projectile.owner);
            */
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            if (!player.dead && player.HasBuff(ModContent.BuffType<KernelBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;

            Projectile.rotation = 0f;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        /*public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //modified trail code, OG by turingcomplete30
        {
            Texture2D texture = ModContent.GetTexture("ScratchTest/Pets/Kernelsprite_Color");

            Color mainColor = Main.player[projectile.owner].eyeColor;

            for (int k = projectile.oldPos.Length - 1; k >= 0; k--)
            {
                Color color = mainColor * ((float)(projectile.oldPos.Length - k - 0.5f) / (float)projectile.oldPos.Length);
                float scale = projectile.scale;

                Vector2 newerPos = projectile.position;
                if (k + 1 < projectile.oldPos.Length)
                {
                    newerPos = projectile.oldPos[k + 1];
                }

                spriteBatch.Draw(texture, projectile.oldPos[k] + (projectile.Size / 2f) - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, 0f, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2), scale, SpriteEffects.None, 1f);
                
                scale = projectile.scale * ((projectile.oldPos.Length - k) / projectile.oldPos.Length);
                color = mainColor * ((projectile.oldPos.Length - k) / projectile.oldPos.Length);
                spriteBatch.Draw(texture, projectile.Center - projectile.position + (projectile.oldPos[k] + newerPos) / 2 - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, 0f, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2), scale, SpriteEffects.None, 1f);

                //Color colorTest = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / projectile.oldPos.Length);
                //spriteBatch.Draw(texture, projectile.oldPos[k] - Main.screenPosition + new Vector2(24), texture.Frame(2, 1, projectile.frame), colorTest, 0f, new Vector2(24), scale, SpriteEffects.None, 1f);
            }

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), mainColor, projectile.rotation, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2), projectile.scale, SpriteEffects.None, 1f);
            return false;
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            //Redraw the projectile with the color not influenced by light
            //Texture2D texture = ModContent.GetTexture("ScratchTest/Pets/Kernelsprite_Color");
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height / Main.projFrames[Projectile.type] * 0.5f);
            Rectangle rect = new(0, Projectile.frame * texture.Height / Main.projFrames[Projectile.type], texture.Width, texture.Height / Main.projFrames[Projectile.type]);
            Color eyeColor = Main.player[Projectile.owner].eyeColor;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Color color = eyeColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture, drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, (int)0f);
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Color").Value;
            Color eyeColor = Main.player[Projectile.owner].eyeColor;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(2);
            Rectangle rectangle = new(0, Projectile.frame * texture.Height / Main.projFrames[Projectile.type], texture.Width, texture.Height / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(texture, position, rectangle, eyeColor, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / Main.projFrames[Projectile.type] / 2), Projectile.scale, SpriteEffects.None, (int)1f);
        }
    }

    public class CruxtruderDowelProjectile : ModProjectile
    {
        public override string Texture => ModAssets.ProjectilesPath + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cruxite Dowel");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = true;
            Projectile.penetrate = 8;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity

            if (Projectile.velocity.Y > 10f) // Terminal vel
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.rotation += Projectile.velocity.X / 20;
            //projectile.velocity.X = projectile.velocity.X * 0.99f; // 0.99f for rolling grenade speed reduction. Try values between 0.9f and 0.99f

            if (Projectile.penetrate > 1)
            {
                Projectile.ai[0] = 60 * 3;
            }
            else
            {
                Projectile.ai[0]--;

                if (Projectile.ai[0] == 0)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color eyeColor = Main.player[Projectile.owner].eyeColor;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), eyeColor, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, SpriteEffects.None, (int)0f);
            return false;
        }

        public override void Kill(int timeLeft) //spawn dust on death
        {
            Color eyeColor = Main.player[Projectile.owner].eyeColor;

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 5, 5, DustID.Stone, 0f, 0f, 0, eyeColor, 1f)];
                dust.noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity) //bounce code
        {
            if (Projectile.penetrate > 1) Projectile.penetrate--;
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

            Projectile.velocity.X *= 0.3f;
            Projectile.velocity.Y *= 0.2f;

            return false;
        }
    }

    public class KernelBuff : ModBuff
    {
        public override string Texture => ModAssets.BuffsPath + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kernelsprite");
            Description.SetDefault("An unprototyped Kernelsprite is following you");

            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<KernelspriteProjectile>()] <= 0;

            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<KernelspriteProjectile>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}