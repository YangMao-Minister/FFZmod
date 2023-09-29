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
    public class RBomb : ModItem
    {
        private int explodePower = 400;
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.RBomb>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;
            Item.holdStyle = 5;
            Item.maxStack = Item.CommonMaxStack;

            base.SetDefaults();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) return false;
            Item.stack--;
            Vector2 unit = Vector2.Normalize(Main.MouseWorld - player.Center);
            Projectile.NewProjectile(
                source,
                player.Center + new Vector2(0, -16),
                unit * Item.shootSpeed,
                ModContent.ProjectileType<Projectiles.RBomb>(),
                0, knockback, player.whoAmI, 0, explodePower, 0);
            Main.NewText($"爆炸威力为{explodePower}");
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                explodePower = Item.stack;
                Main.NewText($"将爆炸威力设置为{explodePower}");
            }
            return true;
        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}