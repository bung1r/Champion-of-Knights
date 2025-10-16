using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : ScriptableObject
{
    // Start is called before the first frame update
    public List<WeaponAction> moveset;
    public float timeBetweenMoves = 1f;

    private float timeSinceLastMove = 999f;
    private bool canAttack = true;

    public virtual void OnFixedUpdate()
    {
        timeSinceLastMove += Time.fixedDeltaTime;
        if (timeSinceLastMove > timeBetweenMoves)
        {
            canAttack = true;
        }

        if (canAttack)
        {
            
        }

    }
}
