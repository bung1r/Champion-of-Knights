
using UnityEngine;

public class DronePackage : MonoBehaviour
{
    public Item item; 
    public GameObject packagePrefab;
    public Vector3 targetPos;
    private bool droppedPackage = false;
    void Start()
    {
        Destroy(gameObject, 25f);
    }
    public void Update()
    {
        Vector3 pos1 = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 pos2 = new Vector3(targetPos.x, 0, targetPos.z);
        if (Vector3.Distance(pos1, pos2) < 0.5f && !droppedPackage)
        {
            droppedPackage = true;
            GameObject package = Instantiate(packagePrefab, transform.position, Quaternion.identity);
            package.GetComponent<Package>().item = item;
            // Destroy(gameObject);
        }
    }
}