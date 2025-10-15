using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject weaponPrefab;
    private Weapon weapon;
    // public Animator animator;

    void Start()
    {
        GameObject w = Instantiate(weaponPrefab, transform);
        weapon = w.GetComponent<Weapon>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (weapon != null) weapon.TryPrimary(gameObject);
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (weapon != null) weapon.TrySecondary(gameObject);
        }
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        // destroy old visual, instantiate new prefab as child, and get Weapon component
        foreach (Transform t in transform) // naive cleanup if child used; better manage in dedicated holder
        {
            if (t.name == "EquippedWeapon") Destroy(t.gameObject);
        }
        if (weaponPrefab != null)
        {
            GameObject w = Instantiate(weaponPrefab, transform);
            w.name = "EquippedWeapon";
            weapon = w.GetComponent<Weapon>();
            // link animator: prefer weapon's animator, fallback to player animator
            // if (weapon != null && weapon.animator == null)
            //     weapon.animator = animator;
        }
    }
}
