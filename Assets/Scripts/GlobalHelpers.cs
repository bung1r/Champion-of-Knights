using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[Serializable]
public class GlobalPrefabs : MonoBehaviour
{
    public static GlobalPrefabs Instance { get; private set; }
    public GameObject deathVFX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
    }
    public void DeathVFX(Transform target)
    {
        if (deathVFX == null) return;
        if (target == null) return;
        if (target.root.TryGetComponent<BoxCollider>(out var boxCollider))
        {
            Vector3 targetPos = target.position + new Vector3(0, boxCollider.size.y/2, 0);
            // Debug.Log(targetPos);
            GameObject vfx = Instantiate(deathVFX, targetPos, target.rotation);
            Destroy(vfx, 2);
        }
        
    }
}
public enum ModifierTypes
{
    Additive, Multiplicative, Set
}
