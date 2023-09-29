using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Security.Permissions;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using System;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace FFZmod
{
    public struct VertexInfo : IVertexType
    //IVertexType is a vertex shader offered by vanilla Terraria.
    //IVertexType 是原版Terraria提供的顶点着色器。
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });
        //For vertex shader, we need 3 parameters for each element in the list passed into it. The list includes position, color and texcoord.
        //对于顶点着色器，我们需要对列表中的每个元素传入三个参数。这三个参数分别是：坐标（屏幕坐标），坐标映射到材质上的位置，颜色。
        public Vector2 Position;//Represents the screen coordinate of the draw things.代表绘制的屏幕坐标。
        public Color Color;//Represents the color.代表颜色。
        public Vector3 TexCoord;
        //Represents the Position's corrosponding position in the sprite picture passed into vertex shader.This is a vector3, with two parameters stand for X and Y coordinates in the picture (both vary from 0 to 1) and one parameter for "depth".
        //代表这个坐标映射到材质上的位置。这个三维向量的前两个参数代表着映射后位置的XY坐标（范围从0到1），最后一个参数代表“深度”。
        public VertexInfo(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
        //Declare a VertexInfo, so we can easily pass the parameters in other files.
        //声明一个VertexInfo，这样在其他文件中我们就可以轻易地给顶点着色器传参了。
        public VertexDeclaration VertexDeclaration
        {
            get => _vertexDeclaration;
        }
    }
    public class FFZmod : Mod
    {
        public static Effect effect1;
        public static Effect testNpcEffect;
        public static Effect GBlurEffect;

        public override void Load()
        {
            effect1 = Assets.Request<Effect>("Effects/Effect1", AssetRequestMode.ImmediateLoad).Value;
            // testNpcEffect = Assets.Request<Effect>("Effects/npcEffect", AssetRequestMode.ImmediateLoad).Value;




            // 给Filters.Scene注册上新建的ScreenShaderData
            // 注意设置正确的Pass名字，Scene的名字可以随便填，不和别的Mod以及原版冲突即可
            /* Filters.Scene["FFZmod:ScreenZoom"] = new Filter(
                new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/Effect1", (AssetRequestMode) 2).Value), "ScreenZoom"),
                EffectPriority.Medium
            ); */
            // Filters.Scene["FFZmod:ScreenZoom"].Load();

            // 注意设置正确的Pass名字，Scene的名字可以随便填，不和别的Mod以及原版冲突即可
            Filters.Scene["SZoom"] = new Filter(
                 new TestScreenShaderData(new Ref<Effect>(effect1), "SZoom"),
                 EffectPriority.Medium
            );
            Filters.Scene["SZoom"].Load();

            Filters.Scene["SZoom2"] = new Filter(
                 new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/Effect1_2", AssetRequestMode.ImmediateLoad).Value), "SZoom"),
                 EffectPriority.Medium
            );  
            Filters.Scene["SZoom2"].Load();

            Filters.Scene["Explosion"] = new Filter(
                 new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/Explosion", AssetRequestMode.ImmediateLoad).Value), "Explosion"),
                 EffectPriority.Medium
            );
            Filters.Scene["Explosion"].Load();

            Filters.Scene["Explosion2"] = new Filter(
                new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/Explosion2", AssetRequestMode.ImmediateLoad).Value), "Explosion"),
                EffectPriority.Medium
           );
            Filters.Scene["Explosion2"].Load();

            Filters.Scene["ShockWave"] = new Filter(
                 new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/ShockWave", AssetRequestMode.ImmediateLoad).Value), "ShockWave"),
                 EffectPriority.Medium
            );
            Filters.Scene["ShockWave"].Load();
        }
    }
}