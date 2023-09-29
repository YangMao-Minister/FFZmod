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
    public class NJSword : ModItem
    {

        private int shootCount = 0;

        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.FFZmod.hjson file.
        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.width = 96;
            Item.height = 96;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.15f;
            Item.value = Item.sellPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.crit = 16;
            Item.shoot = ModContent.ProjectileType<NJShockPulse>();
            Item.holdStyle = 5;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Vector2 unit = Vector2.Normalize(Main.MouseWorld - player.Center);
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            //Dust dust = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 91, unit.X * 2.5f, unit.Y * 2.5f, 0, new Color(255, 255, 255), 1.3f);
            //dust.noGravity = true;
            Star.SpawnStars(20);

            base.MeleeEffects(player, hitbox);
        }

        public override bool CanShoot(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            shootCount++;

            // 从玩家到达鼠标位置的单位向量
            Vector2 unit = Vector2.Normalize(Main.MouseWorld - player.Center);
            if (shootCount % 8 == 0)
            {
                shootCount = 0;
                SoundEngine.PlaySound(SoundID.Item105);
                Projectile.NewProjectile(
                    source,
                    player.Center + new Vector2(0, -16),
                    unit * (player.velocity.Length() + 10f) * 1.5f,
                    ModContent.ProjectileType<NJShockPulse>(),
                    (int)(damage * 1.3f), knockback, player.whoAmI, 0, 1f, 4f);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.MaxMana);
                Projectile.NewProjectile(
                    source,
                    player.Center + new Vector2(0, -16),
                    unit * (player.velocity.Length() + 10f),
                    ModContent.ProjectileType<NJShockPulse>(),
                    damage, knockback, player.whoAmI, 0, 0, 3f);


            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }


    }
}