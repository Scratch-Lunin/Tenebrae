using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Tenebrae.Items.Placable;
using Tenebrae.Items.Armor;

namespace Tenebrae
{
    public partial class Tenebrae : Mod
    {
        public override void PostSetupContent()
        {
            Mod fargos = ModLoader.GetMod("Fargowiltas");
            if (fargos != null)
            {
                /* AddSummon, order or value in terms of vanilla bosses, your mod internal name, summon  
                item internal name, inline method for retrieving downed value, price to sell for in copper */
                fargos.Call("AddSummon", 10.5f, "Tenebrae", "InpuratusSummonFargo", (Func<bool>)(() => TenebraeWorld.downedInpuratus), 480000);

            }

            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            {
                bossChecklist.Call(
                    "AddBoss",
                    10.5f,
                    ModContent.NPCType<NPCs.Inpuratus.Inpuratus>(),
                    this, // Mod
                    "Inpuratus",
                    (Func<bool>)(() => TenebraeWorld.downedInpuratus),
                    ModContent.ItemType<Items.Summoning.InpuratusSummon>(),
                    new List<int> { ModContent.ItemType<InpuratusTrophy>(), ModContent.ItemType<InpuratusMask>(), 
                    /*},
                    new List<int> { ModContent.ItemType<VileGlaive>(), ModContent.ItemType<CursedCarbine>(), ModContent.ItemType<CursefernoBurst>(), ModContent.ItemType<VileAmulet>(),*/
                    ItemID.CursedFlame, ItemID.RottenChunk},
                    "Use a [i:" + ItemType("InpuratusSummon") + "] in the Underground Corruption.",
                    "Inpuratus retreats to the depths of the Corruption...",
                    "Tenebrae/BossChecklistTextures/BossInpuratus",
                    //"Tenebrae/NPCs/Inpuratus/Inpuratus_Head_Boss",
                    (Func<bool>)(() => !WorldGen.crimson));
                // Additional bosses here
            }
        }
    }
}