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
    //IVertexType ��ԭ��Terraria�ṩ�Ķ�����ɫ����
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });
        //For vertex shader, we need 3 parameters for each element in the list passed into it. The list includes position, color and texcoord.
        //���ڶ�����ɫ����������Ҫ���б��е�ÿ��Ԫ�ش������������������������ֱ��ǣ����꣨��Ļ���꣩������ӳ�䵽�����ϵ�λ�ã���ɫ��
        public Vector2 Position;//Represents the screen coordinate of the draw things.������Ƶ���Ļ���ꡣ
        public Color Color;//Represents the color.������ɫ��
        public Vector3 TexCoord;
        //Represents the Position's corrosponding position in the sprite picture passed into vertex shader.This is a vector3, with two parameters stand for X and Y coordinates in the picture (both vary from 0 to 1) and one parameter for "depth".
        //�����������ӳ�䵽�����ϵ�λ�á������ά������ǰ��������������ӳ���λ�õ�XY���꣨��Χ��0��1�������һ������������ȡ���
        public VertexInfo(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
        //Declare a VertexInfo, so we can easily pass the parameters in other files.
        //����һ��VertexInfo�������������ļ������ǾͿ������׵ظ�������ɫ�������ˡ�
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




            // ��Filters.Sceneע�����½���ScreenShaderData
            // ע��������ȷ��Pass���֣�Scene�����ֿ����������ͱ��Mod�Լ�ԭ���ͻ����
            /* Filters.Scene["FFZmod:ScreenZoom"] = new Filter(
                new TestScreenShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/Effect1", (AssetRequestMode) 2).Value), "ScreenZoom"),
                EffectPriority.Medium
            ); */
            // Filters.Scene["FFZmod:ScreenZoom"].Load();

            // ע��������ȷ��Pass���֣�Scene�����ֿ����������ͱ��Mod�Լ�ԭ���ͻ����
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