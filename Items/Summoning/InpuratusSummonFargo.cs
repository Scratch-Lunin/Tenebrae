using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Tenebrae.NPCs.Inpuratus;

namespace Tenebrae.Items.Summoning
{
    public class InpuratusSummonFargo : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Amalgam of Vile Crystals");
			Tooltip.SetDefault("Filled with corrupted energy"+"\nSummons Inpuratus");
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
			return Main.hardMode && !NPC.AnyNPCs(ModContent.NPCType<Inpuratus>());
		}

		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Inpuratus>());
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}
	}
}