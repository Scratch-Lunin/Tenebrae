using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class InpuratusMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Inpuratus Mask");
		}
		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 22;
			item.rare = ItemRarityID.Blue;
			item.vanity = true;
		}

		public override bool DrawHead()
		{
			return false;
		}
	}
}