using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

[CreateAssetMenu(menuName = "AI/AI Melee Action/Wolf Bite")]
public class WolfBiteAction : AIMeleeAction
{

    public override float EvaluatePriority(GameObject owner)
    {

        if (healthScript == null || enemyAIScript == null) return basePriority;
        float calculatedPriority = basePriority;
        // do the priority calculations over here, BAKA!
        calculatedPriority = Mathf.Clamp(calculatedPriority, minPriority, maxPriority);

        return calculatedPriority;
    }
    
}