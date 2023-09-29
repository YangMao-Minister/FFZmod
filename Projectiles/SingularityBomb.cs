using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.Graphics.Effects;
using Terraria.Audio;
using Terraria.DataStructures;
using FFZmod.Utils;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace FFZmod.Projectiles
{
    public class SingularityBomb : ModProjectile
    {
        private int state = 0;
        private int timer = 0;
        private float S2scale;
        private float Sscale;
        private float sigma;
        private float effectRadius;
        private float shockWaveSpeed;
        private float r;

        private float x;

        private Texture2D SFlash = (Texture2D)ModContent.Request<Texture2D>("FFZmod/Projectiles/SingularityBomb_Glow");
        public override string Texture => "FFZmod/Items/Weapons/SingularityBomb";
        public override void SetDefaults()
        {
            Projectile.width = 10; // 弹幕的碰撞箱宽度
            Projectile.height = 10; // 弹幕的碰撞箱高度
                                    // 这两个字段不赋值弹幕会射不出来！16*16的碰撞箱相当于泰拉里一个物块那么大
                                    // 特别注意，请不要搞什么碰撞箱大小设为贴图大小的骚操作，那会造成奇怪的后果
            Projectile.ignoreWater = true; // 弹幕是否忽视水
            Projectile.tileCollide = true; // 弹幕撞到物块会创死吗
            Projectile.penetrate = -1; // 弹幕的穿透数，默认1次
            Projectile.timeLeft = 3600; // 弹幕的存活时间，它会从弹幕生成开始每次更新减1，为零时弹幕会被kill，默认3600
                                        // Projectile.alpha = 255; // 弹幕的透明度，0 ~ 255，0是完全不透明（int）
                                        // Projectile.Opacity = 1; // 弹幕的不透明度，0 ~ 1，0是完全透明，1是完全不透明(float)，用哪个你们自己挑，这两是互相影响的
            Projectile.friendly = true; // 弹幕是否攻击敌方，默认false
            Projectile.hostile = false; // 弹幕是否攻击友方和城镇NPC，默认false
            Projectile.DamageType = DamageClass.Default; // 弹幕的伤害类型，默认default，npc射的弹幕用这种，玩家的什么类型武器就设为什么吧

            // Projectile.aiStyle = ProjAIStyleID.Arrow; // 弹幕使用原版哪种弹幕AI类型
            // AIType = ProjectileID.FireArrow; // 弹幕模仿原版哪种弹幕的行为
            // 上面两条，第一条是某种行为类型，可以查源码看看，这里是箭矢，第二条要有第一条才有效果，是让这个弹幕能执行对应弹幕的特殊判定行为
            Projectile.aiStyle = -1; // 不用原版的就写这个，也可以不写


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 碰到物块就粘住
            Projectile.velocity.X = 0f;
            Projectile.velocity.Y = -8f;
            Projectile.tileCollide = false;
            timer = 0;
            state = 1;

            return false;
        }


        public override void AI()
        {
            timer++;
            if (state == 0)
            {
                Projectile.velocity.Y += 0.3f;
                Projectile.rotation += Projectile.velocity.X * 0.05f;

            }
            else if (state == 1)
            {
                if (timer == 59)
                {
                    SoundStyle launchSound = new SoundStyle($"{nameof(FFZmod)}/Sounds/singularity2")
                    {
                        Volume = 1.2f,
                    };
                    SoundEngine.PlaySound(launchSound);
                }

                if (timer > 10)
                {
                    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 0.0f, 0.1f);

                    if (Projectile.velocity.Y > -0.02f)
                    {
                        Projectile.velocity.Y = 0f;
                    }
                }

                Projectile.rotation += timer * MathHelper.Pi / 360;

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustDirect(Projectile.position + new Vector2(-10, -10), 20, 20, DustID.Vortex, Scale: 0.5f);
                }

                if (timer >= 60)
                {
                    int t = timer - 60;

                    Projectile.rotation += t * MathHelper.Pi / 360;

                    Vector2 v = Projectile.position + new Vector2(8, 8) - Main.screenPosition;
                    Vector2 centerPos = new Vector2(v.X / Main.screenWidth, v.Y / Main.screenHeight);
                    Sscale = 5f;
                    Sscale *= Main.rand.NextFloat(0.85f, 1.15f);
                    sigma = 0.01f;

                    effectRadius = -0.000004f * (t - 160) * (t - 160) + 0.08f;
                    effectRadius *= Main.rand.NextFloat(0.85f, 1.15f);


                    // Main.NewText($"中心坐标{v} 换算后：{centerPos}");
                    Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                    Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(effectRadius);
                    Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(Sscale);
                    Filters.Scene["SZoom"].GetShader().Shader.Parameters["uSigma"].SetValue(0.02f);

                    Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                    Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(0.005f * t);
                    Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(1.002f);
                    Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uSigma"].SetValue(0.5f + 0.005f * t);

                    // 开启滤镜
                    Filters.Scene.Activate("SZoom2");
                    Filters.Scene.Activate("SZoom");

                    foreach (Item e in Main.item)
                    {
                        Vector2 offset = Projectile.position - e.position;
                        if (offset.Length() < 640)
                        {
                            e.velocity += offset * timer / 500f;

                            if (e.velocity.Length() > 10f)
                            {
                                e.velocity = e.velocity * 50f / e.velocity.Length();
                            }
                        }
                    }

                    foreach (NPC n in Main.npc)
                    {
                        Vector2 offset = Projectile.position - n.position;
                        if (offset.Length() < 640)
                        {
                            n.velocity += offset * timer / 500f;
                        }
                    }

                    foreach (Player p in Main.player)
                    {
                        Vector2 offset = Projectile.position - p.position;
                        if (offset.Length() < 640)
                        {
                            p.velocity += 300 * offset / offset.LengthSquared();
                        }
                    }


                    if (Main.rand.NextBool())
                    {
                        Lighting.AddLight(Projectile.position, 3f, 3f, 7f);
                        SoundStyle arcS = new SoundStyle($"{nameof(FFZmod)}/Sounds/AM_IND-Arc_edited")
                        {
                            Volume = 2f,
                        };
                        SoundEngine.PlaySound(arcS);
                    }

                    if (Main.rand.NextBool())
                    {
                        for (int i = 0; i < Main.rand.Next(0, 2); i++)
                        {
                            // 生成闪电粒子
                            //Vector2 sparkPos = Main.player[Projectile.owner].position;
                            Vector2 sparkPos = Main.rand.NextVector2Unit() * Main.rand.Next(900, 1300) + Projectile.position;
                            LightningArc p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<LightningArc>(), 0, 0).ModProjectile as LightningArc;
                            p.initLightningArc(Projectile.position, sparkPos, 8, 20, Color.Blue, genFloatingRange: 0.4f);
                        }
                    }

                }

                if (timer == 240)
                {
                    state = 2;
                    timer = 0;
                }
            }
            else if (state == 2)
            {
                Projectile.hide = true;
                x = timer / 100f;

                Vector2 v = Projectile.position + new Vector2(8, 8) - Main.screenPosition;
                Vector2 centerPos = new Vector2(v.X / Main.screenWidth, v.Y / Main.screenHeight);
                Sscale -= 0.2f / 360;
                shockWaveSpeed *= 0.99f;
                r += shockWaveSpeed;

                Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(0.08f + 0.015f * timer);
                Filters.Scene["SZoom"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(Sscale);
                Filters.Scene["SZoom"].GetShader().Shader.Parameters["uSigma"].SetValue(0.4f + 0.005f * timer);

                Filters.Scene["ShockWave"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                Filters.Scene["ShockWave"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(r);
                Filters.Scene["ShockWave"].GetShader().Shader.Parameters["uEffectPower"].SetValue(0.0873f - 0.000242f * timer);
                Filters.Scene["ShockWave"].GetShader().Shader.Parameters["uSigma"].SetValue(200f);
                Filters.Scene["ShockWave"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(MathHelper.Lerp(1.3f, 1.0f, timer / 360f));

                Filters.Scene["Explosion"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                Filters.Scene["Explosion"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(-0.00000324f * timer * timer + 0.00117f * timer);

                Filters.Scene["Explosion2"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                Filters.Scene["Explosion2"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(-0.0000000341f * timer * timer * timer + 0.0000157f * timer * timer - 0.00122f * timer);

                Filters.Scene.Activate("SZoom");
                Filters.Scene.Activate("Explosion");
                Filters.Scene.Activate("Explosion2");
                Filters.Scene.Activate("ShockWave");

                if (timer > 60)
                {
                    /*                    S2scale -= 0.1f / 360;
                                        Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectCenterPos"].SetValue(centerPos);
                                        Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectRadius"].SetValue(0.00005f * timer);
                                        Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(S2scale);
                                        Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uSigma"].SetValue(0.5f + 0.005f * timer);*/
                }


                if (timer == 1)
                {
                    shockWaveSpeed = 0.005f;
                    r = 0.1f;
                    S2scale = 1.1f;
                    Filters.Scene["SZoom2"].GetShader().Shader.Parameters["uEffectZoom"].SetValue(1f);

                    Sscale = 1.2f;
                    Projectile.position += new Vector2(-80, -80);
                    Projectile.height = 160;
                    Projectile.width = 160;
                    Projectile.damage = 1;
                    Projectile.friendly = false;

                    SoundStyle bang = new SoundStyle($"{nameof(FFZmod)}/Sounds/ScavSpearBang")
                    {
                        Volume = 5f,
                    };
                    SoundEngine.PlaySound(bang);

                    foreach (Item e in Main.item)
                    {
                        Vector2 offset = Projectile.position - e.position;
                        if (offset.Length() < 1600)
                        {
                            e.velocity -= 20 * offset / offset.Length();
                        }
                    }

                    foreach (NPC n in Main.npc)
                    {
                        Vector2 offset = Projectile.position - n.position;
                        if (offset.Length() < 1600)
                        {
                            n.velocity -= 20 * offset / offset.Length();
                        }
                    }

                    foreach (Player p in Main.player)
                    {
                        Vector2 offset = Projectile.position - p.position;
                        if (offset.Length() < 1600)
                        {
                            p.velocity -= 20 * offset / offset.Length();
                        }
                    }
                }

                if (timer == 2)
                {
                    Projectile.position += new Vector2(80, 80);
                    Projectile.height = 1;
                    Projectile.width = 1;
                    Projectile.damage = 0;
                    Projectile.friendly = true;
                }

                if (1 <= timer && timer <= 20)
                {
                    Lighting.AddLight(Projectile.position, 20f, 20f, 35f);
                }


                if (timer >= 20 && timer <= 200)
                {
                    SoundEngine.StopAmbientSounds();
                    SoundEngine.StopTrackedSounds();
                    Main.audioSystem.PauseAll();
                }


                if (timer == 360)
                {
                    state = 3;
                    timer = 0;
                    Filters.Scene.Deactivate("SZoom");
                    Filters.Scene.Deactivate("SZoom2");
                    Filters.Scene.Deactivate("Explosion");
                    Filters.Scene.Deactivate("Explosion2");
                    Projectile.Kill();

                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.InstantKill = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetInstantKill();
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.KillMe(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), int.MaxValue, 1);

        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            target.KillMe(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), int.MaxValue, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;

            /*            if (60 <= timer && timer <= 240)
                        {
                            Color c = Color.Lerp(new Color(2, 17, 255, 20), new Color(85, 70, 239, 50), Main.rand.NextFloat());

                            Main.EntitySpriteDraw(SFlash, pos, SFlash.Frame(), c, 0, SFlash.Size() / 2, Main.rand.NextFloat(2f, 3f), SpriteEffects.None);
                        }*/


            Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }

    }

    public class LightningSpark : ModProjectile
    {
        private Vector2 target;
        private int timer = 0;
        private float dustScale = 0.5f;

        public override string Texture => "Terraria/Images/Extra_98";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hide = true;
            Projectile.penetrate = -1;


        }
        public override void AI()
        {
            if (timer == 0)
            {
                target = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                Projectile.timeLeft = (int)Projectile.ai[2];
                Projectile.extraUpdates = Projectile.timeLeft;
                dustScale = Projectile.knockBack;
            }

            Dust dust = Dust.NewDustPerfect(Projectile.Center, MyDustId.LightCyanParticle1);
            dust.noGravity = true;
            dust.velocity = Vector2.Zero;
            dust.scale = dustScale;

            Vector2 ranDir = target + Main.rand.NextVector2Unit() * Vector2.Distance(Projectile.position, target) * 2f;

            Projectile.velocity = Vector2.Normalize(ranDir - Projectile.position) * 4f;

            if ((target - Projectile.position).Length() < 10f)
            {
                Projectile.Kill();
            }

            timer++;
        }

        public override void Kill(int timeLeft)
        {
            Lighting.AddLight(Projectile.position, 1f, 1f, 2f);
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric);
                dust.noGravity = true;
                dust.velocity = Main.rand.NextVector2Unit() * 2f;
                dust.scale = 1.3f;
            }
            base.Kill(timeLeft);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 60;
            hitbox.Height = 60;
            hitbox.X = (int)Projectile.position.X - 30;
            hitbox.Y = (int)Projectile.position.Y - 30;
        }
    }

}
