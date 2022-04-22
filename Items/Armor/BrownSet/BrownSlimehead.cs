namespace Tenebrae.Items.Armor.BrownSet
{
    [AutoloadEquip(EquipType.Head)]
    public class BrownSlimehead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brown's Stellar Slimehead");
            Tooltip.SetDefault("'Great for impersonating modded devs!'"
                + "\n'Seems like you've got yourself in a sticky situation'"
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
            recipe.AddIngredient(null, "BrownHelmet");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}