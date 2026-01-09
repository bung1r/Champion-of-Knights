using UnityEngine;

public class Package : MonoBehaviour
{
    public Item item;
    void Start()
    {
        RoundManager.Instance.AddPackage(gameObject); 
    }
}