using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.Items.Armor.ScratchSet
{
	[AutoloadEquip(EquipType.Legs)]
	public class ScratchBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scratch's Uni-Plated Greaves");
			Tooltip.SetDefault("'Great for impersonating modded devs!'" + "\n'To truly understand someone, you must walk a mile in their shoes. \nFor me, you must walk much further.'");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 18;
			Item.sellPrice(0, 5, 0, 0);
			item.rare = ItemRarityID.Cyan;
			item.vanity = true;
		}
	}
}