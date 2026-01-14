using UnityEngine;

public class ItemPerformHandler : MonoBehaviour
{
    public static ItemPerformHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PerformItem(StatManager statManager, Item item)
    {
        if (item == null) return;
        if (item is BombItem bombItem)
        {
            GameObject bomb = Instantiate(bombItem.realBombPrefab, statManager.transform.position + statManager.transform.up * 0.8f + statManager.transform.forward * 0.2f, Quaternion.identity);
            if (bomb.TryGetComponent<BombBehavior>(out BombBehavior bombBehavior))
            {
                bomb.GetComponent<Rigidbody>().AddForce(statManager.transform.forward * 5.0f + statManager.transform.up * 2.0f, ForceMode.Impulse);
                bombBehavior.Initialize(bombItem.hitboxData, bombItem.damageData, statManager.gameObject);
            }
        } else
        {
            
        }
    }
}