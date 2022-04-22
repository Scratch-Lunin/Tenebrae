using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.Items.Armor.BrownSet
{
	[AutoloadEquip(EquipType.Head)]
	public class BrownHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brown's Slime-Filled Helmet");
			Tooltip.SetDefault("'Great for impersonating modded devs!'"
				+ "\n'The slime's 100% breathable! Trust!'"
				+ "\nCan be crafted into a different helmet");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			Item.sellPrice(0, 5, 0, 0);
			item.rare = ItemRarityID.Cyan;
			item.vanity = true;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "BrownSlimehead");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}