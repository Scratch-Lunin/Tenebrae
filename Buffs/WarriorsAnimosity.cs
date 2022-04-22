using Tenebrae.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace Tenebrae.Buffs
{
    public class WarriorsAnimosity : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Warrior's Animosity");
			Description.SetDefault("Your lifeforce drains, your vessel fades to ash. The Warrior has claimed your life.");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.life -= (int)(npc.life * 0.1f);
			npc.defense = 0;
			npc.GetGlobalNPC<NPCDebuffs>().warriordebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<TenebraePlayer>().warriordebuff = true;
			player.statLife -= (int)(player.statLife * 0.1f);
			player.statDefense = 0;
		}
	}
}