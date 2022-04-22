﻿using Tenebrae.Tiles;

namespace Tenebrae.Items.Placable
{
    public class InpuratusTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inpuratus Trophy");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 50000;
            item.rare = ItemRarityID.Blue;
            item.createTile = TileType<BossTrophy>();
            item.placeStyle = 0;
        }
    }
}