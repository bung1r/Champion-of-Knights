using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSystem : MonoBehaviour
{

    public List<Gear> allGears;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        HashSet<Gear> visited = new HashSet<Gear>();

        // Find all driver gears (multiple possible)
        foreach (var gear in allGears)
        {
            if (gear.isDriver && gear.constraintReached)
            {
                bool changedDirection = Mathf.Sign(gear.savedConstraintRotation) != Mathf.Sign(gear.rotationSpeed) && gear.rotationSpeed != 0f;
                if (changedDirection)
                {
                    gear.ResetAllConstraints();
                }
            }
            if (gear.isDriver && !gear.constraintReached)
                gear.PropagateRotation(gear.rotationSpeed, visited);
        }
    }
}
