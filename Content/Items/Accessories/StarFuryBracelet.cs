using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories
{
    public class StarFuryBracelet : ActiveAccessory
    {
        public override void ModSetDefaults()
        {
            Item.width = 50;
            Item.height = 42;
            Item.damage = 20;
            Item.knockBack = 3;
            Item.DamageType = ModContent.GetInstance<Unarmed>();
            AbilityReloadMax = 65;
            abilityIcon = new FallingStarIcon();
            DescriptionLinesCount = 1;
            Item.GetT().activeAccessory = true;
        }
        public override bool OnUseAbility(Player player)
        {   
            player.Custom().ShootingStars(35, 5, (int)player.GetTotalDamage(ModContent.GetInstance<Unarmed>()).ApplyTo(Item.damage), Item.knockBack, 12, Item.DamageType, player.position, Vector2.Zero, true, true, ProjectileID.StarCannonStar);
            return false;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.Custom().unarmed)
            {
                Item.DamageType = ModContent.GetInstance<Unarmed>();
            }
            else
            {
                Item.DamageType = DamageClass.Magic;
            }
            player.Custom().StarFuryBrasslet = true;
        }
    }
}
