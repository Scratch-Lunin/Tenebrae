namespace Tenebrae.Items.Armor.ScratchSet
{
    [AutoloadEquip(EquipType.Head)]
    public class ScratchHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scratch's Tri-Pointed Helm");
            Tooltip.SetDefault("'Great for impersonating modded devs!'"
                + "\n'Look into my eyes, and you shall see endless realms of possibilities.'");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 22;
            Item.sellPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.vanity = true;
        }
    }
}