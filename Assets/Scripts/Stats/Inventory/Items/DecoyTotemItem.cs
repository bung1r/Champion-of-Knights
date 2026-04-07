using System;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/DecoyTotem")]
[Serializable]
public class DecoyTotemItem : Item
{
    public GameObject decoyTotemPrefab;
    public override void Perform(StatManager statManager)
    {
        
        ItemPerformHandler.Instance.PerformItem(statManager, this);
    }
}