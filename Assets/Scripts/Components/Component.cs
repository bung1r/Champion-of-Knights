using UnityEngine;
using System.Collections.Generic;

public abstract class InteractableComponent : MonoBehaviour
{
    public bool canInteract = true;
    public bool constraintReached;
    public List<InteractableComponent> connectedComponents = new List<InteractableComponent>();
    public abstract void MoveComponent(float distance, GameObject owner = null);
    public abstract void DisengageComponent(GameObject owner);
}