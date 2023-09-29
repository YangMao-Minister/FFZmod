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
    public class VoidCrush: ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 1.15f;
            Item.value = Item.sellPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item118;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.crit = 16;
            Item.shoot = ModContent.ProjectileType<Projectiles.VoidCrush>();
            Item.holdStyle = 5;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}
