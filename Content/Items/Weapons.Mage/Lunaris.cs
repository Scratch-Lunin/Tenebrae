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
using Tenebrae.Content.Particles;
using Tenebrae.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Content.Items.Weapons.Mage
{
    public class Lunaris : ModItem
    {
        public static readonly Color LightColor = new(188, 101, 191);
        public static readonly Vector2 HeldItemRotateVector = new(1, -2);
        public static readonly Vector2 HeldItemOffsetVector = new(4, 24);

        // ...

        public override string Texture => ModAssets.ItemsPath + nameof(Lunaris);

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            DisplayName.SetDefault("Lunaris");
            Tooltip.SetDefault("'There could be your ad here'");

            HeldItemLayer.RegisterItemGlowmask(Type, DrawItemGlowmask);
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 62;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 50;
            Item.crit = 0;
            Item.knockBack = 4f;
            Item.mana = 10;
            Item.rare = ItemRarityID.Quest;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Thrust;
            //Item.UseSound = SoundID.Item45;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<LunarisProjectile>();
            Item.shootSpeed = 16f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<LunarisClockProjectile>();
                position = Main.MouseWorld;
                velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/MagicAppear"), position);
                return;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<LunarisClockProjectile>()] > 0)
            {
                var proj = Main.projectile.First(i => i.ModProjectile is LunarisClockProjectile && i.active && i.owner == player.whoAmI);
                if (proj != null)
                {
                    position = proj.Center;
                    velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();
                    SoundEngine.PlaySound(SoundID.Item43, position);
                    return;
                }
            }

            position += Vector2.Normalize(new Vector2(HeldItemRotateVector.X * player.direction, HeldItemRotateVector.Y * player.gravDir)) * 50;
            velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();
            SoundEngine.PlaySound(SoundID.Item43, position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                var hasClock = player.ownedProjectileCounts[ModContent.ProjectileType<LunarisClockProjectile>()] > 0;
                var proj = Main.projectile[Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, velocity.Length())];

                if (hasClock && proj.ModProjectile is LunarisProjectile modProj)
                {
                    modProj.ChangeState(LunarisProjectile.AIState.AutoAiming);
                }

                return false;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                var mousePos = Main.MouseWorld.ToTileCoordinates();
                return !WorldGen.SolidTile(Main.tile[mousePos.X, mousePos.Y]) && player.ownedProjectileCounts[ModContent.ProjectileType<LunarisClockProjectile>()] == 0;
            }
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation = (player.MountedCenter + new Vector2(player.direction * HeldItemOffsetVector.X, player.gravDir * HeldItemOffsetVector.Y)).Floor();
            player.itemRotation = player.direction * player.gravDir * new Vector2(-HeldItemRotateVector.Y, -HeldItemRotateVector.X).ToRotation();

            Lighting.AddLight(player.itemLocation, LightColor.ToVector3() * 0.2f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, LightColor.ToVector3() * 0.2f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = ModContent.Request<Texture2D>(Texture + "_Glow");
            var drawPosition = Item.position - Main.screenPosition + Item.Size * 0.5f;
            var origin = Item.Size * 0.5f;

            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White * 0.9f, rotation, origin, scale, SpriteEffects.None, 0f);
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

            var drawData = new DrawData(texture.Value, position, sourceRect, Color.White * 0.9f, drawPlayer.itemRotation, origin, adjustedItemScale, drawInfo.itemEffect, 0);
            drawInfo.DrawDataCache.Add(drawData);
        }
    }

    public class LunarisProjectile : ModProjectile, IAfterUpdatingCameraPosition
    {
        public enum AIState
        {
            WithoutAutoAiming,
            Expectation, // Not used
            AutoAiming
        }

        public AIState State { get => (AIState)Projectile.ai[0]; set => Projectile.ai[0] = (float)value; }
        public float InitSpeed { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int TargetIndex { get; set; } = -1;

        public const float TimerMaxValue = 20;

        public override string Texture => ModAssets.ProjectilesPath + nameof(LunarisProjectile);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunaris");

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;

            Projectile.timeLeft = 60 * 15;
        }

        public override void AI()
        {
            Timer = Math.Min(++Timer, (int)TimerMaxValue);

            switch (State)
            {
                case AIState.WithoutAutoAiming:
                    {
                        Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 16);
                    }
                    break;
                case AIState.AutoAiming:
                    {
                        if (TargetIndex != -1)
                        {
                            var npc = Main.npc[TargetIndex];
                            if (npc == null || !npc.active)
                            {
                                TargetIndex = -1;
                                Projectile.netUpdate = true;

                            }
                            else
                            {
                                Projectile.MoveTowards(npc.Center, InitSpeed * 2f, 16f);
                            }
                        }
                        else
                        {
                            var target = NPCUtils.NearestNPC(Projectile.Center, 16 * 25, i => i.CanBeChasedBy(Projectile, false) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, i.position, i.width, i.height));
                            var npc = target.npc;

                            if (npc != null)
                            {
                                TargetIndex = npc.whoAmI;
                                Projectile.netUpdate = true;
                                break;
                            }

                            Projectile.velocity *= 0.96f;
                            Projectile.timeLeft--;
                        }
                    }
                    break;
                default:
                    Projectile.Kill();
                    break;
            }

            Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.2f;
            Lighting.AddLight(Projectile.Center, Lunaris.LightColor.ToVector3() * 0.3f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(2, 4)).RotatedBy(Projectile.rotation + MathHelper.TwoPi / 3f * i);
                Particle.NewParticle<WaveHitParticle>(Projectile.Center - velocity * 10f, velocity, Lunaris.LightColor, rotation: velocity.ToRotation(), scale: 0.35f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate -= 1;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            Projectile.velocity *= 0.75f;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            OnHit();
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (State == AIState.AutoAiming)
            {
                Projectile.Kill();
                return;
            }

            OnHit();
        }

        public void OnHit()
        {
            if (Projectile.penetrate > 1) return;

            ChangeState(AIState.AutoAiming);
        }

        public void ChangeState(AIState state)
        {
            if (state == AIState.AutoAiming)
            {
                Projectile.timeLeft = 60 * 2;
                Projectile.penetrate = -1;
            }

            State = state;
            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TargetIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TargetIndex = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var colorMult = Timer / TimerMaxValue;
            var drawPosition = Projectile.Center - Main.screenPosition;
            var texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White * colorMult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        void IAfterUpdatingCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = Projectile.Center - Main.screenPosition;
            var scale = Projectile.scale * Vector2.One;
            var progress = 0.5f + MathF.Cos(Main.GlobalTimeWrappedHourly) * 0.5f;
            var colorMult = Timer / TimerMaxValue;
            var color = Lunaris.LightColor * MathHelper.Lerp(0.8f, 0.4f, progress) * colorMult;
            var texture = ModAssets.GetExtraTexture(3);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var position = Projectile.oldPos[k] + Projectile.Size * 0.5f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                var num = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, position, null, color * num, Projectile.oldRot[k], texture.Size() * 0.5f, scale * num * 0.15f, SpriteEffects.None, true));
            }

            texture = ModContent.Request<Texture2D>(Texture + "_Effect");
            for (int i = 0; i < 4; i++)
            {
                var position = drawPosition + new Vector2(2 + progress * 2, 0).RotatedBy(i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly);
                AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, position, null, color, Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, true));
            }
        }
    }

    public class LunarisClockProjectile : ModProjectile, IAfterUpdatingCameraPosition
    {
        private List<RomanNumber> romanNumbers;

        public ref float PulsationProgress => ref Projectile.localAI[0];
        public ref float DeathProgress => ref Projectile.localAI[1];

        // ...

        public override string Texture => ModAssets.ProjectilesPath + nameof(LunarisClockProjectile);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunaris");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 60 * 12 + 5;
        }

        public override void AI()
        {
            // It's better to move this to OnSpawn when it's available
            if (romanNumbers == null)
            {
                romanNumbers = new List<RomanNumber>();
                for (int i = 1; i <= 12; i++)
                {
                    var texture = ModContent.Request<Texture2D>(ModAssets.ProjectilesPath + nameof(LunarisClockProjectile) + "_Numbers", AssetRequestMode.ImmediateLoad);
                    romanNumbers.Add(new RomanNumber(i, texture));
                }
            }

            foreach (var number in romanNumbers)
            {
                number.Update();
            }

            if (Projectile.timeLeft % 60 == 0)
            {
                var index = Projectile.timeLeft / 60 - 1;
                if (index < romanNumbers.Count && index >= 0)
                {
                    romanNumbers[index].Appear();
                    PulsationProgress = 1.3f;
                    var sound = SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/Clock"), Projectile.Center);
                    sound.Volume *= 0.65f;
                }
            }

            const float deathTime = 10;
            if (Projectile.timeLeft <= deathTime)
            {
                if (Projectile.timeLeft == deathTime)
                {
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/Bell"), Projectile.Center);
                }

                DeathProgress = Math.Min(DeathProgress + 1 / deathTime, 1);
            }

            PulsationProgress = Math.Max(PulsationProgress - 0.05f, 0f);
            Projectile.rotation = MathF.Sin(Projectile.timeLeft / 60f * MathHelper.Pi) * (MathHelper.PiOver4 * 0.33f);
            Projectile.scale *= (1 - DeathProgress);

            Lighting.AddLight(Projectile.Center, Lunaris.LightColor.ToVector3() * 0.4f);
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            const float offset = 0.03f; // Slight offset of the arrows

            var drawPosition = GetDrawPosition();
            var texture = ModContent.Request<Texture2D>(Texture + "_Hour");
            var rotation = Projectile.timeLeft / (float)(12 * 60) * MathHelper.TwoPi - MathHelper.PiOver2 + offset;
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White, rotation, new Vector2(0, texture.Height() * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(Texture + "_Minute");
            rotation = Projectile.timeLeft / 60f * MathHelper.TwoPi - MathHelper.PiOver2 + offset;
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White, rotation, new Vector2(0, texture.Height() * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(Texture + "_Glow");
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            foreach (var number in romanNumbers)
            {
                number.Draw(drawPosition, Projectile.scale);
            }

            return false;
        }

        void IAfterUpdatingCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = GetDrawPosition();
            var color = new Color(188, 101, 191);
            var scale = Projectile.scale * Vector2.One;
            var texture = ModContent.Request<Texture2D>(Texture + "_Effect3");

            for (int i = 0; i < 4; i++)
            {
                var position = drawPosition + new Vector2(2 * PulsationProgress, 0).RotatedBy(i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly);
                AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, position, null, Lunaris.LightColor * 0.5f * PulsationProgress, Projectile.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, true));
            }

            texture = ModContent.Request<Texture2D>(Texture + "_Effect2");
            AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, drawPosition, null, Color.White * 0.4f, Main.GlobalTimeWrappedHourly * 4f, texture.Size() * 0.5f, scale * 0.3f, SpriteEffects.None, true));

            texture = ModContent.Request<Texture2D>(Texture + "_Effect");
            AdditiveDrawSystem.AddToDataCache(new AdditiveDrawData(texture.Value, drawPosition, null, color * 0.65f, Projectile.rotation, texture.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, false));
        }

        public Vector2 GetDrawPosition() => Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(Projectile.timeLeft * 0.015f), MathF.Cos(Projectile.timeLeft * 0.065f)) * 1.5f;

        // ...

        private class RomanNumber
        {
            public readonly int number;
            public float alpha;
            public float scale;

            private Rectangle rectangle;
            private Vector2 origin;
            private Asset<Texture2D> texture;

            public RomanNumber(int number, Asset<Texture2D> texture)
            {
                this.number = number;
                this.texture = texture;

                var height = texture.Height() / 12;

                rectangle = new Rectangle(0, (number - 1) * height, texture.Width(), height);
                origin = new Vector2(rectangle.Width, rectangle.Height) * 0.5f;
                alpha = 0f;
            }

            public void Appear()
            {
                alpha = 2f;
                scale = 0f;
            }

            public void Update()
            {
                if (alpha <= 0f) return;

                alpha = Math.Max(alpha - 0.01f, 0f);
                scale = Math.Min(scale + 0.1f, 1f);
            }

            public void Draw(Vector2 position, float scale)
            {
                if (alpha <= 0f) return;

                Main.EntitySpriteDraw(texture.Value, position + new Vector2(0, -64).RotatedBy(number * MathHelper.Pi / 6), rectangle, Color.White * alpha, 0f, origin, scale * this.scale, SpriteEffects.None, 0);
            }
        }
    }
}