using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/BombItem")]
[Serializable]
public class BombItem : Item
{
    public MeleeHitboxData hitboxData;
    public DamageData damageData;
    public GameObject realBombPrefab;
    public override void Perform(StatManager statManager)
    {
        if (prefab == null) {
            Debug.LogError("No prefab assigned to BombItem!");
            return;
        }
        ItemPerformHandler.Instance.PerformItem(statManager, this);
    }
}