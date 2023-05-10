
public class HealingOnStandStilItem : ItemAbstract
{
    public override void OnStandStill(PlayerController player, int stacks)
    {
        player.health.GetHeal(5 + (2 * stacks));
    }
    
}
