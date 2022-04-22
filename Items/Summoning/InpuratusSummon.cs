using Tenebrae.NPCs.Inpuratus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Tenebrae.Items.Summoning
{
    public class InpuratusSummon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vile Crystal Amalgam");
			Tooltip.SetDefault("Filled with corrupted energy"+"\nSummons Inpuratus when used in the Underground Corruption");
			ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 32;
			item.maxStack = 20;
			item.rare = ItemRarityID.LightPurple;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item44;
			item.consumable = true;
		}

		public override bool CanUseItem(Player player)
		{
			// "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
			return Main.hardMode && player.ZoneCorrupt && player.ZoneRockLayerHeight && !NPC.AnyNPCs(ModContent.NPCType<Inpuratus>());
		}

		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Inpuratus>());
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}
	}
}