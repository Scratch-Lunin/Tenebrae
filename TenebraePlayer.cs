using Tenebrae.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;

namespace Tenebrae
{
    public class TenebraePlayer : ModPlayer
    {
        public int InpuratusDeathShake;
        public int DashShakeTimer;

        public override void ModifyScreenPosition()
        {
            if (TenebraeWorld.InpuratusDies == true)
            {
                InpuratusDeathShake++;
                float intensity = 10f;
                if (InpuratusDeathShake >= 1)
                {
                    Main.screenPosition += new Vector2(Main.rand.NextFloat(intensity), Main.rand.NextFloat(intensity));
                    Main.screenPosition -= new Vector2(Main.rand.NextFloat(intensity), Main.rand.NextFloat(intensity));
                    intensity *= 0.9f;
                    if (InpuratusDeathShake == 30)
                    {
                        TenebraeWorld.InpuratusDies = false;
                        InpuratusDeathShake = 0;
                    }
                }
            }

            if (TenebraeWorld.DashShake == true)
            {
                DashShakeTimer++;
                float intensity = 3f;
                if (DashShakeTimer >= 1)
                {
                    Main.screenPosition += new Vector2(Main.rand.NextFloat(intensity), Main.rand.NextFloat(intensity));
                    Main.screenPosition -= new Vector2(Main.rand.NextFloat(intensity), Main.rand.NextFloat(intensity));
                    intensity *= 0.9f;
                    if (DashShakeTimer == 15)
                    {
                        TenebraeWorld.DashShake = false;
                        DashShakeTimer = 0;
                    }
                }
            }
        }

        public bool warriordebuff;

        public override void ResetEffects()
        {
            warriordebuff = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (warriordebuff)
            {
                if (player.statLife < 10)
                {
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                    player.lifeRegen -= player.statLife * 10;
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8 && player.HasBuff(ModContent.BuffType<WarriorsAnimosity>()))
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s soul was claimed by the Warrior");
            }
            return true;
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (warriordebuff)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.HealingPlus, 0f, 0f, 100, new Color(255, 0, 0), 1f);
                    Main.dust[dust].noGravity = true;
                    dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.FlameBurst, 0f, 0f, 100, new Color(255, 0, 0), .8f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}