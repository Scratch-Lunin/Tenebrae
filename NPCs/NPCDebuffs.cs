using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Tenebrae.NPCs
{
    public class NPCDebuffs : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool warriordebuff = false;

        public override void ResetEffects(NPC npc)
        {
            warriordebuff = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (warriordebuff)
            {
                if (npc.life < 10)
                {
                    if (npc.lifeRegen > 0)
                    {
                        npc.lifeRegen = 0;
                    }
                    npc.lifeRegen -= npc.life * 10;
                }
            }   
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (warriordebuff)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.HealingPlus, 0f, 0f, 100, new Color(255, 0, 0), 1f);
                    Main.dust[dust].noGravity = true;

                    dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.FlameBurst, 0f, 0f, 100, new Color(255, 0, 0), .8f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}
