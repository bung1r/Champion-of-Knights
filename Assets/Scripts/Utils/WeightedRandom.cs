using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom
{
    public static T Choose<T>(List<T> items, List<float> weights)
    {
        if (items.Count != weights.Count || items.Count == 0)
        {
            Debug.LogError("Items and weights lists must have the same non-zero length.");
            return default;
        }

        // Sum all weights
        float totalWeight = 0f;
        foreach (float w in weights)
            totalWeight += Mathf.Max(0, w); // prevent negative weights

        // Random point in the total weight
        float randomPoint = Random.value * totalWeight;

        // Find which item this point falls into
        for (int i = 0; i < items.Count; i++)
        {
            if (randomPoint < weights[i])
                return items[i];

            randomPoint -= weights[i];
        }

        // Fallback (shouldn’t happen, but safe)
        return items[items.Count - 1];
    }
}