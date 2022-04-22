using Tenebrae.Items.Armor;
using Tenebrae.NPCs.Inpuratus;

namespace Tenebrae.Items.Misc
{
    public class InpuratusBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 44;
            item.height = 42;
            item.rare = ItemRarityID.Pink;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            player.TryGettingDevArmor();
            player.QuickSpawnItem(ItemID.CursedFlame, 20 + Main.rand.Next(10));
            player.QuickSpawnItem(ItemID.RottenChunk, 50 + Main.rand.Next(10));

            if (Main.rand.NextBool(7))
            {
                player.QuickSpawnItem(ModContent.ItemType<InpuratusMask>());
            }

            /*if (Main.rand.NextBool(3))
			{
				player.QuickSpawnItem(ModContent.ItemType<VileAmulet>());
			}

			switch (Main.rand.Next(3))
			{
				case 0:
					player.QuickSpawnItem(ModContent.ItemType<CursefernoBurst>());
					break;
				case 1:
					player.QuickSpawnItem(ModContent.ItemType<VileGlaive>());
					break;
				case 2:
					player.QuickSpawnItem(ModContent.ItemType<CursedCarbine>());
					break;
			}*/

            player.QuickSpawnItem(ItemID.WormScarf);
        }

        public override int BossBagNPC => ModContent.NPCType<Inpuratus>();
    }
}