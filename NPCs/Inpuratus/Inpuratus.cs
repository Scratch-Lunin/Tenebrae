using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;
using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace TenebraeMod.NPCs.Inpuratus
{
    public class Inpuratus : ModNPC
    {
        float cen = 0f;
        Vector2[] FootPositions = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        Vector2[] FootPositionAdds = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

        #region Wrappers and Enums
        public AttackState CurrentAttackState
        {
            get => (AttackState)(int)npc.ai[0];
            set
            {
                npc.ai[0] = (int)value;
                npc.netUpdate = true;
            }
        }
        public enum AttackState
        {
            JustMoving = 0
        }
        public enum MovementState
        {
            Leggy = 0,
            NotLeggy = 1
        }
        #endregion 

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inpuratus");
            Main.npcFrameCount[npc.type] = 20;
            NPCID.Sets.TrailCacheLength[npc.type] = 12;
            NPCID.Sets.TrailingMode[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            for (int i = 0; i < 4; i++)
            {
                FootPositions[i] = npc.Center;
            }

            npc.aiStyle = -1;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.width = 120;
            npc.height = 210;
            npc.life = 28000;
            npc.lifeMax = 28000;
            npc.knockBackResist = 0f;
            npc.defense = 11;
            npc.scale = 1f;
            npc.damage = 23;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            npc.TargetClosest();

            Player player = Main.player[npc.target];

            float vel = (float)Math.Sqrt(Math.Pow(npc.velocity.X, 2) + Math.Pow(npc.velocity.Y, 2));

            float rot = npc.AngleFrom(player.Center) + MathHelper.ToRadians(90f);

            Vector2 targetPosition;


            if (CurrentAttackState == AttackState.JustMoving)
            {
                targetPosition = FindCaveMid(player.Center);
                float dist = npc.Distance(targetPosition);
                npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(targetPosition) * dist * 0.005f, 0.2f);
                if (vel > 10) npc.velocity *= 0.95f;
                if (vel > 15) npc.velocity *= 0.95f;
            }
            npc.rotation = rot;

            //if (MovementState.Leggy)
            for (int i = 0; i < 4; i++)
            {
                Vector2 addon = new Vector2(-1, -1).RotatedBy(MathHelper.ToRadians(i * 90));
                //addon.X /= 6f;
                //addon.Y /= 2f;

                float sign = Math.Sign(addon.X);
                addon.X = 0;

                float[] scanarray = new float[3];
                float distance = 800;
                Collision.LaserScan(FindCenterLeg(i) + new Vector2(sign * 200, 0), addon, 0, distance, scanarray);

                distance = 0;
                foreach (float scan in scanarray)
                {
                    distance += (scan / scanarray.Length);
                }

                Vector2 dest = new Vector2((float)Math.Floor((FindCenterLeg(i) + new Vector2(sign * 200, 0) + (addon * distance)).X / 16) * 16, (float)Math.Floor((FindCenterLeg(i) + new Vector2(sign * 200, 0) + (addon * distance)).Y / 16) * 16);

                float posneg = -1;
                if (i < 2) posneg = 1;

                FootPositions[i] = Vector2.Lerp(FootPositions[i], dest, 0.22f);
                FootPositions[i].Y += Math.Abs((dest - FootPositions[i]).X) / 10 * posneg;
            }

            /*npc.ai[2]++;
            {
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText("");
                Main.NewText(FootPositions[2].ToString());
                Main.NewText(FootPositions[3].ToString());
                Main.NewText("");
                Main.NewText(npc.Center.ToString());
            }

            for (int i = 0; i < 4; i++)
            {
                FootPositions[i] += FootPositionAdds[i];
            }*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D foot = mod.GetTexture("NPCs/Inpuratus/InpuratusFoot");
            Texture2D footglow = mod.GetTexture("NPCs/Inpuratus/InpuratusFootGlow");
            Texture2D joint = mod.GetTexture("NPCs/Inpuratus/InpuratusJoint");
            Texture2D leg = mod.GetTexture("NPCs/Inpuratus/InpuratusLeg");


            Vector2[,] jointPositions = new Vector2[FootPositions.Length, 2];
            for (int i = 0; i < 4; i++)
            {
                Vector2 destination = FootPositions[i];
                jointPositions[i, 0] = Vector2.Lerp(npc.Center, destination, 0.3f);
                jointPositions[i, 1] = Vector2.Lerp(destination, npc.Center, 0.2f);

                float posneg = -1;
                if (i < 2) posneg = 1;

                jointPositions[i, 1].Y += posneg * 55;
            }


            for (int i = 0; i < FootPositions.Length; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 lastPos;
                    if (j == 0) lastPos = npc.Center;
                    else lastPos = jointPositions[i, j - 1];
                    spriteBatch.Draw(leg, jointPositions[i, j] - Main.screenPosition, new Rectangle(0, 0, leg.Width, leg.Height), Lighting.GetColor((int)jointPositions[i, j].X / 16, (int)jointPositions[i, j].Y / 16), npc.AngleTo(npc.Center + (lastPos - jointPositions[i, j])) - MathHelper.ToRadians(90f), new Vector2(leg.Width / 2, 0), new Vector2(1f, 1f / leg.Height * Vector2.Distance(jointPositions[i, j], lastPos)), SpriteEffects.None, 0f);
                }
                Vector2 lastPos2 = jointPositions[i, 1];
                Vector2 endPos = FootPositions[i];
                spriteBatch.Draw(leg, endPos - Main.screenPosition, new Rectangle(0, 0, leg.Width, leg.Height), Lighting.GetColor((int)endPos.X / 16, (int)endPos.Y / 16), npc.AngleTo(npc.Center + (lastPos2 - endPos)) - MathHelper.ToRadians(90f), new Vector2(leg.Width / 2, 0), new Vector2(1f, 1f / (float)leg.Height * Vector2.Distance(endPos, lastPos2)), SpriteEffects.None, 0f);
            }

            for (int i = 0; i < FootPositions.Length; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    spriteBatch.Draw(joint, jointPositions[i, j] - Main.screenPosition, new Rectangle(0, 0, joint.Width, joint.Height), Lighting.GetColor((int)jointPositions[i, j].X / 16, (int)jointPositions[i, j].Y / 16), 0f, new Vector2(joint.Width / 2, joint.Height / 2), 1f, SpriteEffects.None, 0f);
                }
            }

            for (int i = 0; i < FootPositions.Length; i++)
            {
                spriteBatch.Draw(foot, FootPositions[i] - Main.screenPosition, new Rectangle(0, 0, foot.Width, foot.Height), Lighting.GetColor((int)FootPositions[i].X / 16, (int)FootPositions[i].Y / 16), 0f, new Vector2(foot.Width / 2, (foot.Height / 2) + 6), 1f, i < 2 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                spriteBatch.Draw(footglow, FootPositions[i] - Main.screenPosition, new Rectangle(0, 0, foot.Width, foot.Height), Color.White, 0f, new Vector2(foot.Width / 2, (foot.Height / 2) + 6), 1f, i < 2 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            // ensure that there are no significant gaps in the trail
            for (int i = 1; i < npc.oldPos.Length; i++)
            {
                npc.oldPos[i] = npc.oldPos[i - 1] + (npc.oldPos[i] - npc.oldPos[i - 1]).SafeNormalize(Vector2.Zero) * MathHelper.Min(Vector2.Distance(npc.oldPos[i - 1], npc.oldPos[i]), 2f);
            }

            /*if (enraged)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

                Texture2D tex = Main.npcTexture[npc.type];

                spriteBatch.Draw(tex, npc.oldPos[1] - Main.screenPosition + new Vector2(npc.width / 2, npc.height / 2), npc.frame, Color.GreenYellow, npc.rotation, new Vector2(npc.width / 2 + npc.visualOffset.X, npc.height / 2 + npc.visualOffset.Y), 1f, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
            }*/

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D glowmask = mod.GetTexture("NPCs/Inpuratus/InpuratusGlow");

            for (int i = 0; i < npc.oldPos.Length; i++)
            {
                spriteBatch.Draw(glowmask, npc.oldPos[i] - Main.screenPosition + new Vector2(npc.width / 2, npc.height / 2) + new Vector2(0, 4), npc.frame, Color.Lerp(Color.White, Color.DarkSeaGreen, i / npc.oldPos.Length).MultiplyRGBA(new Color(255 - (i * 25), 255 - (i * 25), 255 - (i * 25), 255 - (i * 25))), npc.oldRot[i], new Vector2(npc.width / 2 + npc.visualOffset.X, npc.height / 2 + npc.visualOffset.Y), 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            npc.frame.Y = (int)Math.Floor(npc.frameCounter / 6) % Main.npcFrameCount[npc.type] * npc.height;
        }

        Vector2 FindCaveMid(Vector2 midPos)
        {
            {
                float[] scanarray = new float[3];
                float distance = 700;
                Vector2 addon = new Vector2(0, -1);
                Collision.LaserScan(midPos, addon, 0, distance, scanarray);

                distance = 0;
                foreach (float scan in scanarray)
                {
                    distance += (scan / scanarray.Length);
                }

                Vector2 dest1 = midPos + (addon * distance);

                scanarray = new float[3];
                distance = 700;
                addon = new Vector2(0, 1);
                Collision.LaserScan(midPos, addon, 0, distance, scanarray);

                distance = 0;
                foreach (float scan in scanarray)
                {
                    distance += (scan / scanarray.Length);
                }

                Vector2 dest2 = midPos + (addon * distance);

                return Vector2.Lerp(dest1, dest2, 0.5f);
            }
        }

        Vector2 FindCenterLeg(int legID)
        {
            Player player = Main.player[npc.target];

            bool bl = legID % 2 == 1;

            Vector2 pos = bl ? new Vector2((float)Math.Round(npc.Center.X / 400) * 400, npc.Center.Y) : new Vector2((float)Math.Round((npc.Center.X / 400) + 0.5f) * 400, npc.Center.Y);
            if (legID < 2)
            {
                pos = bl ? new Vector2((float)Math.Round(npc.Center.X / 400) * 400, npc.Center.Y) : new Vector2((float)Math.Round((npc.Center.X / 400) + 0.5f) * 400, npc.Center.Y);
            }

            pos.Y = MathHelper.Lerp(player.Center.Y, npc.Center.Y, 0.5f);

            return pos;
        }
    }
}