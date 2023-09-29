
using Terraria.ModLoader;

namespace FFZmod.Projectiles
{
    public class VoidCrush : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";
        public override void SetDefaults()
        {
            Projectile.hide = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.ignoreWater = true; // 弹幕是否忽视水
            Projectile.tileCollide = true; // 弹幕撞到物块会创死吗
            Projectile.penetrate = -1; // 弹幕的穿透数，默认1次
            Projectile.friendly = true; // 弹幕是否攻击敌方，默认false
            Projectile.hostile = false; // 弹幕是否攻击友方和城镇NPC，默认false
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
        }
    }
}
