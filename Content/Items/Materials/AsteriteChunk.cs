using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Content.Items.Materials
{
    public class AsteriteChunk : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + Name;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;

            DisplayName.SetDefault("Asterite Chunk");
            Tooltip.SetDefault("'Glimmers with heavenly energy'");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;

            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 999;
            Item.consumable = true;
            //Item.createTile = mod.TileType("AsteriteChunk");
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }
    }
}
