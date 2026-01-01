using UnityEngine;

public class DeathBrick : MonoBehaviour
{
    private PlayerStatManager playerStats;
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStatManager>();
    }
    void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<IDamageable>(out var damageable)) {
            damageable.TakeDamage(new DamageData
            {
                baseDamage = 99999999,
                source = null,
                type = DamageType.Fixed
            });
            playerStats.HandleStyleBonus(StyleBonusTypes.NaturalCauses);
        }
    }
}