using UnityEngine;

public class PackageStatManager : EnemyStatManager
{
    public override void Die(DamageData damage)
    {
        RoundManager.Instance.SpawnItemAtPos(GetComponent<Package>().item, transform.position + Vector3.up * 0.2f);
        GlobalPrefabs.Instance.DeathVFX(transform);
        AudioManager.Instance.PlayDeathSFX(transform);
        Destroy(gameObject);
    }
}