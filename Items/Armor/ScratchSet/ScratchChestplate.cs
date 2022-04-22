namespace Tenebrae.Items.Armor.ScratchSet
{
    [AutoloadEquip(EquipType.Body)]
    public class ScratchChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scratch's Ion Chestguard");
            Tooltip.SetDefault("'Great for impersonating modded devs!'" + "\n'Stand guard, I've given myself a reason to be feared.'");
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