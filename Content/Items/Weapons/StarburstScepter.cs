using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tenebrae.Common;
using Tenebrae.Common.AdditiveDrawing;
using Tenebrae.Common.Particles;
using Tenebrae.Common.PlayerDrawLayers;
using Tenebrae.Content.Dusts;
using Tenebrae.Content.Items.Materials;
using Tenebrae.Content.Particles;
using Tenebrae.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Content.Items.Weapons
{
    public class StarburstScepter : ModItem
    {
        public static readonly Color LightColor = new(244, 179, 12);

        // ...

        public override string Texture => ModAssets.ItemsPath + nameof(StarburstScepter);

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            DisplayName.SetDefault("Starburst Scepter");
            Tooltip.SetDefault(
                "Summons orbiting stars that fly towards the cursor\n" +
                "Orbiting stars disappear when the scepter isn't held"
            );

            Item.staff[Type] = true;

            HeldItemLayer.RegisterItemGlowmask(Type, DrawItemGlowmask);
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 56;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 15;
            Item.crit = 0;
            Item.knockBack = 4f;
            Item.mana = 10;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item45;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<StarburstStarProjectile>();
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AsteriteIngot>(5).AddTile(TileID.Anvils).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var stars = Main.projectile.ToList().FindAll(proj => proj.active && proj.type == Item.shoot && proj.owner == player.whoAmI && (proj.ModProjectile as StarburstStarProjectile).IsReadyToAttack);
            var starsCount = stars.Count;

            if (starsCount > 0)
            {
                DustUtils.SpawnDustCircle(Main.MouseWorld, 16, 17, (type) => ModContent.DustType<AsteriteDust>(), (dust, index, angle) =>
                {
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    dust.velocity = new Vector2(Main.rand.NextFloat(1.5f)).RotatedBy(angle);
                    dust.noLight = true;
                });

                (stars[Main.rand.Next(starsCount)].ModProjectile as StarburstStarProjectile).SetTargetPosition(Main.MouseWorld);
            }

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return Main.projectile.Any(proj => proj.active && proj.type == Item.shoot && proj.owner == player.whoAmI && (proj.ModProjectile as StarburstStarProjectile).IsReadyToAttack);
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[Item.shoot] < 3)
            {
                Dictionary<int, int> stars = new();
                int maxTimer = 0;

                // Find old stars
                foreach (var proj in Main.projectile.ToList().FindAll(i => i.owner == player.whoAmI && i.type == Item.shoot && i.active))
                {
                    var starProj = proj.ModProjectile as StarburstStarProjectile;
                    stars.Add(starProj.Index, proj.whoAmI);
                    maxTimer = Math.Max(maxTimer, starProj.Timer);
                }

                // Creating new ones with a unique index
                for (int index = 0; index < 3; index++)
                {
                    if (stars.ContainsKey(index)) continue;

                    var proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI, ai0: index);
                    stars.Add(index, proj);
                }

                // Setting the same timer
                foreach (var projIndex in stars.Values)
                {
                    var proj = Main.projectile[projIndex];
                    proj.netUpdate = true;
                    var starProj = proj.ModProjectile as StarburstStarProjectile;
                    starProj.Timer = maxTimer;
                }
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Lighting.AddLight(player.itemLocation, LightColor.ToVector3() * 0.2f * Main.essScale);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, LightColor.ToVector3() * 0.2f * Main.essScale);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = ModContent.Request<Texture2D>(Texture + "_Glow");
            var drawPosition = Item.position - Main.screenPosition + Item.Size * 0.5f;
            var origin = Item.Size * 0.5f;
            var offset = (float)Math.Cos(Main.GlobalTimeWrappedHourly);

            void DrawGlowmask(Vector2 pos, Color color)
            {
                spriteBatch.Draw(texture.Value, drawPosition + pos, null, color * Main.essScale, rotation, origin, scale, SpriteEffects.None, 0f);
            }

            DrawGlowmask(Vector2.One.RotatedBy(Main.GlobalTimeWrappedHourly) * offset, Color.White * 0.3f);
            DrawGlowmask(Vector2.One.RotatedBy(Main.GlobalTimeWrappedHourly + MathHelper.Pi) * offset, Color.White * 0.3f);
            DrawGlowmask(Vector2.Zero, Color.White);
        }

        public void DrawItemGlowmask(ref PlayerDrawSet drawInfo)
        {
            var texture = ModContent.Request<Texture2D>(Texture + "_Glow");
            var drawPlayer = drawInfo.drawPlayer;
            var heldItem = drawInfo.heldItem;

            var adjustedItemScale = drawPlayer.GetAdjustedItemScale(heldItem);
            var position = new Vector2((int)(drawInfo.ItemLocation.X - Main.screenPosition.X), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y));
            var sourceRect = new Rectangle?(new Rectangle(0, 0, texture.Width(), texture.Height()));

            var origin = new Vector2(drawPlayer.direction == -1 ? texture.Width() : 0, drawPlayer.gravDir == -1 ? 0 : texture.Height());
            var offset = (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 2f;

            void DrawGlowmask(ref PlayerDrawSet drawInfo, Vector2 offset, Color color)
            {
                var drawData = new DrawData(texture.Value, position + offset, sourceRect, color, drawPlayer.itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
                drawInfo.DrawDataCache.Add(drawData);
            }

            DrawGlowmask(ref drawInfo, Vector2.One.RotatedBy(Main.GlobalTimeWrappedHourly) * offset, Color.White * 0.3f);
            DrawGlowmask(ref drawInfo, Vector2.One.RotatedBy(Main.GlobalTimeWrappedHourly + MathHelper.Pi) * offset, Color.White * 0.3f);
            DrawGlowmask(ref drawInfo, Vector2.Zero, Color.White);
        }
    }

    public class StarburstStarProjectile : ModProjectile, IAfterUpdatingCameraPosition
    {
        public static readonly Color[] StarColors = new[]
        {
            new Color(0.95f, 0.65f, 0.03f, 0.4f),
            new Color(0.35f, 0.05f, 0.95f, 0.5f),
            new Color(0.2f, 0.4f, 0.8f, 0.5f),
            new Color(0.2f, 0.8f, 0.5f, 0.5f)
        };

        public enum AIState : int
        {
            Spawn,
            RotationAroundPlayer,
            Attack
        }

        public bool IsReadyToAttack { get => State == AIState.RotationAroundPlayer; }
        public int Timer { get; set; } = 0;
        public int Index { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float Progress { get => Projectile.ai[1]; set => Projectile.ai[1] = value; } // 0 -> 2 * Pi
        public float SpawnProgress { get => Projectile.timeLeft / 100f; set => Projectile.timeLeft = (int)(value * 100); }
        public Vector2? Target { get; private set; }
        public AIState State { get; set; } = AIState.Spawn;

        // ...

        public override string Texture => ModAssets.ProjectilesPath + nameof(StarburstStarProjectile);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starburst Star");

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.hide = true; // Not remove
            Projectile.timeLeft = 2; // Don't touch, used in SpawnProgress
        }

        public override bool PreAI()
        {
            var owner = Main.player[Projectile.owner];
            if (owner.HeldItem.ModItem is not StarburstScepter)
            {
                Index = -1;
                Projectile.Kill();
            }

            Projectile.timeLeft += 1; // So that it does not decrease
            return true;
        }

        public override void AI()
        {
            const float MAX_X = 70f;
            const float MAX_Y = 7f;

            Timer += 1;

            var time = Timer * 0.05f;
            var owner = Main.player[Projectile.owner];
            var localProgress = (time + (Index == 0 ? 0 : (Index / 3f * MathHelper.TwoPi))) % MathHelper.TwoPi;

            Progress = (localProgress + MathHelper.PiOver2 + MathHelper.Pi) % MathHelper.TwoPi;

            switch (State)
            {
                case AIState.Spawn:
                    {
                        Projectile.rotation = MathF.Sin(localProgress) * 0.85f;
                        Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY + new Vector2(MathF.Sin(localProgress) * MAX_X, MathF.Cos(localProgress) * MAX_Y * MathF.Sin(time)) * SpawnProgress;

                        SpawnProgress += 0.07f;
                        if (SpawnProgress >= 1)
                        {
                            SpawnProgress = 1f;
                            ChangeState(AIState.RotationAroundPlayer);
                        }
                    }
                    break;
                case AIState.RotationAroundPlayer:
                    {
                        Projectile.rotation = MathF.Sin(localProgress) * 0.85f;
                        Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY + new Vector2((float)Math.Sin(localProgress) * MAX_X, (float)Math.Cos(localProgress) * MAX_Y * MathF.Sin(time));
                    }
                    break;
                case AIState.Attack:
                    {
                        if (Target == null || Vector2.Distance(Projectile.Center, Target.Value) < 8)
                        {
                            OnAttackHit();
                            return;
                        }

                        Progress = 0;
                        Projectile.rotation += 0.2f;
                        Projectile.velocity = Vector2.Normalize(Target.Value - Projectile.Center) * (owner?.HeldItem.shootSpeed ?? 15f);

                        if (Timer % 3 == 0 || Timer % 5 == 0)
                        {
                            var position = Projectile.Center + new Vector2(Main.rand.NextFloat(-20, 20), 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                            var color = StarColors[Main.rand.Next(1, StarColors.Length)];
                            Particle.NewParticle(ParticleSystem.ParticleType<AsteriteParticle>(), position, Vector2.Zero, color, 0, Main.rand.NextFloat(MathHelper.TwoPi));
                        }
                    }
                    break;
                default:
                    {
                        ChangeState(AIState.RotationAroundPlayer);
                    }
                    break;
            }

            Lighting.AddLight(Projectile.Center, StarburstScepter.LightColor.ToVector3() * 0.3f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (State == AIState.Attack)
            {
                OnAttackHit();
            }
        }

        public override bool? CanHitNPC(NPC target) => State == AIState.Attack && (target.CanBeChasedBy() || target.type == NPCID.TargetDummy);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == AIState.Attack)
            {
                OnAttackHit();
            }

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Progress < MathHelper.Pi)
            {
                behindProjectiles.Add(index);
                return;
            }

            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var localProgress = 0.5f - MathF.Sin(Progress) / 2f;
            var drawPosition = Projectile.Center - Main.screenPosition;
            var colorNum = MathHelper.Lerp(0.8f, 1f, localProgress) * SpawnProgress;
            var color = new Color(colorNum, colorNum, colorNum, 0.8f);
            var scale = MathHelper.Lerp(0.65f, 1f, localProgress) * Projectile.scale * SpawnProgress;
            var texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, color, Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);

            return false;
        }

        void IAfterUpdatingCameraPosition.PostUpdateCameraPosition()
        {
            Asset<Texture2D> texture;
            Color color;
            Vector2 scale = Projectile.scale * Vector2.One;

            if (State == AIState.Attack)
            {
                texture = ModAssets.GetExtraTexture(1);
                for (int k = 1; k < Projectile.oldPos.Length; k++)
                {
                    var position = Projectile.oldPos[k] + Projectile.Size * 0.5f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                    var num = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                    var trailColor = Color.Lerp(StarColors[0], StarColors[1], num) * num;
                    AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, position, null, trailColor, Projectile.oldRot[k], texture.Size() * 0.5f, scale * num, SpriteEffects.None, true));
                }
            }

            var drawPosition = Projectile.Center - Main.screenPosition;
            var localProgress = 0.5f - MathF.Sin(Progress) / 2f;
            texture = ModContent.Request<Texture2D>(Texture + "_Effect");
            color = StarColors[0] * MathHelper.Lerp(0.8f, 1f, localProgress) * SpawnProgress;
            AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, drawPosition, null, color, Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, true));
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Target ?? Vector2.Zero);
            writer.Write((int)State);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            var vector = reader.ReadVector2();
            Target = vector == Vector2.Zero ? null : vector;
            State = (AIState)reader.ReadInt32();
        }

        public void ChangeState(AIState state)
        {
            State = state;
            Projectile.netUpdate = true;
        }

        public void SetTargetPosition(Vector2 value)
        {
            if (!IsReadyToAttack) return;

            Projectile.tileCollide = true;
            Target = value;
            ChangeState(AIState.Attack);
        }

        public void OnAttackHit()
        {
            Projectile.tileCollide = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.oldVelocity = Vector2.Zero;

            SpawnProgress = 0.02f;
            ChangeState(AIState.Spawn);
            Target = null;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StarburstStarHitProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            return;
        }
    }

    public class StarburstStarHitProjectile : ModProjectile, IAfterUpdatingCameraPosition
    {
        public override string Texture => ModAssets.InvisiblePath;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starburst Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 10;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                var vector = new Vector2(Main.rand.NextFloat(-15, 15), 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                var position = Projectile.Center + vector;
                var color = StarburstStarProjectile.StarColors[Main.rand.Next(1, StarburstStarProjectile.StarColors.Length)];
                Particle.NewParticle(ParticleSystem.ParticleType<AsteriteParticle>(), position, vector * 0.2f, color, 0, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(1.2f, 3f));
            }
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        void IAfterUpdatingCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = Projectile.Center - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(0);
            var origin = texture.Size() * 0.5f + new Vector2(0, 8);
            var progress = 1 - Math.Abs(1 - Projectile.timeLeft / 10f);
            var color = StarburstStarProjectile.StarColors[1];
            var scale = Projectile.scale * Vector2.One * progress;
            color.A = 255;

            AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, drawPosition, null, color * 0.7f, Projectile.rotation, origin, scale * 0.5f, SpriteEffects.None, false));
            AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, drawPosition, null, color * 0.5f, -Projectile.rotation, origin, scale * 0.4f, SpriteEffects.None, false));
        }
    }
}