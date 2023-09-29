using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FFZmod.Utils;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace FFZmod.Items.Test
{
    public class TestItem : ModItem
    {
        public override string Texture => "Terraria/Images/Projectile_919";

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightningArc>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            LightningArc p = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<LightningArc>(), damage, knockback, player.whoAmI).ModProjectile as LightningArc;
            p.initLightningArc(player.position, Main.MouseWorld, 10, Main.rand.Next(10, 60), Color.Lerp(Color.Blue, Color.Red, Main.rand.NextFloat()), genFloatingRange: 0.4f);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}