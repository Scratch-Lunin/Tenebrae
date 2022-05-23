using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tenebrae.Common.PlayerDrawLayers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Content.Items.Materials
{
    public class AsteriteIngot : ModItem
    {
        public static readonly Color LightColor = new(244, 179, 12);

        // ...

        public override string Texture => ModAssets.ItemsPath + Name;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;

            DisplayName.SetDefault("Asterite Ingot");
            Tooltip.SetDefault("'Forged with the essence of starlight'");

            // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable
            ItemID.Sets.SortingPriorityMaterials[Type] = 65;

            HeldItemLayer.RegisterItemGlowmask(Type, DrawItemGlowmask);
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;

            Item.maxStack = 99;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Blue;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            //Item.createTile = mod.TileType("AsteriteIngot");
            Item.placeStyle = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AsteriteChunk>(3).AddIngredient(ItemID.FallenStar).AddTile(TileID.SkyMill).Register();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Lighting.AddLight(player.itemLocation, LightColor.ToVector3() * 0.3f * Main.essScale);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, LightColor.ToVector3() * 0.3f * Main.essScale);
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
}
