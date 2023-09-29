using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using FFZmod.Utils;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FFZmod.Projectiles;
using Microsoft.CodeAnalysis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace FFZmod.Items.Weapons
{
    public class OverloadedAurora : ModItem
    {
        private OAProjectile[] oaStars = new OAProjectile[7];
        private int shootCount = 0;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 9));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            // ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 16;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 110;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.shoot = ModContent.ProjectileType<OAProjectile>();
            Item.rare = ItemRarityID.Yellow;
            Item.knockBack = 0.5f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < 8; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item11);
                    OAProjectile oap = Projectile.NewProjectileDirect(source, player.Center, Vector2.UnitX.RotatedBy(i * MathHelper.PiOver4) * 6f, ModContent.ProjectileType<OAProjectile>(), Item.damage, knockback, player.whoAmI).ModProjectile as OAProjectile;
                    oap.target = Main.MouseWorld;
                }

                return false;
            };

            Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.position);
            if (shootCount == 7)
            // 发射7次之后
            {
                foreach (OAProjectile oap in oaStars)
                {
                    oap.target = Main.MouseWorld;
                }
                shootCount = ++shootCount % 8;
                SoundEngine.PlaySound(SoundID.Item8);
            }
            else
            {
                LightningArc p = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<LightningArc>(), damage, knockback, player.whoAmI).ModProjectile as LightningArc;
                p.initLightningArc(player.position, Main.MouseWorld, 8, 50, Color.Lerp(Color.Blue, Color.Red, shootCount / 7), genFloatingRange: 0.4f);

                OAProjectile oap = Projectile.NewProjectileDirect(source, Main.MouseWorld, dir * 3f, ModContent.ProjectileType<OAProjectile>(), Item.damage, knockback, player.whoAmI).ModProjectile as OAProjectile;
                oaStars[shootCount] = oap;
                shootCount = ++shootCount % 8;
                // Main.NewText($"{shootCount}");
            }


            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
    public class OAProjectile : ModProjectile
    {
        private int timer = 0;
        public Vector2 target;
        private int startIndex = 5;
        private bool isKilled = false;
        public override string Texture => "Terraria/Images/Extra_57";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 最多记录30帧
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            // Projectile.alpha = (int)MathHelper.Lerp(0, 255, timer / 90);
            // Projectile.scale += timer / 10;
            if (timer == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, 20, 20, 133, Main.rand.NextFloat(3f) + Projectile.velocity.X, Main.rand.NextFloat(3f) + Projectile.velocity.Y);
                    d.noGravity = true;
                }
            }
            else if (timer < 40)
            {
                Projectile.velocity += Vector2.Normalize(Main.player[Projectile.owner].position - Projectile.position) * 0.2f;
            }
            else if (40 <= timer && timer < 180)
            {

            }
            else if (timer == 180) PreKill(Projectile.timeLeft);

            if (target != Vector2.Zero)
            {
                Projectile.velocity += 100 * Vector2.Normalize(target - Projectile.position) / Vector2.Distance(target, Projectile.position);
                if (Vector2.Distance(Projectile.position, target) < 20) target = Vector2.Zero;
            }

            if (isKilled) startIndex++;

            if (startIndex > Projectile.oldPos.Length) Projectile.Kill();
            // Main.NewText($"{isKilled}");

            timer++;
        }


        public override bool PreKill(int timeLeft)
        {
            if (isKilled) return false;
            Projectile.damage = 0;
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, 20, 20, 133, Projectile.velocity.X * 1f, Projectile.velocity.Y * 1.5f);
                d.noGravity = true;
            }
            isKilled = true;
            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity
            });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!isKilled)
            {
                Texture2D tex = TextureAssets.Projectile[Type].Value;
                Vector2 pos = Projectile.position - Main.screenPosition;
                Vector2 origin = tex.Size() / 2;
                Vector2 scale = new Vector2((0.6f + Projectile.velocity.Length() * 0.05f), 0.6f + 0.2f * (float)Math.Sin(timer * 0.07f));
                Color c = Color.Lerp(Color.White, Color.LightBlue, 0.6f + 0.4f * (float)Math.Cos(timer * 0.07f));

                Main.spriteBatch.Draw(tex, pos, tex.Frame(), Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

                //Main.EntitySpriteDraw(tex, pos, tex.Frame(), Color.White, Projectile.rotation, origin, Projectile.scale * scale, SpriteEffects.None);
            }
            #region 绘制拖尾
            List<VertexInfo> vertices = new();
            for (int i = startIndex; i < Projectile.oldPos.Length; i++)
            {
                float l = (float)Projectile.oldPos.Length;
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                Color color = Color.Lerp(Color.Cyan, Color.White, i / l);
                //Color color = Color.Blue;
                Vector3 Depth1 = new Vector3(((float)((timer * 3) % l) + i) / (l * 2), 0f, 1 - i / l);
                Vector3 Depth2 = new Vector3(((float)((timer * 3) % l) + i) / (l * 2), 1f, 1 - i / l);
                // Main.NewText($"{Depth1}");

                Vector2 SidePoint1 = new Vector2((l - i) * 0.7f, 0f).RotatedBy(Projectile.oldRot[i] + MathHelper.PiOver2);
                Vector2 SidePoint2 = new Vector2((l - i) * 0.7f, 0f).RotatedBy(Projectile.oldRot[i] - MathHelper.PiOver2);

                if (i == startIndex)
                {
                    vertices.Add(new VertexInfo(Projectile.oldPos[i] - Main.screenPosition, new Vector3(0f, 0.5f, 1f), color));
                    continue;
                }

                vertices.Add(new VertexInfo(Projectile.oldPos[i] - Main.screenPosition + SidePoint1, Depth2, color));
                vertices.Add(new VertexInfo(Projectile.oldPos[i] - Main.screenPosition + SidePoint2, Depth1, color));
                // Main.NewText($"{Projectile.oldPos[i] - Main.screenPosition}");
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //Switch spritesortmode from Deferred to Immediate and switch blendstate from AlphaBlend to Additive.
            // Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Terraria/Images/Extra_194").Value;
            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Terraria/Images/Extra_194").Value;
            //This texture is vanilla one xD you can use urself's sprite of course.
            //这个材质是原版的，你当然可以换成自己的图片试试效果。
            if (vertices.Count >= 3)
            //Judge if there have over 3 vertices that adjoining each other. If not, we can't connect triangles between them.
            //判断是否有连续3个相邻的顶点参数，如果没有的话就无法连接成三角形然后绘制。
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                //TriangleStrip means you'll draw triangles between the vertices, and the amount of triangles should be vertices.Count - 2.
                //TriangleStrip意味着你会用三角形填充这些顶点参数的坐标连成的网格，三角形个数应当为顶点个数-2。
            }
            #endregion
            return false;
        }

    }

}
