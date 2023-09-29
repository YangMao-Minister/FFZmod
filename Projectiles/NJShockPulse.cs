using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.CodeAnalysis;

namespace FFZmod.Projectiles
{
    public class NJShockPulse : ModProjectile
    {
        private int pierceCount = 0;

        // timer1是类的成员，所以要写在正确的位置
        public int timer1
        {
            get
            {
                return (int)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = value;
            }
        }
        public override void SetStaticDefaults()
        {
            // 记录弹幕上一帧的位置
            ProjectileID.Sets.TrailingMode[Type] = 0;
            // 最多记录30帧
            ProjectileID.Sets.TrailCacheLength[Type] = 30;

        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // 弹幕的碰撞箱宽度
            Projectile.height = 10; // 弹幕的碰撞箱高度
                                    // 这两个字段不赋值弹幕会射不出来！16*16的碰撞箱相当于泰拉里一个物块那么大
                                    // 特别注意，请不要搞什么碰撞箱大小设为贴图大小的骚操作，那会造成奇怪的后果
            Projectile.ignoreWater = true; // 弹幕是否忽视水
            Projectile.tileCollide = true; // 弹幕撞到物块会创死吗
            Projectile.penetrate = -1; // 弹幕的穿透数，默认1次
            Projectile.timeLeft = 600; // 弹幕的存活时间，它会从弹幕生成开始每次更新减1，为零时弹幕会被kill，默认3600
            Projectile.alpha = 120; // 弹幕的透明度，0 ~ 255，0是完全不透明（int）
                                    // Projectile.Opacity = 1; // 弹幕的不透明度，0 ~ 1，0是完全透明，1是完全不透明(float)，用哪个你们自己挑，这两是互相影响的
            Projectile.friendly = true; // 弹幕是否攻击敌方，默认false
            Projectile.hostile = false; // 弹幕是否攻击友方和城镇NPC，默认false
            Projectile.DamageType = DamageClass.Melee; // 弹幕的伤害类型，默认default，npc射的弹幕用这种，玩家的什么类型武器就设为什么吧

            // Projectile.aiStyle = ProjAIStyleID.Arrow; // 弹幕使用原版哪种弹幕AI类型
            // AIType = ProjectileID.FireArrow; // 弹幕模仿原版哪种弹幕的行为
            // 上面两条，第一条是某种行为类型，可以查源码看看，这里是箭矢，第二条要有第一条才有效果，是让这个弹幕能执行对应弹幕的特殊判定行为
            Projectile.aiStyle = -1; // 不用原版的就写这个，也可以不写

            // Projectile.extraUpdates = 0; // 弹幕每帧的额外更新次数，默认0，这个之后细讲
            // 以及写一些关于无敌帧的设定

            // ai[0] 计时器
            // ai[1] 特殊弹幕
            // ai[2] 弹幕缩放
            if (Projectile.ai[1] == 1f) Projectile.extraUpdates = 10;

        }

        public override bool PreAI()
        {
            return base.PreAI();
        }

        public override void AI()
        {
            // 粒子
            //这是旋转后贴图左下和右上坐标
            Vector2 v1 = Projectile.Center + new Vector2(0, 50 * Projectile.scale).RotatedBy(Projectile.rotation);
            Vector2 v2 = Projectile.Center + new Vector2(50 * Projectile.scale, 0).RotatedBy(Projectile.rotation);


            Rectangle r = new Rectangle((int)Math.Min(v1.X, v2.X), (int)Math.Min(v1.Y, v2.Y), (int)Math.Abs((v1 - v2).X), (int)Math.Abs((v1 - v2).Y));

            float s = 0.8f;
            if (Projectile.ai[1] == 1f)
            {
                s = 2f;
                float u = (float)(Math.Cos(timer1 / 5f) + 1);
                Vector2 v3 = u * v1 + (1 - u) * v2;
                Vector2 v4 = u * v2 + (1 - u) * v1;
                Dust dust2 = Dust.NewDustDirect(v3, 2, 2, 91, 0, 0, 90, new Color(68, 73, 237), s);
                dust2.noGravity = true;

                Dust dust3 = Dust.NewDustDirect(v4, 2, 2, 91, 0, 0, 90, new Color(239, 80, 203), s);
                dust3.noGravity = true;
            }
            Dust dust = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, 91, 0, 0, 90, new Color(255, 255, 255), s);
            dust.noGravity = true;

            Lighting.AddLight(Projectile.Center, 2f, 2f, 2f);

            Player player = Main.player[Projectile.owner];

            Projectile.velocity *= 1.02f;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 2.357f; // 3* pi/4

            Projectile.scale = 0.4f * (float)Math.Sin(timer1 / 20f) + Projectile.ai[2];
            Projectile.alpha = (int)((0.4f * (float)Math.Cos(timer1 / 20f) + 1) * 90);

            timer1++;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 每次穿透减少5%伤害
            Projectile.damage = (int)(Projectile.damage * 0.95f);

            // 产生4个飞向中心的书页弹幕
            for (int i = 0; i < 4; i++)
            {
                Vector2 v = Main.rand.NextVector2Unit();
                Projectile.NewProjectile(
                   Projectile.GetSource_FromAI(),// 生成源一般不知道填什么的时候就这么写，反正没用
                   target.Center + v * 116f, v * -3f,
                   712, // 无限智慧巨著
                   (int)(Projectile.damage * 0.4f),
                   0,
                   Projectile.owner
                  );
            }

            // 暴击产生额外的超星炮弹幕
            if (hit.Crit)
            {
                Projectile.NewProjectile(
                       Projectile.GetSource_FromAI(),// 生成源一般不知道填什么的时候就这么写，反正没用
                       target.Center, Vector2.Zero,
                       ProjectileID.SuperStarSlash, // 无限智慧巨著
                       (int)(damageDone * 1.25f),
                       0,
                       Projectile.owner
                      );
            }

            pierceCount++;

            base.OnHitNPC(target, hit, damageDone);
        }


        public override void Kill(int timeLeft)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(-50 * Projectile.scale, 50 * Projectile.scale), (int)(50 * Projectile.scale), (int)(100 * Projectile.scale), 100, 0, 0, 90, new Color(255, 255, 255), 2f);
            dust.noGravity = true;

            SoundEngine.PlaySound(SoundID.Item118);

            int paperCount;

            if (Projectile.ai[1] == 1f)
            {
                Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center, Vector2.Zero,
                        624, // 爆星
                        (int)(Projectile.damage * 0.8f),
                        0,
                        Projectile.owner
                );
                paperCount = 12;
            }
            else
            {
                paperCount = 8;
            }

            for (int i = 0; i < paperCount; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit();
                Projectile.NewProjectile(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center + vel * 2f, vel * 8f,
                    712,
                    (int)(Projectile.damage * 0.8f),
                    0,
                    Projectile.owner);
            }


        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // This is how large the circumference is, aka how big the range is. Vanilla uses 94f to match it to the size of the texture.
            float coneLength = 35f * Projectile.scale;
            // This number affects how much the start and end of the collision will be rotated.
            // Bigger Pi numbers will rotate the collision counter clockwise.
            // Smaller Pi numbers will rotate the collision clockwise.
            float coneRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
            // 扫描碰撞箱前方-pi/4到pi/4范围
            float maximumAngle = MathHelper.PiOver2; // The maximumAngle is used to limit the rotation to create a dead zone.

            if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 为了解决碰撞箱不在贴图中心的问题，复写PreDraw方法 
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = Projectile.Size * 3 / 2;

            Color c = new Color(255, 255, 255);
            if (Projectile.ai[1] == 1f)
            {
                c.R = 146;
                c.G = 223;
                c.B = 248;
            }

            Main.EntitySpriteDraw(tex, pos, tex.Frame(), c, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            // 返回false禁用原版的绘制
            return false;
        }

    }
}