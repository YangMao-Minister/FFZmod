using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FFZmod.Projectiles;
using Microsoft.CodeAnalysis;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System.CodeDom;
using System.Collections.Generic;

namespace FFZmod.Utils
{
    public class LightningArc : ModProjectile
    {
        Vector2 start;
        Vector2 end;
        Color color;
        List<Vector2> nodes = new();
        float updateOffset;
        int renderWidth;
        int defaultWidth;
        int nodeCount;
        int fadeTime;

        public override string Texture => "Terraria/Images/Extra_98";

        public void initLightningArc(Vector2 start, Vector2 end, int timeLeft, int width, Color color, int maxNodes = 30, int fadeTime = 20, float genOffset = 0.6f, float step = 64f, float updateOffset = 10f, float genFloatingRange = 0.5f)
        {
            Vector2 tempPos = start;
            this.start = start;
            this.end = end;
            this.updateOffset = updateOffset;
            Projectile.timeLeft = timeLeft;
            this.renderWidth = width;
            this.defaultWidth = width;
            this.fadeTime = fadeTime;
            this.color = color;

            while (true)
            {
                float d = Vector2.Distance(tempPos, end);
                if (nodes.Count > maxNodes) break;
                if (d < step)
                // 如果距离小于步长的话直接连到终点
                {
                    nodes.Add(end);
                    break;
                }

                nodes.Add(tempPos);

                // 定向随机游走生成节点
                Vector2 ranDir = end + Main.rand.NextVector2Unit() * d * genOffset;

                tempPos += Vector2.Normalize(ranDir - tempPos) * step * (1f + Main.rand.NextFloat(-genFloatingRange, genFloatingRange));
            }
            this.nodeCount = nodes.Count;
            SoundEngine.PlaySound(new SoundStyle($"{nameof(FFZmod)}/Sounds/AM_IND-Arc_edited") { Volume = 2f });


        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = false;
            Projectile.timeLeft = Projectile.timeLeft;
            Projectile.friendly = true;
        }


        public override void AI()
        {
            for (int i = 0; i < this.nodeCount; i++)
            {
                if (nodes[i] == Vector2.Zero) continue;
                if (Main.rand.NextBool(30))
                {
                    this.nodes[i] += updateOffset * Main.rand.NextVector2Unit();
                    // Main.NewText($"第{i}号节点: {nodes[i]}");
                }
            }
            if (Projectile.timeLeft < this.fadeTime)
            {
                this.renderWidth = this.defaultWidth * Projectile.timeLeft / this.fadeTime;
            }
            Lighting.AddLight(this.end, color.R / 128, color.G / 128, color.B / 128);
            Lighting.AddLight(this.start, color.R / 128, color.G / 128, color.B / 128);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo> vertices = new();
            for (int i = 1; i < this.nodeCount - 1; i++)
            {
                if (nodes[i] == Vector2.Zero) continue;
                // Main.NewText($"第{i}号节点: {nodes[i]}");
                // 绘制端点的屏幕坐标
                // Dust.NewDustPerfect(nodes[i], MyDustId.BlueMagic);

                Vector2 nodeBefore = nodes[i - 1] - Main.screenPosition;
                Vector2 nodeNow = nodes[i] - Main.screenPosition;
                Vector2 nodeNext = nodes[i + 1] - Main.screenPosition;
                float dir = ((nodeNext - nodeNow).ToRotation() + (nodeBefore - nodeNow).ToRotation()) / 2;

                Vector3 Depth1 = new Vector3(0f, 0f, 1f);
                Vector3 Depth2 = new Vector3(1f, 1f, 1f);

                Color color = Color.Lerp(this.color, Color.White, Main.rand.NextFloat());

                if (i == 1)
                {
                    vertices.Add(new VertexInfo(nodeBefore, Depth1, color));
                    continue;
                }
                else if (i == this.nodeCount - 2)
                {
                    vertices.Add(new VertexInfo(nodeNext, Depth2, color));
                    break;
                }

                vertices.Add(new VertexInfo(nodeNow + renderWidth * Vector2.UnitX.RotatedBy(dir - MathHelper.PiOver2 + Main.rand.NextFloat(MathHelper.TwoPi / 10)) * Main.rand.NextFloat(0.6f, 1.3f), Depth1, color));
                vertices.Add(new VertexInfo(nodeNow + renderWidth * Vector2.UnitX.RotatedBy(dir + MathHelper.PiOver2 + Main.rand.NextFloat(MathHelper.TwoPi / 10)) * Main.rand.NextFloat(0.6f, 1.3f), Depth2, color));
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //Switch spritesortmode from Deferred to Immediate and switch blendstate from AlphaBlend to Additive.
            // Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Terraria/Images/Extra_194").Value;
            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("FFZMod/Images/Ex1").Value;
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
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //Remember to switch back the SpriteSortMode and BlendState to vanilla ones.
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            for (int i = 1; i < this.nodeCount - 1; i++)
            {
                if (Terraria.Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), nodes[i], nodes[i + 1], 16, ref point))
                    return true;
            }
            return false;
        }

    }

}
