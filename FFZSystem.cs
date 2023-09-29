using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;

namespace FFZmod
{
    public class FFZSystem : ModSystem
    {
        public override void PreUpdateEntities()
        {
            /*if (!Filters.Scene["SZoom"].IsActive())
            {
                // 开启滤镜
                Filters.Scene.Activate("SZoom");
            }*/
        }
    }
}
