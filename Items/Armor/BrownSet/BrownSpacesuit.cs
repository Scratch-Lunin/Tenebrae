using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.Items.Armor.BrownSet
{
	[AutoloadEquip(EquipType.Body)]
	public class BrownSpacesuit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brown's Spacesuit");
			Tooltip.SetDefault("'Great for impersonating modded devs!'" + "\n'Gotta have some sort of chest-protector if ya' wanna protect your chest'");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 20;
			Item.sellPrice(0, 5, 0, 0);
			item.rare = ItemRarityID.Cyan;
			item.vanity = true;
		}
	}
}