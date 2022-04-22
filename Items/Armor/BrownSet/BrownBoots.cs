using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.Items.Armor.BrownSet
{
	[AutoloadEquip(EquipType.Legs)]
	public class BrownBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brown's Astro Boots");
			Tooltip.SetDefault("'Great for impersonating modded devs!'" + "\n'They fit snugly no matter the size!'");
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