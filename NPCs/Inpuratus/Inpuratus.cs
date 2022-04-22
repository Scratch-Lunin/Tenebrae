using Tenebrae.Items.Armor;
using Tenebrae.Items.Misc;
using Tenebrae.Projectiles.Inpuratus;

namespace Tenebrae.NPCs.Inpuratus
{
    [AutoloadBossHead]
    public class Inpuratus : ModNPC
    {
        bool start = false;
        float dead = 0;
        Vector2[] FootPositions = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        float FootOffset = 0;

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
        public ref float AttackTimer => ref npc.ai[1];
        public enum AttackState
        {
            JustMoving = 0,
            Fireball = 1,
            Dashing = 2,
            StompDown = 3,
            HomingSac = 4,
            SlowingSpread = 5,
            StompUp = 6,
            FireBreath = 7
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
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossLow;
            npc.width = 118;
            npc.height = 208;
            npc.scale = 1f;
            npc.damage = 30;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.defense = 30;
            npc.lifeMax = 25000;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = Item.buyPrice(0, 10, 0, 0);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.buffImmune[BuffID.Confused] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            bossBag = ModContent.ItemType<InpuratusBag>();
        }

        #region Loot
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Placable.InpuratusTrophy>());
            }

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                if (Main.rand.NextBool(7))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<InpuratusMask>());
                }

                /*if (Main.rand.NextBool(3))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<VileAmulet>());
                }

                switch (Main.rand.Next(3))
                {
                    case 0:
                        Item.NewItem(npc.getRect(), ModContent.ItemType<CursefernoBurst>());
                        break;
                    case 1:
                        Item.NewItem(npc.getRect(), ModContent.ItemType<VileGlaive>());
                        break;
                    case 2:
                        Item.NewItem(npc.getRect(), ModContent.ItemType<CursedCarbine>());
                        break;
                }*/

                Item.NewItem(npc.getRect(), ItemID.CursedFlame, 20 + Main.rand.Next(10));
                Item.NewItem(npc.getRect(), ItemID.RottenChunk, 50 + Main.rand.Next(10));
                Item.NewItem(npc.position, ItemID.CursedFlame, 20 + Main.rand.Next(10));
                Item.NewItem(npc.position, ItemID.RottenChunk, 50 + Main.rand.Next(10));
                var dropChooser = new WeightedRandom<int>();
                //dropChooser.Add(ModContent.ItemType<Items.Weapons.Mage.CursefernoBurst>(), 5);
                //dropChooser.Add(ModContent.ItemType<Items.Weapons.Melee.VileGlaive>(), 5);
                //dropChooser.Add(ModContent.ItemType<Items.Weapons.Ranger.CursedCarbine>(), 5);
                int choice = dropChooser;
                Item.NewItem(npc.getRect(), choice);
            }
        }
        #endregion

        public override void AI()
        {
            if (!start)
            {
                CurrentAttackState = AttackState.StompDown;
                for (int i = 0; i < 4; i++)
                {
                    FootPositions[i] = npc.Center;
                }
                start = true;
            }

            npc.TargetClosest();

            Player player = Main.player[npc.target];

            float vel = (float)Math.Sqrt(Math.Pow(npc.velocity.X, 2) + Math.Pow(npc.velocity.Y, 2));

            float rot = npc.AngleFrom(player.Center) + MathHelper.ToRadians(90f);

            Vector2 targetPosition;

            if (player.dead)
            {
                dead++;

                npc.velocity.X *= 0.995f;
                npc.velocity.Y += 0.06f;

                if (dead > 60 && npc.Distance(player.Center) > 2000)
                {
                    npc.active = false;
                }
            }
            else
            {
                AttackTimer++;

                if (CurrentAttackState == AttackState.JustMoving)
                {
                    targetPosition = FindCaveMid(player.Center);
                    float dist = npc.Distance(targetPosition);
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(targetPosition) * dist * 0.05f, 0.2f);
                    if (vel > 10) npc.velocity *= 0.95f;
                    if (vel > 15) npc.velocity *= 0.95f;

                    if (AttackTimer > 150)
                    {
                        CurrentAttackState = AttackState.Fireball;
                        AttackTimer = 0;
                    }
                }
                if (CurrentAttackState == AttackState.Fireball)
                {
                    npc.velocity *= 0.97f;
                    if (AttackTimer <= 120 && AttackTimer >= 60)
                    {
                        targetPosition = FindCaveMid(player.Center);
                        npc.velocity *= 0.97f;
                        if (AttackTimer % 15 == 0)
                        {
                            Vector2 v = npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 11))) * 2;
                            npc.velocity += -v * 0.2f;
                            Projectile.NewProjectile(npc.Center + (v * 45f), v, ModContent.ProjectileType<InpuratusBigFireball>(), 17, 6f);
                            Projectile.NewProjectile(npc.Center + (v * 45f), Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), 17, 6f);
                            Main.PlaySound(SoundID.Item73, (int)npc.Center.X, (int)npc.Center.Y);
                        }
                        else
                        {
                            npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(targetPosition) * 5, 0.12f);
                        }
                    }
                    if (AttackTimer > 130)
                    {
                        AttackTimer = 0;
                        CurrentAttackState = AttackState.Dashing;
                    }
                }
                if (CurrentAttackState == AttackState.StompDown)
                {
                    npc.velocity *= 0.93f;
                    npc.velocity *= 0.93f;

                    if (AttackTimer >= 40 && AttackTimer < 70)
                    {
                        FootOffset = MathHelper.Lerp(FootOffset, -110, 0.09f);
                    }
                    else if (AttackTimer > 70)
                    {
                        FootOffset += MathHelper.Clamp((AttackTimer - 70) / 3, 0, 10);
                        FootOffset = MathHelper.Clamp(FootOffset, -100, 0);
                        if (FootOffset == 0)
                        {
                            Main.PlaySound(SoundID.Item14);
                            SpawnPillars(true);
                            AttackTimer = -2;
                        }
                    }
                    else if (AttackTimer < 0)
                    {
                        AttackTimer -= 2;
                        if (AttackTimer < 100)
                        {
                            AttackTimer = 0;
                            CurrentAttackState = AttackState.JustMoving;
                        }
                    }
                }
                if (CurrentAttackState == AttackState.Dashing)
                {
                    float dashCounter = 90;
                    if (AttackTimer % dashCounter < 20)
                    {
                        npc.velocity += player.DirectionTo(npc.Center) * 0.3f;
                        npc.velocity *= 0.96f;
                    }
                    else if (AttackTimer % dashCounter == 20)
                    {
                        npc.velocity = npc.DirectionTo(player.Center) * 15;
                        Main.PlaySound(SoundID.ForceRoar, npc.Center, 0);
                    }
                    else if (AttackTimer % dashCounter < dashCounter - 20 && AttackTimer % 6 == 0)
                    {
                        Main.PlaySound(SoundID.Item73, (int)npc.Center.X, (int)npc.Center.Y);
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), 18, 5f);
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<InpuratusStillFireball>(), 18, 5f);
                    }
                    if (AttackTimer >= dashCounter * 4)
                    {
                        AttackTimer = 0;
                        CurrentAttackState = AttackState.HomingSac;
                    }
                }
                if (CurrentAttackState == AttackState.HomingSac)
                {
                    targetPosition = player.Center + new Vector2(0, -600).RotatedBy(player.AngleTo(npc.Center) + MathHelper.ToRadians(0.01f));
                    float dist = npc.Distance(targetPosition);
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(targetPosition) * dist * 0.07f, 0.2f);
                    if (vel > 10) npc.velocity *= 0.95f;
                    if (vel > 15) npc.velocity *= 0.95f;

                    if (AttackTimer % 40 == 0 && AttackTimer > 80)
                    {
                        float aitwo = 1 + Main.rand.NextFloat(0.01f, 0.07f);
                        if (Main.rand.NextBool(2)) aitwo = -aitwo;

                        int sac = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CursedSac>(), ai2: aitwo);
                        Main.npc[sac].velocity = player.DirectionTo(npc.Center).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-60, 61))) * 9f;
                    }
                    if (AttackTimer > 300)
                    {
                        AttackTimer = 0;
                        CurrentAttackState = AttackState.StompDown;
                    }
                }
            }

            npc.rotation = rot;

            for (int i = 0; i < 4; i++)
            {
                Vector2 addon = new Vector2(-1, -1).RotatedBy(MathHelper.ToRadians(i * 90));
                //addon.X /= 6f;
                //addon.Y /= 2f;

                float sign = Math.Sign(addon.X);
                addon.X = 0;

                float[] scanarray2 = new float[3];
                float dist2 = 200;
                Collision.LaserScan(FindCenterLeg(i), new Vector2(sign, i < 2 ? -0.5f : 0.5f), 0, dist2, scanarray2);

                dist2 = 0;
                foreach (float scan in scanarray2)
                {
                    dist2 += (scan / scanarray2.Length);
                }


                float[] scanarray = new float[3];
                float distance = 800;
                Collision.LaserScan(FindCenterLeg(i) + new Vector2(sign * dist2, 0), addon, 0, distance, scanarray);

                distance = 0;
                foreach (float scan in scanarray)
                {
                    distance += (scan / scanarray.Length);
                }


                Vector2 dest = new Vector2((float)Math.Floor((FindCenterLeg(i) + new Vector2(sign * dist2, 0) + (addon * distance)).X / 16) * 16, (float)Math.Floor((FindCenterLeg(i) + new Vector2(sign * 200, 0) + (addon * distance)).Y / 16) * 16);

                float offY = 0;

                if (i >= 2)
                {
                    offY = MathHelper.Clamp(FootOffset, -999, 0);
                }
                else
                {
                    offY = MathHelper.Clamp(FootOffset, 0, 999);
                }

                float posneg = -1;
                if (i < 2) posneg = 1;

                float vel10 = MathHelper.Clamp(vel / 10, 1f, 3f);

                if (Math.Abs(offY) < 1) FootPositions[i] = Vector2.Lerp(FootPositions[i], dest, 0.17f * vel10);
                else FootPositions[i] = Vector2.Lerp(FootPositions[i], dest, 0.3f);
                FootPositions[i].Y += Math.Abs((dest - FootPositions[i]).X) / 13 * posneg * vel10;
                FootPositions[i].Y += (offY);
            }

            /*for (int i = 0; i < 4; i++)
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
            }*/

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

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                TenebraeWorld.InpuratusDies = true;
                TenebraeWorld.downedInpuratus = true;
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
                for (int i = 0; i < 8; i++)
                {
                    Gore.NewGore(npc.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 4)), mod.GetGoreSlot("Gores/InpuratusGore" + i), npc.scale);
                }
                for (int i = 0; i < 40; i++)
                {
                    float xSpeed = Main.rand.NextFloat(-2f, 2f);
                    float ySpeed = Main.rand.NextFloat(-2f, 2f);
                    Dust.NewDust(npc.position, 1, 1, DustID.Vile, xSpeed, ySpeed, 100, default(Color), 1f);
                }
            }
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

        void SpawnPillars(bool startAtBottom)
        {
            Projectile.NewProjectile(npc.Center, new Vector2(8, 0), ModContent.ProjectileType<InpuratusPillarSpawner>(), 10, 5);
            Projectile.NewProjectile(npc.Center, new Vector2(-8, 0), ModContent.ProjectileType<InpuratusPillarSpawner>(), 10, 5);
        }
        Vector2 FindCenterLeg(int legID)
        {
            Vector2 addon = new Vector2(-150, -150).RotatedBy(MathHelper.ToRadians(legID * 90));

            bool bl = legID % 2 == 1;

            Vector2 pos = bl ? new Vector2((float)Math.Round(npc.Center.X / 400) * 400, npc.Center.Y) : new Vector2((float)Math.Round((npc.Center.X / 400) + 0.5f) * 400, npc.Center.Y);
            if (legID < 2)
            {
                pos = bl ? new Vector2((float)Math.Round(npc.Center.X / 400) * 400, npc.Center.Y) : new Vector2((float)Math.Round((npc.Center.X / 400) + 0.5f) * 400, npc.Center.Y);
            }
            pos.Y = bl ? (float)Math.Round(npc.Center.Y / 300) * 300 : (float)Math.Round((npc.Center.Y / 300) + 0.5f) * 300;
            pos += addon;

            return pos;
        }
    }
}