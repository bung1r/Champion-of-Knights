using UnityEngine;
using System.Collections.Generic;

public abstract class InteractableComponent : MonoBehaviour
{
    public bool constraintReached;
    public abstract void MoveComponent(float distance);
}