using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

//This one always has the same p;riority no matter what. Min and Max Priority do not matter.
[CreateAssetMenu(menuName = "AI/AI Melee Action/Basic Attack")]
public class BasicAttack : AIMeleeAction
{

    public override float EvaluatePriority(GameObject owner)
    {

        // if (healthScript == null || enemyAIScript == null) return basePriority;
        // float calculatedPriority = basePriority;
        // // do the priority calculations over here, BAKA!
        // calculatedPriority = Mathf.Clamp(calculatedPriority, minPriority, maxPriority);

        return basePriority;
    }
    
}