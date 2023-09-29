using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Humanizer;
using Terraria.Graphics.Effects;

namespace FFZmod.Projectiles
{
    public class RBomb : ModProjectile
    {
        int sourcePower = 500;
        public override void SetDefaults()
        {
            Projectile.width = 10; // 弹幕的碰撞箱宽度
            Projectile.height = 10; // 弹幕的碰撞箱高度
                                    // 这两个字段不赋值弹幕会射不出来！16*16的碰撞箱相当于泰拉里一个物块那么大
                                    // 特别注意，请不要搞什么碰撞箱大小设为贴图大小的骚操作，那会造成奇怪的后果
            Projectile.ignoreWater = true; // 弹幕是否忽视水
            Projectile.tileCollide = true; // 弹幕撞到物块会创死吗
            Projectile.penetrate = -1; // 弹幕的穿透数，默认1次
            Projectile.timeLeft = 300; // 弹幕的存活时间，它会从弹幕生成开始每次更新减1，为零时弹幕会被kill，默认3600
                                       // Projectile.alpha = 255; // 弹幕的透明度，0 ~ 255，0是完全不透明（int）
                                       // Projectile.Opacity = 1; // 弹幕的不透明度，0 ~ 1，0是完全透明，1是完全不透明(float)，用哪个你们自己挑，这两是互相影响的
            Projectile.friendly = true; // 弹幕是否攻击敌方，默认false
            Projectile.hostile = false; // 弹幕是否攻击友方和城镇NPC，默认false
            Projectile.DamageType = DamageClass.Ranged; // 弹幕的伤害类型，默认default，npc射的弹幕用这种，玩家的什么类型武器就设为什么吧

            // Projectile.aiStyle = ProjAIStyleID.Arrow; // 弹幕使用原版哪种弹幕AI类型
            // AIType = ProjectileID.FireArrow; // 弹幕模仿原版哪种弹幕的行为
            // 上面两条，第一条是某种行为类型，可以查源码看看，这里是箭矢，第二条要有第一条才有效果，是让这个弹幕能执行对应弹幕的特殊判定行为
            Projectile.aiStyle = -1; // 不用原版的就写这个，也可以不写

            // ai[1] 爆炸威力
            sourcePower = (int)Projectile.ai[1];


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            // 碰到物块就粘住
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;




            return false;
        }


        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            if (Projectile.tileCollide) Projectile.velocity.Y += 0.3f;
        }


        public override void Kill(int timeLeft)
        {
            sourcePower = (int)Projectile.ai[1];

            // Play explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            // Smoke Dust spawn
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            /*for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }*/

            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }

            for (float degree = 0; degree < MathHelper.Pi * 2f; degree += MathHelper.Pi / 256)
            {
                Vector2 direction = Vector2.UnitX.RotatedBy(degree);
                int power = (int)(sourcePower * Main.rand.Next(75, 125) / 100f);
                for (float distance = 0.2f; power > 0; distance += 0.75f)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position + direction * distance * 16, 10, 10, DustID.Torch, 0f, 0f, 100, default, 1f);
                    }

                    int targetPosX = (int)(Projectile.position.X / 16f + direction.X * distance);
                    int targetPosY = (int)(Projectile.position.Y / 16f + direction.Y * distance);

                    if (Main.rand.NextBool(10))
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                        dust.noGravity = true;
                    }


                    Tile targetTile = Main.tile[targetPosX, targetPosY];
                    int powerDecrase;
                    if (
                        targetTile.TileType == TileID.Dirt ||
                        targetTile.TileType == TileID.ClayBlock ||
                        targetTile.TileType == TileID.Sand ||
                        targetTile.TileType == TileID.HardenedSand ||
                        targetTile.TileType == TileID.Ash ||
                        targetTile.TileType == TileID.Mud ||
                        targetTile.TileType == TileID.Silt ||
                        targetTile.TileType == TileID.Slush ||
                        targetTile.TileType == TileID.SnowBlock
                        )
                        powerDecrase = 50;
                    else if (
                             targetTile.TileType == TileID.Ebonstone ||
                             targetTile.TileType == TileID.Crimstone ||
                             targetTile.TileType == TileID.Pearlstone ||
                             targetTile.TileType == TileID.Hellstone ||
                             targetTile.TileType == TileID.Cobalt ||
                             targetTile.TileType == TileID.Palladium ||
                             targetTile.TileType == TileID.BlueDungeonBrick ||
                             targetTile.TileType == TileID.PinkDungeonBrick ||
                             targetTile.TileType == TileID.GreenDungeonBrick
                            )
                        powerDecrase = 200;
                    else if (
                             targetTile.TileType == TileID.Mythril ||
                             targetTile.TileType == TileID.Orichalcum
                            )
                        powerDecrase = 300;
                    else if (
                             targetTile.TileType == TileID.Adamantite ||
                             targetTile.TileType == TileID.Titanium ||
                             targetTile.TileType == TileID.Spikes ||
                             targetTile.TileType == TileID.WoodenSpikes ||
                             targetTile.TileType == TileID.LihzahrdBrick
                            )
                        powerDecrase = 400;
                    else if (
                             targetTile.TileType == TileID.Chlorophyte
                            )
                        powerDecrase = 500;
                    else powerDecrase = 100;
                    if (power >= powerDecrase)
                    {
                        power -= powerDecrase;
                        WorldGen.KillTile(targetPosX, targetPosY);
                        WorldGen.KillWall(targetPosX, targetPosY);
                    }
                    else break;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}