using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using FFZmod.Utils;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FFZmod.Projectiles;
using Microsoft.CodeAnalysis;
using Terraria.Audio;

namespace FFZmod.Items.Weapons
{
    public class SolidEnergy: ModItem
    {
        public override string Texture => "Terraria/Images/Extra_98";
        public override void SetDefaults()
        {

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}
