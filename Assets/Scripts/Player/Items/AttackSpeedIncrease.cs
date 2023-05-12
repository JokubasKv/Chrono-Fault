
public class AttackSpeedIncrease : ItemAbstract
{
    public override void OnPickup(PlayerController player)
    {
        player.attackSpeed += 0.5f;
    }
    
}
