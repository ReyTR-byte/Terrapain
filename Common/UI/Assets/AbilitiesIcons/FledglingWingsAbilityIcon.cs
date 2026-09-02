namespace Terrapain.Common.UI.Assets.AbilitiesIcons
{
    public class FledglingWingsAbilityIcon : AbilityIcon
    {
        public override Vector2 DrawCenter => new Vector2(14, 26);
        public FledglingWingsAbilityIcon()
        {
            animationType = 1;
            animationSpeed = 12;
            frameCount = 4;
        }
    }
}