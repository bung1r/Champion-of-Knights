using System;
using UnityEngine;
[CreateAssetMenu(menuName = "AI/AI Melee Action/Complex Attack")]
public class ComplexAttack : AIMeleeAction
{
    [Header("HP Priority Scaling")]
    public HPPriorityScaling HPPS;


    public override float EvaluatePriority(GameObject owner)
    {
        float calculatedPriority = basePriority;
        float hpPercent = healthScript.currentHealth / healthScript.maxHealth;
        if (hpPercent > HPPS.maxHP || hpPercent < HPPS.minHP)
        {
            return 0f;
        }

        if (HPPS.quadraticType == QuadraticType.Standard) calculatedPriority = Mathf.Pow(HPPS.a * hpPercent, 2f) + (HPPS.b * hpPercent) + HPPS.c;
        else if (HPPS.quadraticType == QuadraticType.Vertex) calculatedPriority = (HPPS.a * Mathf.Pow(hpPercent - HPPS.vertex.x, 2f)) + HPPS.vertex.y;
        else if (HPPS.quadraticType == QuadraticType.Factored) calculatedPriority = HPPS.a * ((hpPercent - HPPS.xint1) * (hpPercent - HPPS.xint2));

        calculatedPriority = Mathf.Clamp(calculatedPriority, minPriority, maxPriority);
        return calculatedPriority;
    }
}

[System.Serializable]
public struct HPPriorityScaling
{
    // [Header(("-b+-sqrt(b^2-4ac))/2a HELP HELP HELP"))]
    [Tooltip("If below this HP%, priority is set to 0")]
    public float minHP;

    [Tooltip("If above this HP%, priority is set to 0")]
    public float maxHP;

    //X in this case means % of HP.
    public QuadraticType quadraticType;

    [Header("ax^2+bx+c form (bx + c for linear)")]
    //ax^2, a = 0 means Linear.
    [Tooltip("Use this 'a' for all formulas")]
    public float a;
    //+ bx
    public float b;
    //+ c
    public float c;

    [Header("Vertex Form (fill a in the previous section)")]
    [Tooltip("This one is the easiest to do, so use this.")]
    public Vector2 vertex;

    [Header("Factored Form (Don't use ts...)")]
    public float xint1;
    public float xint2;
    [Header("Optional Stuff")]
    public bool leaveOffAtVertex;
    [Tooltip("Valid Options: Right or Left (of the vertex)")]
    public Direction leaveOffDirection;
    [Tooltip("Only works for bx + c")]
    public bool useAbsoluteValue;

}


public enum QuadraticType
{
    Standard, Vertex, Factored
}